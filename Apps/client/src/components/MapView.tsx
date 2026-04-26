"use client";

import { useEffect, useMemo, useState } from "react";
import dynamic from "next/dynamic";

import type {
  Node,
  Road,
  ShortestPathNodeDto,
  AlgorithmResponse,
  ShortestPathResultDto,
  MstResultDto,
  MaintenancePlanningResultDto,
  TrafficSignalResultDto,
} from "@/types";
import {
  getShortestPath,
  getTimeVaryingShortestPath,
} from "@/services/routes/routePlanning";
import { getEmergencyRoute } from "@/services/routes/emergencyRouting";
import { getCheapestNetwork } from "@/services/network/networkExpansion";
import { getMaintenancePlan } from "@/services/maintenance/maintenanceStrategy";
import { getSignalOptimization } from "@/services/traffic/signalOptimization";

// Dynamically import Leaflet components only on client side
const MapContainer = dynamic(
  () => import("react-leaflet").then((mod) => mod.MapContainer),
  { ssr: false },
);
const TileLayer = dynamic(
  () => import("react-leaflet").then((mod) => mod.TileLayer),
  { ssr: false },
);
const Marker = dynamic(
  () => import("react-leaflet").then((mod) => mod.Marker),
  { ssr: false },
);
const Popup = dynamic(() => import("react-leaflet").then((mod) => mod.Popup), {
  ssr: false,
});
const Polyline = dynamic(
  () => import("react-leaflet").then((mod) => mod.Polyline),
  { ssr: false },
);

interface MapViewProps {
  nodes: Node[];
  edges: Road[];
}

type AlgorithmType =
  | "dijkstra"
  | "astar"
  | "time-varying"
  | "maintenance"
  | "signals";

const PERIODS = ["morning", "evening", "night"];

function MapInner({ nodes, edges }: MapViewProps) {
  const [startId, setStartId] = useState<string | null>(null);
  const [endId, setEndId] = useState<string | null>(null);
  const [pathNodes, setPathNodes] = useState<ShortestPathNodeDto[]>([]);
  const [pathDistance, setPathDistance] = useState<number | null>(null);
  const [algorithm, setAlgorithm] = useState<AlgorithmType>("dijkstra");
  const [period, setPeriod] = useState<string>("morning");
  const [budget, setBudget] = useState<number>(50);
  const [response, setResponse] =
    useState<AlgorithmResponse<ShortestPathResultDto> | null>(null);
  const [maintenanceResponse, setMaintenanceResponse] =
    useState<AlgorithmResponse<MaintenancePlanningResultDto> | null>(null);
  const [signalResponse, setSignalResponse] =
    useState<AlgorithmResponse<TrafficSignalResultDto> | null>(null);
  const [topN, setTopN] = useState<number>(10);
  const [loading, setLoading] = useState(false);
  const [leaflet, setLeaflet] = useState<unknown | null>(null);
  const [mstEdges, setMstEdges] = useState<Road[]>([]);
  const [selectedRoad, setSelectedRoad] = useState<Road | null>(null);
  const [selectedNode, setSelectedNode] = useState<Node | null>(null);
  const [showMst, setShowMst] = useState(false);
  const [mstResponse, setMstResponse] =
    useState<AlgorithmResponse<MstResultDto> | null>(null);

  // Import Leaflet only on client side
  useEffect(() => {
    import("leaflet").then((L) => {
      setLeaflet(L);
      // Fix default Leaflet icons
      // eslint-disable-next-line @typescript-eslint/no-explicit-any
      delete (L.Icon.Default.prototype as any)._getIconUrl;

      L.Icon.Default.mergeOptions({
        iconRetinaUrl:
          "https://unpkg.com/leaflet@1.9.4/dist/images/marker-icon-2x.png",
        iconUrl: "https://unpkg.com/leaflet@1.9.4/dist/images/marker-icon.png",
        shadowUrl:
          "https://unpkg.com/leaflet@1.9.4/dist/images/marker-shadow.png",
      });
    });
  }, []);

  // Fetch MST data on component load
  useEffect(() => {
    getCheapestNetwork().then((res) => {
      setMstResponse(res);
      if (res.success && res.data.selectedRoads) {
        const mstRoads: Road[] = res.data.selectedRoads.map((r) => ({
          id: r.id,
          fromNodeId: r.fromNodeId,
          toNodeId: r.toNodeId,
          distance: r.distance,
          capacity: r.capacity,
          condition: r.condition,
          isExisting: r.isExisting,
          constructionCost: r.constructionCost,
        }));
        setMstEdges(mstRoads);
      }
    });
  }, []);

  // Icons (IMPORTANT: always return a valid icon)
  const icons = useMemo(() => {
    if (!leaflet) return null;

    // eslint-disable-next-line @typescript-eslint/no-explicit-any
    const L = leaflet as any;
    return {
      default: new L.Icon.Default(),

      green: L.divIcon({
        className: "custom-marker",
        html: `<div style="background-color:#22c55e;width:12px;height:12px;border-radius:50%;border:2px solid white;"></div>`,
        iconSize: [12, 12],
        iconAnchor: [6, 6],
      }),

      red: L.divIcon({
        className: "custom-marker",
        html: `<div style="background-color:#ef4444;width:12px;height:12px;border-radius:50%;border:2px solid white;"></div>`,
        iconSize: [12, 12],
        iconAnchor: [6, 6],
      }),
    };
  }, [leaflet]);

  // Fetch shortest path based on selected algorithm
  useEffect(() => {
    if (algorithm === "maintenance") {
      // Maintenance doesn't need start/end nodes
      return;
    }

    if (!startId || !endId) {
      // eslint-disable-next-line
      setPathNodes([]);
      setPathDistance(null);
      setResponse(null);
      return;
    }

    let cancelled = false;
    setLoading(true);

    const fetchRoute = async () => {
      try {
        let res: AlgorithmResponse<ShortestPathResultDto>;

        switch (algorithm) {
          case "astar":
            res = await getEmergencyRoute(startId, endId);
            break;
          case "time-varying":
            res = await getTimeVaryingShortestPath(startId, endId, period);
            break;
          case "dijkstra":
          default:
            res = await getShortestPath(startId, endId);
        }

        if (cancelled) return;

        setResponse(res);

        if (res.success && res.data.found) {
          setPathNodes(res.data.pathNodes);
          setPathDistance(res.data.totalDistance);
        } else {
          setPathNodes([]);
          setPathDistance(null);
        }
      } catch {
        // Silently handle error
      } finally {
        if (!cancelled) {
          setLoading(false);
        }
      }
    };

    fetchRoute();

    return () => {
      cancelled = true;
    };
  }, [startId, endId, algorithm, period]);

  const handleCalculateMaintenance = async () => {
    setLoading(true);
    try {
      const res = await getMaintenancePlan(budget);
      setMaintenanceResponse(res);
    } catch {
      // Silently handle error
    } finally {
      setLoading(false);
    }
  };

  const handleResetMaintenance = () => {
    setMaintenanceResponse(null);
  };

  const handleCalculateSignals = async () => {
    setLoading(true);
    try {
      const res = await getSignalOptimization(period, topN, false);
      setSignalResponse(res);
    } catch {
      // Silently handle error
    } finally {
      setLoading(false);
    }
  };

  const handleResetSignals = () => {
    setSignalResponse(null);
  };

  // Marker click logic
  const handleMarkerClick = (id: string) => {
    const node = nodeLookup[id];
    setSelectedNode(node || null);
    setSelectedRoad(null);

    if (!startId) {
      setStartId(id);
    } else if (!endId) {
      setEndId(id);
    } else {
      if (id === startId) {
        // Clicked on start node - deselect both
        setStartId(null);
        setEndId(null);
        setPathNodes([]);
        setPathDistance(null);
        setResponse(null);
      } else {
        // Keep start, update end to new node
        setEndId(id);
      }
    }
  };

  // Road click logic
  const handleRoadClick = (edge: Road) => {
    setSelectedRoad(edge);
    setSelectedNode(null);
  };

  const handleReset = () => {
    setStartId(null);
    setEndId(null);
    setPathNodes([]);
    setPathDistance(null);
    setResponse(null);
  };

  const handleAlgorithmChange = (algo: AlgorithmType) => {
    setAlgorithm(algo);
    setPathNodes([]);
    setPathDistance(null);
    setResponse(null);
    setMaintenanceResponse(null);
    setSignalResponse(null);
  };

  // Lookup for nodes
  const nodeLookup = useMemo(() => {
    const map: Record<string, Node> = {};
    nodes.forEach((n) => (map[n.id] = n));
    return map;
  }, [nodes]);

  const nodeIdByName = useMemo(() => {
    const map: Record<string, string> = {};
    nodes.forEach((n) => {
      map[n.name.trim().toLowerCase()] = n.id;
    });
    return map;
  }, [nodes]);

  // Draw all roads
  const edgeSegments = useMemo(() => {
    return edges
      .map((edge) => {
        const from = nodeLookup[edge.fromNodeId];
        const to = nodeLookup[edge.toNodeId];
        if (!from || !to) return null;

        return {
          edge,
          pos: [
            [from.y, from.x],
            [to.y, to.x],
          ] as [number, number][],
        };
      })
      .filter(Boolean) as { edge: Road; pos: [number, number][] }[];
  }, [edges, nodeLookup]);

  // Draw shortest path
  const pathPositions = useMemo(() => {
    return pathNodes
      .filter(
        (n): n is ShortestPathNodeDto & { x: number; y: number } =>
          n.x !== undefined && n.y !== undefined,
      )
      .map((n) => [n.y, n.x] as [number, number]);
  }, [pathNodes]);

  // Draw MST roads
  const mstPositions = useMemo(() => {
    return mstEdges
      .map((edge) => {
        const from = nodeLookup[edge.fromNodeId];
        const to = nodeLookup[edge.toNodeId];
        if (!from || !to) return null;

        return [
          [from.y, from.x],
          [to.y, to.x],
        ] as [number, number][];
      })
      .filter(Boolean) as [number, number][][];
  }, [mstEdges, nodeLookup]);

  const getMarkerIcon = (id: string) => {
    if (!icons) return undefined;
    if (id === startId) return icons.green;
    if (id === endId) return icons.red;
    return icons.default;
  };

  // Get road color based on priority (for maintenance mode)
  const getRoadPriorityColor = (road: Road) => {
    if (!road.maintenancePriority) return "#9ca3af"; // gray if no priority
    if (road.maintenancePriority >= 7) return "#ef4444"; // red for high priority
    if (road.maintenancePriority >= 4) return "#f59e0b"; // yellow for medium priority
    return "#22c55e"; // green for low priority
  };

  // Check if road is selected for maintenance
  const isRoadSelectedForMaintenance = (road: Road) => {
    if (!maintenanceResponse) return false;

    // Try matching by road ID first
    const matchById = maintenanceResponse.data.selectedRoads.some(
      (r) => r.roadId === Math.abs(road.id),
    );
    if (matchById) return true;

    // Fallback: try matching by from/to location names
    const fromNode = nodeLookup[road.fromNodeId];
    const toNode = nodeLookup[road.toNodeId];
    if (fromNode && toNode) {
      const fromName = fromNode.name.trim().toLowerCase();
      const toName = toNode.name.trim().toLowerCase();

      const matchByLocation = maintenanceResponse.data.selectedRoads.some(
        (r) =>
          (r.fromLocation ?? "").trim().toLowerCase() === fromName &&
          (r.toLocation ?? "").trim().toLowerCase() === toName,
      );
      if (matchByLocation) return true;

      // Try reverse direction
      const matchByLocationReverse =
        maintenanceResponse.data.selectedRoads.some(
          (r) =>
            (r.fromLocation ?? "").trim().toLowerCase() === toName &&
            (r.toLocation ?? "").trim().toLowerCase() === fromName,
        );
      if (matchByLocationReverse) return true;
    }

    // Fallback 2: map maintenance from/to names to node IDs and match by endpoints
    for (const r of maintenanceResponse.data.selectedRoads) {
      const fromId = nodeIdByName[(r.fromLocation ?? "").trim().toLowerCase()];
      const toId = nodeIdByName[(r.toLocation ?? "").trim().toLowerCase()];

      if (!fromId || !toId) continue;

      if (road.fromNodeId === fromId && road.toNodeId === toId) return true;
      if (road.fromNodeId === toId && road.toNodeId === fromId) return true;
    }

    return false;
  };

  const statusText =
    algorithm === "maintenance"
      ? loading
        ? "Calculating maintenance plan..."
        : maintenanceResponse?.success
          ? `Selected: ${maintenanceResponse.data.selectedRoadCount} roads`
          : "Adjust budget and calculate"
      : algorithm === "signals"
        ? loading
          ? "Optimizing signals..."
          : signalResponse?.success
            ? `${signalResponse.data.summary.optimizedIntersections} intersections optimized`
            : "Select period and calculate"
        : !startId
          ? "Click a location to set start"
          : !endId
            ? "Click a location to set destination"
            : loading
              ? "Calculating..."
              : pathNodes.length > 0 && pathDistance !== null
                ? `Path: ${pathDistance.toFixed(1)} km`
                : "No path found";

  // Map intersection signals from API response with node lookup
  const intersectionSignals = useMemo(() => {
    if (!signalResponse?.success) return null;

    const groups: Record<
      string,
      {
        intersectionName: string;
        nodeId: string | null;
        cycleTimeSeconds: number;
        signals: (typeof signalResponse.data.intersections)[0]["roads"];
        maxCongestion: number;
      }
    > = {};

    for (const intersection of signalResponse.data.intersections) {
      const key = intersection.name;
      // Try to find node ID from location name
      const nodeId = nodeIdByName[key.trim().toLowerCase()] ?? null;

      // Calculate max congestion from roads
      const maxCongestion = Math.max(
        ...intersection.roads.map((r) => r.congestionPercent / 100),
      );

      groups[key] = {
        intersectionName: key,
        nodeId,
        cycleTimeSeconds: intersection.cycleTimeSeconds,
        signals: intersection.roads,
        maxCongestion,
      };
    }

    return groups;
  }, [signalResponse, nodeIdByName]);

  // Get intersection severity color
  const getIntersectionSeverityColor = (maxCongestion: number) => {
    if (maxCongestion > 1.0) return "#ef4444"; // Red - critical
    if (maxCongestion > 0.7) return "#f97316"; // Orange - high
    if (maxCongestion > 0.5) return "#f59e0b"; // Yellow - moderate
    return "#3b82f6"; // Blue - normal
  };

  // Get road congestion color (similar to maintenance priority)
  const getRoadCongestionColor = (congestionPercent: number) => {
    if (congestionPercent > 100) return "#ef4444"; // red - critical
    if (congestionPercent > 70) return "#f97316"; // orange - high
    if (congestionPercent > 50) return "#f59e0b"; // yellow - moderate
    return "#22c55e"; // green - normal
  };

  // Check if road is in signal response and get its congestion
  const getRoadCongestionFromSignal = (road: Road) => {
    if (!signalResponse?.success) return null;

    for (const intersection of signalResponse.data.intersections) {
      for (const signalRoad of intersection.roads) {
        // Try matching by location names
        const fromNode = nodeLookup[road.fromNodeId];
        const toNode = nodeLookup[road.toNodeId];
        if (!fromNode || !toNode) continue;

        const fromMatch =
          fromNode.name.trim().toLowerCase() ===
          signalRoad.from.trim().toLowerCase();
        const toMatch =
          toNode.name.trim().toLowerCase() ===
          signalRoad.to.trim().toLowerCase();

        if (toMatch && fromMatch) {
          return signalRoad.congestionPercent;
        }
      }
    }

    return null;
  };

  const resultClassName = "ml-1 font-medium text-black";
  return (
    <div className="relative h-full w-full">
      {/* Dashboard Panel */}
      <div className="absolute left-4 top-4 z-[1000] max-w-xs rounded-lg bg-white p-4 shadow-lg">
        {/* Algorithm Selection */}
        <div className="mb-4">
          <p className="mb-2 text-xs font-semibold uppercase text-gray-500">
            Algorithm
          </p>
          <div className="flex gap-2 flex-wrap">
            {[
              { key: "dijkstra", label: "Dijkstra" },
              { key: "astar", label: "A*" },
              { key: "time-varying", label: "Time-Varying" },
              { key: "maintenance", label: "Maintenance" },
              { key: "signals", label: "Signals" },
            ].map(({ key, label }) => (
              <button
                key={key}
                onClick={() => handleAlgorithmChange(key as AlgorithmType)}
                className={`rounded px-2 py-1 text-xs font-medium ${
                  algorithm === key
                    ? "bg-blue-500 text-white"
                    : "bg-gray-100 text-gray-700 hover:bg-gray-200"
                }`}
              >
                {label}
              </button>
            ))}
          </div>
        </div>

        {/* Period Selection (only for time-varying) */}
        {algorithm === "time-varying" && (
          <div className="mb-4">
            <p className="mb-2 text-xs font-semibold uppercase text-gray-500">
              Period
            </p>
            <select
              value={period}
              onChange={(e) => setPeriod(e.target.value)}
              className="w-full rounded border border-gray-300 px-2 py-1 text-sm"
            >
              {PERIODS.map((p) => (
                <option key={p} value={p}>
                  {p.charAt(0).toUpperCase() + p.slice(1)}
                </option>
              ))}
            </select>
          </div>
        )}

        {/* Budget Selection (only for maintenance) */}
        {algorithm === "maintenance" && (
          <div className="mb-4">
            <p className="mb-2 text-xs font-semibold uppercase text-gray-500">
              Budget ($)
            </p>
            <input
              type="number"
              min="10"
              max="1000"
              step="1"
              value={budget}
              onChange={(e) => setBudget(Number(e.target.value))}
              className="w-full rounded border border-gray-300 px-2 py-1 text-sm text-black"
            />
            <div className="mt-2 flex gap-2">
              <button
                onClick={handleCalculateMaintenance}
                className="flex-1 rounded bg-orange-500 px-2 py-2 text-xs font-medium text-white hover:bg-orange-600"
                disabled={loading}
              >
                Calculate Plan
              </button>
              <button
                onClick={handleResetMaintenance}
                className="flex-1 rounded bg-gray-100 px-2 py-2 text-xs font-medium text-gray-700 hover:bg-gray-200"
                disabled={loading}
              >
                Reset View
              </button>
            </div>
          </div>
        )}

        {/* Signal Optimization (only for signals) */}
        {algorithm === "signals" && (
          <div className="mb-4">
            <p className="mb-2 text-xs font-semibold uppercase text-gray-500">
              Period
            </p>
            <select
              value={period}
              onChange={(e) => setPeriod(e.target.value)}
              className="w-full rounded border border-gray-300 px-2 py-1 text-sm mb-2"
            >
              {PERIODS.map((p) => (
                <option key={p} value={p}>
                  {p.charAt(0).toUpperCase() + p.slice(1)}
                </option>
              ))}
            </select>
            <p className="mb-2 text-xs font-semibold uppercase text-gray-500">
              Top N Roads
            </p>
            <input
              type="number"
              min="1"
              max="50"
              step="1"
              value={topN}
              onChange={(e) => setTopN(Number(e.target.value))}
              className="w-full rounded border border-gray-300 px-2 py-1 text-sm text-black"
            />
            <div className="mt-2 flex gap-2">
              <button
                onClick={handleCalculateSignals}
                className="flex-1 rounded bg-blue-500 px-2 py-2 text-xs font-medium text-white hover:bg-blue-600"
                disabled={loading}
              >
                Calculate
              </button>
              <button
                onClick={handleResetSignals}
                className="flex-1 rounded bg-gray-100 px-2 py-2 text-xs font-medium text-gray-700 hover:bg-gray-200"
                disabled={loading}
              >
                Reset
              </button>
            </div>
          </div>
        )}

        {/* MST Toggle */}
        <div className="mb-4">
          <button
            onClick={() => setShowMst(!showMst)}
            className={`w-full rounded px-3 py-2 text-xs font-medium ${
              showMst
                ? "bg-blue-500 text-white"
                : "bg-gray-100 text-gray-700 hover:bg-gray-200"
            }`}
          >
            {showMst ? "Hide MST" : "Show MST"}
          </button>
        </div>

        {/* Status */}
        <p className="mb-3 text-sm font-medium text-gray-800">{statusText}</p>

        {/* Selected Info Panel */}
        {(selectedNode || selectedRoad) && (
          <div className="mb-3 rounded-md bg-blue-50 p-3 text-xs">
            <p className="mb-2 font-semibold text-gray-700">
              {selectedNode ? "Node Info" : "Road Info"}
            </p>
            {selectedNode && (
              <div className="space-y-1">
                <div>
                  <span className="text-gray-500">Name:</span>
                  <span className="ml-1 font-medium text-black">
                    {selectedNode.name}
                  </span>
                </div>
                <div>
                  <span className="text-gray-500">ID:</span>
                  <span className="ml-1 font-medium text-black">
                    {selectedNode.id}
                  </span>
                </div>
                <div>
                  <span className="text-gray-500">Type:</span>
                  <span className="ml-1 font-medium text-black">
                    {selectedNode.type}
                  </span>
                </div>
                {selectedNode.population && (
                  <div>
                    <span className="text-gray-500">Population:</span>
                    <span className="ml-1 font-medium text-black">
                      {selectedNode.population.toLocaleString()}
                    </span>
                  </div>
                )}
                <div>
                  <span className="text-gray-500">Coordinates:</span>
                  <span className="ml-1 font-medium text-black">
                    ({selectedNode.y.toFixed(4)}, {selectedNode.x.toFixed(4)})
                  </span>
                </div>
                <div>
                  <span className="text-gray-500">Critical:</span>
                  <span className="ml-1 font-medium text-black">
                    {selectedNode.isCritical ? "Yes" : "No"}
                  </span>
                </div>
                {/* Signal Optimization Data (when available) */}
                {(
                  selectedNode as Node & {
                    signalData?: typeof intersectionSignals extends Record<
                      string,
                      infer V
                    >
                      ? V
                      : never;
                  }
                ).signalData && (
                  <>
                    <div className="pt-2 border-t border-gray-200 mt-2">
                      <span className="font-semibold text-gray-700">
                        Signal Optimization:
                      </span>
                    </div>
                    <div>
                      <span className="text-gray-500">Intersection:</span>
                      <span className="ml-1 font-medium text-black">
                        {
                          (
                            selectedNode as Node & {
                              signalData: { intersectionName: string };
                            }
                          ).signalData.intersectionName
                        }
                      </span>
                    </div>
                    <div>
                      <span className="text-gray-500">Cycle Time:</span>
                      <span className="ml-1 font-medium text-black">
                        {
                          (
                            selectedNode as Node & {
                              signalData: { cycleTimeSeconds: number };
                            }
                          ).signalData.cycleTimeSeconds
                        }
                        s
                      </span>
                    </div>
                    <div>
                      <span className="text-gray-500">Roads Optimized:</span>
                      <span className="ml-1 font-medium text-black">
                        {
                          (
                            selectedNode as Node & {
                              signalData: { signals: unknown[] };
                            }
                          ).signalData.signals.length
                        }
                      </span>
                    </div>
                    <div>
                      <span className="text-gray-500">Max Congestion:</span>
                      <span className="ml-1 font-medium text-black">
                        {(
                          (
                            selectedNode as Node & {
                              signalData: { maxCongestion: number };
                            }
                          ).signalData.maxCongestion * 100
                        ).toFixed(0)}
                        %
                      </span>
                    </div>
                    <div className="mt-2">
                      <span className="font-semibold text-gray-600 text-xs">
                        Signal Phases:
                      </span>
                      {(
                        selectedNode as Node & {
                          signalData: {
                            signals: Array<{
                              from: string;
                              to: string;
                              congestionPercent: number;
                              priority: number;
                              greenTimeSeconds: number;
                            }>;
                          };
                        }
                      ).signalData.signals.map((signal, idx) => (
                        <div
                          key={idx}
                          className="mt-1 pl-2 border-l-2 border-blue-300"
                        >
                          <p className="text-xs font-medium text-black">
                            {signal.from} → {signal.to}
                          </p>
                          <p className="text-xs text-gray-600 text-black">
                            Green: {signal.greenTimeSeconds}s (Cycle:{" "}
                            {
                              (
                                selectedNode as Node & {
                                  signalData: { cycleTimeSeconds: number };
                                }
                              ).signalData.cycleTimeSeconds
                            }
                            s)
                          </p>
                          <p className="text-xs text-gray-600 text-black">
                            Congestion: {signal.congestionPercent.toFixed(0)}%
                            (Priority #{signal.priority})
                          </p>
                        </div>
                      ))}
                    </div>
                  </>
                )}
              </div>
            )}
            {selectedRoad && (
              <div className="space-y-1">
                <div>
                  <span className="text-gray-500">From:</span>
                  <span className="ml-1 font-medium text-black">
                    {nodeLookup[selectedRoad.fromNodeId]?.name ||
                      selectedRoad.fromNodeId}
                  </span>
                </div>
                <div>
                  <span className="text-gray-500">To:</span>
                  <span className="ml-1 font-medium text-black">
                    {nodeLookup[selectedRoad.toNodeId]?.name ||
                      selectedRoad.toNodeId}
                  </span>
                </div>
                <div>
                  <span className="text-gray-500">Distance:</span>
                  <span className="ml-1 font-medium text-black">
                    {selectedRoad.distance.toFixed(2)} km
                  </span>
                </div>
                <div>
                  <span className="text-gray-500">Capacity:</span>
                  <span className="ml-1 font-medium text-black">
                    {selectedRoad.capacity.toLocaleString()}
                  </span>
                </div>
                {selectedRoad.condition && (
                  <div>
                    <span className="text-gray-500">Condition:</span>
                    <span className="ml-1 font-medium text-black">
                      {selectedRoad.condition}/10
                    </span>
                  </div>
                )}
                <div>
                  <span className="text-gray-500">Existing:</span>
                  <span className="ml-1 font-medium text-black">
                    {selectedRoad.isExisting ? "Yes" : "No"}
                  </span>
                </div>
                {selectedRoad.constructionCost && (
                  <div>
                    <span className="text-gray-500">Construction Cost:</span>
                    <span className="ml-1 font-medium text-black">
                      ${selectedRoad.constructionCost.toFixed(0)}
                    </span>
                  </div>
                )}
                {selectedRoad.maintenancePriority && (
                  <div className="pt-2 border-t border-gray-200">
                    <span className="font-semibold text-gray-700">
                      Maintenance Info:
                    </span>
                  </div>
                )}
                {selectedRoad.maintenancePriority && (
                  <div>
                    <span className="text-gray-500">Priority:</span>
                    <span className="ml-1 font-medium text-black">
                      {selectedRoad.maintenancePriority}/10
                    </span>
                  </div>
                )}
                {selectedRoad.maintenanceCost && (
                  <div>
                    <span className="text-gray-500">Est. Cost:</span>
                    <span className="ml-1 font-medium text-black">
                      ${selectedRoad.maintenanceCost.toFixed(0)}
                    </span>
                  </div>
                )}
              </div>
            )}
          </div>
        )}

        {/* Results Dashboard */}
        {response && (
          <div className="mb-3 rounded-md bg-gray-50 p-3 text-xs">
            <p className="mb-1 font-semibold text-gray-700">Results:</p>
            <div className="grid grid-cols-2 gap-2">
              <div>
                <span className="text-gray-500">Algorithm:</span>
                <span className={resultClassName}>
                  {response.algorithmName}
                </span>
              </div>
              <div>
                <span className="text-gray-500">Time:</span>
                <span className={resultClassName}>
                  {response.trace.executionTimeMs.toFixed(2)}ms
                </span>
              </div>
              <div>
                <span className="text-gray-500">Visited:</span>
                <span className={resultClassName}>
                  {response.trace.visitedNodes}
                </span>
              </div>
              <div>
                <span className="text-gray-500">Expanded:</span>
                <span className={resultClassName}>
                  {response.trace.expandedNodes}
                </span>
              </div>
              <div>
                <span className="text-gray-500">Distance:</span>
                <span className={resultClassName}>
                  {response.data.totalDistance.toFixed(1)}km
                </span>
              </div>
              <div>
                <span className="text-gray-500">Roads:</span>
                <span className={resultClassName}>
                  {response.data.pathRoads.length}
                </span>
              </div>
            </div>
            {response.message && (
              <p className="mt-2 text-gray-600">{response.message}</p>
            )}
          </div>
        )}

        {/* MST Results Dashboard */}
        {showMst && mstResponse && (
          <div className="mb-3 rounded-md bg-green-50 p-3 text-xs">
            <p className="mb-1 font-semibold text-gray-700">MST Results:</p>
            <div className="grid grid-cols-2 gap-2">
              <div>
                <span className="text-gray-500">Connected:</span>
                <span className={resultClassName}>
                  {mstResponse.data.connected ? "Yes" : "No"}
                </span>
              </div>
              <div>
                <span className="text-gray-500">Total Cost:</span>
                <span className={resultClassName}>
                  ${mstResponse.data.totalConstructionCost.toFixed(0)}
                </span>
              </div>
              <div>
                <span className="text-gray-500">Total Nodes:</span>
                <span className={resultClassName}>
                  {mstResponse.data.totalNodes}
                </span>
              </div>
              <div>
                <span className="text-gray-500">Selected Roads:</span>
                <span className={resultClassName}>
                  {mstResponse.data.selectedRoadCount}
                </span>
              </div>
            </div>
            {mstResponse.message && (
              <p className="mt-2 text-gray-600">{mstResponse.message}</p>
            )}
          </div>
        )}

        {/* Maintenance Results Dashboard */}
        {algorithm === "maintenance" && maintenanceResponse && (
          <div className="mb-3 rounded-md bg-orange-50 p-3 text-xs">
            <p className="mb-1 font-semibold text-gray-700">
              Maintenance Plan:
            </p>
            <p className="mb-2 text-xs text-gray-600">
              Matched roads on map:{" "}
              {
                edgeSegments.filter(({ edge }) =>
                  isRoadSelectedForMaintenance(edge),
                ).length
              }
            </p>
            <div className="grid grid-cols-2 gap-2">
              <div>
                <span className="text-gray-500">Budget:</span>
                <span className={resultClassName}>
                  ${maintenanceResponse.data.budget.toLocaleString()}
                </span>
              </div>
              <div>
                <span className="text-gray-500">Condition Imp:</span>
                <span className={resultClassName}>
                  +
                  {maintenanceResponse.data.expectedConditionImprovement.toFixed(
                    1,
                  )}
                </span>
              </div>
            </div>
            <div className="mt-2 pt-2 border-t border-orange-200">
              <p className="font-semibold text-gray-700">Selected Roads:</p>
              {maintenanceResponse.data.selectedRoads.map((r) => (
                <div key={r.roadId} className="mt-1">
                  <span className="text-gray-600">
                    {r.fromLocation} → {r.toLocation} (ID: {r.roadId})
                  </span>
                </div>
              ))}
            </div>
            {maintenanceResponse.message && (
              <p className="mt-2 text-gray-600">
                {maintenanceResponse.message}
              </p>
            )}
          </div>
        )}

        {/* Signal Optimization Results Dashboard */}
        {algorithm === "signals" && signalResponse && (
          <div className="mb-3 rounded-md bg-blue-50 p-3 text-xs">
            <p className="mb-1 font-semibold text-gray-700">
              Signal Optimization:
            </p>
            <p className="mb-2 text-xs text-gray-600">
              Period: {signalResponse.data.period}
            </p>
            <div className="grid grid-cols-2 gap-2">
              <div>
                <span className="text-gray-500">Intersections:</span>
                <span className={resultClassName}>
                  {signalResponse.data.summary.optimizedIntersections}
                </span>
              </div>
              <div>
                <span className="text-gray-500">Roads:</span>
                <span className={resultClassName}>
                  {signalResponse.data.summary.roadsAnalyzed}
                </span>
              </div>
              <div>
                <span className="text-gray-500">Wait Reduction:</span>
                <span className={resultClassName}>
                  {signalResponse.data.summary.estimatedWaitTimeReductionPercent.toFixed(
                    1,
                  )}
                  %
                </span>
              </div>
              <div>
                <span className="text-gray-500">Analyzed:</span>
                <span className={resultClassName}>
                  {signalResponse.data.summary.intersectionsAnalyzed}
                </span>
              </div>
            </div>
            <div className="mt-2 pt-2 border-t border-blue-200">
              <p className="font-semibold text-gray-700">Top Intersections:</p>
              {signalResponse.data.intersections
                .sort((a, b) => {
                  const maxA = Math.max(
                    ...a.roads.map((r) => r.congestionPercent / 100),
                  );
                  const maxB = Math.max(
                    ...b.roads.map((r) => r.congestionPercent / 100),
                  );
                  return maxB - maxA;
                })
                .slice(0, 5)
                .map((intersection) => {
                  const maxCongestion = Math.max(
                    ...intersection.roads.map((r) => r.congestionPercent / 100),
                  );
                  return (
                    <div key={intersection.name} className="mt-1">
                      <span
                        className="inline-block w-2 h-2 rounded-full mr-1"
                        style={{
                          backgroundColor:
                            getIntersectionSeverityColor(maxCongestion),
                        }}
                      />
                      <span className="text-gray-600">
                        {intersection.name} ({intersection.roads.length} roads,{" "}
                        {intersection.cycleTimeSeconds}s cycle,{" "}
                        {(maxCongestion * 100).toFixed(0)}% max)
                      </span>
                    </div>
                  );
                })}
            </div>
            {signalResponse.message && (
              <p className="mt-2 text-gray-600">{signalResponse.message}</p>
            )}
          </div>
        )}

        {/* Actions */}
        {(startId || endId) && (
          <button
            onClick={handleReset}
            className="w-full rounded bg-gray-200 px-3 py-2 text-xs font-medium text-gray-700 hover:bg-gray-300"
          >
            Clear Selection
          </button>
        )}
      </div>

      <MapContainer
        center={[30.0444, 31.2357]}
        zoom={12}
        className="h-full w-full"
      >
        <TileLayer
          url="https://{s}.basemaps.cartocdn.com/light_all/{z}/{x}/{y}{r}.png"
          attribution="&copy; OpenStreetMap contributors &copy; CARTO"
        />

        {/* Roads */}
        {edgeSegments.map(({ edge, pos }, i) => {
          let color = "#9ca3af";
          let weight = 2;
          let opacity = 0.6;

          const highlightSelected =
            algorithm === "maintenance" &&
            !!maintenanceResponse &&
            isRoadSelectedForMaintenance(edge);

          if (algorithm === "maintenance") {
            if (maintenanceResponse) {
              // After calculation: show selected roads in orange, others in gray
              if (isRoadSelectedForMaintenance(edge)) {
                color = "#f97316"; // orange for selected
                weight = 6;
                opacity = 1.0;
              } else {
                color = "#d1d5db"; // lighter gray for not selected
                weight = 1;
                opacity = 0.2;
              }
            } else {
              // Before calculation: show roads by priority
              color = getRoadPriorityColor(edge);
              weight = 3;
              opacity = 0.8;
            }
          } else if (algorithm === "signals" && signalResponse) {
            // Show roads by congestion from signal response
            const congestion = getRoadCongestionFromSignal(edge);
            if (congestion !== null) {
              color = getRoadCongestionColor(congestion);
              weight =
                congestion > 100
                  ? 5
                  : congestion > 70
                    ? 4
                    : congestion > 50
                      ? 3
                      : 2;
              opacity = 0.9;
            } else {
              color = "#d1d5db"; // lighter gray for roads not in signal response
              weight = 1;
              opacity = 0.3;
            }
          }

          return (
            <Polyline
              key={`${i}-${algorithm}-${maintenanceResponse?.data?.selectedRoadCount ?? 0}-${highlightSelected ? 1 : 0}-${signalResponse?.data.summary.optimizedIntersections ?? 0}`}
              positions={pos}
              pathOptions={{ color, weight, opacity }}
              eventHandlers={{
                click: () => handleRoadClick(edge),
              }}
            />
          );
        })}

        {/* MST */}
        {showMst &&
          mstPositions.map((pos, i) => (
            <Polyline
              key={`mst-${i}`}
              positions={pos}
              color="#16a34a"
              weight={4}
              opacity={0.9}
            />
          ))}

        {/* Shortest Path */}
        {pathPositions.length > 1 && (
          <Polyline
            positions={pathPositions}
            color="#3b82f6"
            weight={5}
            opacity={0.9}
          />
        )}

        {/* Signal Intersection Markers (when signals algorithm active) */}
        {algorithm === "signals" &&
          intersectionSignals &&
          Object.values(intersectionSignals).map((intersection) => {
            if (!intersection.nodeId) return null;
            const node = nodeLookup[intersection.nodeId];
            if (!node) return null;

            const color = getIntersectionSeverityColor(
              intersection.maxCongestion,
            );
            const size = 12 + intersection.signals.length * 2; // Larger for more signals

            return (
              <Marker
                key={`signal-${intersection.nodeId}`}
                position={[node.y, node.x]}
                icon={
                  leaflet
                    ? // eslint-disable-next-line @typescript-eslint/no-explicit-any
                      (leaflet as any).divIcon({
                        className: "custom-signal-marker",
                        html: `<div style="background-color:${color};width:${size}px;height:${size}px;border-radius:50%;border:3px solid white;box-shadow:0 0 0 2px ${color};"></div>`,
                        iconSize: [size, size],
                        iconAnchor: [size / 2, size / 2],
                      })
                    : undefined
                }
                eventHandlers={{
                  click: () => {
                    setSelectedNode({
                      ...node,
                      signalData: intersection,
                    } as Node & { signalData: typeof intersection });
                    setSelectedRoad(null);
                  },
                }}
              >
                <Popup>
                  <div className="text-sm">
                    <p className="font-semibold">
                      {intersection.intersectionName}
                    </p>
                    <p className="text-xs text-gray-600">
                      {intersection.signals.length} roads need optimization
                    </p>
                    <p className="text-xs text-gray-600">
                      Max congestion:{" "}
                      {(intersection.maxCongestion * 100).toFixed(0)}%
                    </p>
                  </div>
                </Popup>
              </Marker>
            );
          })}

        {/* Nodes */}
        {nodes.map((node) => (
          <Marker
            key={node.id}
            position={[node.y, node.x]}
            icon={getMarkerIcon(node.id)}
            eventHandlers={{
              click: () => handleMarkerClick(node.id),
            }}
          >
            <Popup>
              <p className="font-medium">{node.name}</p>
            </Popup>
          </Marker>
        ))}
      </MapContainer>
    </div>
  );
}

// Disable SSR (CRITICAL for Leaflet)
export default dynamic(() => Promise.resolve(MapInner), {
  ssr: false,
  loading: () => <div className="h-full w-full bg-gray-100" />,
});
