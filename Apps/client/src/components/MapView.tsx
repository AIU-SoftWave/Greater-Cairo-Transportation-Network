"use client";

import { useEffect, useMemo, useState } from "react";
import dynamic from "next/dynamic";

import type {
  Node,
  Road,
  ShortestPathNodeDto,
  AlgorithmResponse,
  ShortestPathResultDto,
} from "@/types";
import {
  getShortestPath,
  getTimeVaryingShortestPath,
} from "@/services/routes/routePlanning";
import { getEmergencyRoute } from "@/services/routes/emergencyRouting";

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

type AlgorithmType = "dijkstra" | "astar" | "time-varying";

const PERIODS = ["morning", "evening", "night"];

function MapInner({ nodes, edges }: MapViewProps) {
  const [startId, setStartId] = useState<string | null>(null);
  const [endId, setEndId] = useState<string | null>(null);
  const [pathNodes, setPathNodes] = useState<ShortestPathNodeDto[]>([]);
  const [pathDistance, setPathDistance] = useState<number | null>(null);
  const [algorithm, setAlgorithm] = useState<AlgorithmType>("dijkstra");
  const [period, setPeriod] = useState<string>("morning");
  const [response, setResponse] =
    useState<AlgorithmResponse<ShortestPathResultDto> | null>(null);
  const [loading, setLoading] = useState(false);
  const [leaflet, setLeaflet] = useState<unknown | null>(null);

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

  // Icons (IMPORTANT: always return a valid icon)
  const icons = useMemo(() => {
    if (!leaflet) return null;

    // eslint-disable-next-line @typescript-eslint/no-explicit-any
    const L = leaflet as any;
    return {
      default: new L.Icon.Default(),

      green: L.divIcon({
        className: "custom-marker",
        html: `<div style="background-color:#22c55e;width:20px;height:20px;border-radius:50%;border:2px solid white;"></div>`,
        iconSize: [20, 20],
        iconAnchor: [10, 10],
      }),

      red: L.divIcon({
        className: "custom-marker",
        html: `<div style="background-color:#ef4444;width:20px;height:20px;border-radius:50%;border:2px solid white;"></div>`,
        iconSize: [20, 20],
        iconAnchor: [10, 10],
      }),
    };
  }, [leaflet]);

  // Fetch shortest path based on selected algorithm
  useEffect(() => {
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

  // Marker click logic
  const handleMarkerClick = (id: string) => {
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
  };

  // Lookup for nodes
  const nodeLookup = useMemo(() => {
    const map: Record<string, Node> = {};
    nodes.forEach((n) => (map[n.id] = n));
    return map;
  }, [nodes]);

  // Draw all roads
  const edgePositions = useMemo(() => {
    return edges
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

  const getMarkerIcon = (id: string) => {
    if (!icons) return undefined;
    if (id === startId) return icons.green;
    if (id === endId) return icons.red;
    return icons.default;
  };

  const statusText = !startId
    ? "Click a location to set start"
    : !endId
      ? "Click a location to set destination"
      : loading
        ? "Calculating..."
        : pathNodes.length > 0 && pathDistance !== null
          ? `Path: ${pathDistance.toFixed(1)} km`
          : "No path found";
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
          <div className="flex gap-2">
            {[
              { key: "dijkstra", label: "Dijkstra" },
              { key: "astar", label: "A*" },
              { key: "time-varying", label: "Time-Varying" },
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

        {/* Status */}
        <p className="mb-3 text-sm font-medium text-gray-800">{statusText}</p>
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
          url="https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png"
          attribution="&copy; OpenStreetMap contributors"
        />

        {/* Roads */}
        {edgePositions.map((pos, i) => (
          <Polyline
            key={i}
            positions={pos}
            color="#9ca3af"
            weight={2}
            opacity={0.6}
          />
        ))}

        {/* Shortest Path */}
        {pathPositions.length > 1 && (
          <Polyline
            positions={pathPositions}
            color="#16a34a"
            weight={5}
            opacity={0.9}
          />
        )}

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
              <p className="text-xs text-gray-500">{node.type}</p>
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
