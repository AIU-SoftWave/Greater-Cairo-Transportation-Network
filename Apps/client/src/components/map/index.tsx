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
  TransitSchedulingResultDto,
} from "@/types";
import {
  getShortestPath,
  getTimeVaryingShortestPath,
} from "@/services/routes/routePlanning";
import { getEmergencyRoute } from "@/services/routes/emergencyRouting";
import { getCheapestNetwork } from "@/services/network/networkExpansion";
import { getMaintenancePlan } from "@/services/maintenance/maintenanceStrategy";
import { getSignalOptimization } from "@/services/traffic/signalOptimization";
import {
  getTransitSchedule,
  getRouteGeometry,
} from "@/services/transit/transitOperations";
import {
  toggleRoadClosure,
  resetSimulation,
  getClosedRoads,
  getMetrics,
  setWeather,
  PerformanceMetric,
} from "@/services/simulation";

import AlgorithmSelector from "./AlgorithmSelector";
import AlgorithmControls from "./AlgorithmControls";
import SelectedInfoPanel from "./SelectedInfoPanel";
import ResultsDashboard from "./ResultsDashboard";
import PerformanceMetricsModal from "./PerformanceMetricsModal";
import MapCanvas from "./MapCanvas";
import CompareResultsPanel from "./CompareResultsPanel";
import {
  COMPARE_ALGORITHMS,
  type AlgorithmType,
  type IntersectionSignal,
  type CompareAlgorithmType,
} from "./types";
import { isRoadSelectedForMaintenance as checkRoadSelectedForMaintenance } from "./utils";

interface MapViewProps {
  nodes: Node[];
  edges: Road[];
}

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
  const [transitResponse, setTransitResponse] =
    useState<AlgorithmResponse<TransitSchedulingResultDto> | null>(null);
  const [transitPathPositions, setTransitPathPositions] = useState<
    [number, number][]
  >([]);
  const [topN, setTopN] = useState<number>(10);
  const [vehicles, setVehicles] = useState<number>(50);
  const [loading, setLoading] = useState(false);
  const [mstEdges, setMstEdges] = useState<Road[]>([]);
  const [selectedRoad, setSelectedRoad] = useState<Road | null>(null);
  const [selectedNode, setSelectedNode] = useState<Node | null>(null);
  const [showMst, setShowMst] = useState(false);
  const [mstResponse, setMstResponse] =
    useState<AlgorithmResponse<MstResultDto> | null>(null);
  const [closedRoadIds, setClosedRoadIds] = useState<number[]>([]);
  const [metrics, setMetrics] = useState<PerformanceMetric[]>([]);
  const [showMetrics, setShowMetrics] = useState(false);
  const [weather, setWeatherState] = useState<number>(0);

  // Compare mode state
  const [compareAlgoA, setCompareAlgoA] =
    useState<CompareAlgorithmType>("dijkstra");
  const [compareAlgoB, setCompareAlgoB] =
    useState<CompareAlgorithmType>("astar");
  const [compareResponseA, setCompareResponseA] =
    useState<AlgorithmResponse<ShortestPathResultDto> | null>(null);
  const [compareResponseB, setCompareResponseB] =
    useState<AlgorithmResponse<ShortestPathResultDto> | null>(null);
  const [comparePathNodesA, setComparePathNodesA] = useState<
    ShortestPathNodeDto[]
  >([]);
  const [comparePathNodesB, setComparePathNodesB] = useState<
    ShortestPathNodeDto[]
  >([]);

  // Fetch MST and Simulation data on component load
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

    getClosedRoads().then(setClosedRoadIds);
  }, []);

  // Fetch shortest path based on selected algorithm
  useEffect(() => {
    if (algorithm === "maintenance" || algorithm === "compare") {
      return;
    }

    if (!startId || !endId) {
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
  }, [startId, endId, algorithm, period, weather]);

  // Fetch compare mode dual routes
  useEffect(() => {
    if (algorithm !== "compare") {
      return;
    }

    if (!startId || !endId) {
      setComparePathNodesA([]);
      setComparePathNodesB([]);
      setCompareResponseA(null);
      setCompareResponseB(null);
      return;
    }

    let cancelled = false;
    setLoading(true);

    const fetchCompareRoutes = async () => {
      try {
        // Fetch both algorithms in parallel
        const fetchAlgo = async (
          algo: CompareAlgorithmType,
        ): Promise<AlgorithmResponse<ShortestPathResultDto>> => {
          switch (algo) {
            case "astar":
              return await getEmergencyRoute(startId, endId);
            case "time-varying":
              return await getTimeVaryingShortestPath(startId, endId, period);
            case "dijkstra":
            default:
              return await getShortestPath(startId, endId);
          }
        };

        const [resA, resB] = await Promise.all([
          fetchAlgo(compareAlgoA),
          fetchAlgo(compareAlgoB),
        ]);

        if (cancelled) return;

        setCompareResponseA(resA);
        setCompareResponseB(resB);

        if (resA.success && resA.data.found) {
          setComparePathNodesA(resA.data.pathNodes);
        } else {
          setComparePathNodesA([]);
        }

        if (resB.success && resB.data.found) {
          setComparePathNodesB(resB.data.pathNodes);
        } else {
          setComparePathNodesB([]);
        }
      } catch {
        // Silently handle error
      } finally {
        if (!cancelled) {
          setLoading(false);
        }
      }
    };

    fetchCompareRoutes();

    return () => {
      cancelled = true;
    };
  }, [startId, endId, algorithm, compareAlgoA, compareAlgoB, period, weather]);

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

  const handleCalculateTransit = async () => {
    setLoading(true);
    try {
      const res = await getTransitSchedule(vehicles);
      setTransitResponse(res);
    } catch {
      // Silently handle error
    } finally {
      setLoading(false);
    }
  };

  const handleResetTransit = () => {
    setTransitResponse(null);
  };

  const handleToggleClosure = async (roadId: number) => {
    setLoading(true);
    try {
      await toggleRoadClosure(roadId);
      const closed = await getClosedRoads();
      setClosedRoadIds(closed);
      // Trigger re-fetch of current route if any
      if (startId && endId) {
        setAlgorithm((prev) => {
          // Temporarily change and restore to trigger useEffect
          setTimeout(() => setAlgorithm(prev), 10);
          return "dijkstra";
        });
      }
    } catch {
      // Ignore
    } finally {
      setLoading(false);
    }
  };

  const handleResetSimulation = async () => {
    setLoading(true);
    try {
      await resetSimulation();
      setClosedRoadIds([]);
      setAlgorithm("dijkstra");
      handleReset();
    } catch {
      // Ignore
    } finally {
      setLoading(false);
    }
  };

  const handleRefreshMetrics = async () => {
    const data = await getMetrics();
    setMetrics(data);
    setShowMetrics(true);
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
    if (algorithm === "simulation") {
      handleToggleClosure(Math.abs(edge.id));
    }
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

  const handleShowTransitRoute = async (routeId: string) => {
    try {
      const geometry = await getRouteGeometry(routeId);
      const points = geometry
        .filter((g) => g.x !== undefined && g.y !== undefined)
        .map((g) => [g.y!, g.x!] as [number, number]);
      setTransitPathPositions(points);
    } catch (e) {
      console.error(e);
    }
  };

  const handleWeatherChange = async (w: number) => {
    await setWeather(w);
    setWeatherState(w);
    // useEffect will auto-refresh route since weather is in dependencies
  };

  const handleSignalMarkerClick = (
    node: Node,
    signalData: IntersectionSignal,
  ) => {
    setSelectedNode({
      ...node,
      signalData,
    } as Node & { signalData: IntersectionSignal });
    setSelectedRoad(null);
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

        const isClosed = closedRoadIds.includes(Math.abs(edge.id));

        return {
          edge,
          isClosed,
          pos: [
            [from.y, from.x],
            [to.y, to.x],
          ] as [number, number][],
        };
      })
      .filter(Boolean) as {
      edge: Road;
      isClosed: boolean;
      pos: [number, number][];
    }[];
  }, [edges, nodeLookup, closedRoadIds]);

  // Draw shortest path
  const pathPositions = useMemo(() => {
    return pathNodes
      .filter(
        (n): n is ShortestPathNodeDto & { x: number; y: number } =>
          n.x !== undefined && n.y !== undefined,
      )
      .map((n) => [n.y, n.x] as [number, number]);
  }, [pathNodes]);

  // Draw compare paths (Algorithm A - Blue)
  const comparePathPositionsA = useMemo(() => {
    return comparePathNodesA
      .filter(
        (n): n is ShortestPathNodeDto & { x: number; y: number } =>
          n.x !== undefined && n.y !== undefined,
      )
      .map((n) => [n.y, n.x] as [number, number]);
  }, [comparePathNodesA]);

  // Draw compare paths (Algorithm B - Green)
  const comparePathPositionsB = useMemo(() => {
    return comparePathNodesB
      .filter(
        (n): n is ShortestPathNodeDto & { x: number; y: number } =>
          n.x !== undefined && n.y !== undefined,
      )
      .map((n) => [n.y, n.x] as [number, number]);
  }, [comparePathNodesB]);

  // Draw MST roads - separate existing and potential
  const mstRoadsData = useMemo(() => {
    return mstEdges
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
          isNewRoad: !edge.isExisting,
        };
      })
      .filter(Boolean) as {
      edge: Road;
      pos: [number, number][];
      isNewRoad: boolean;
    }[];
  }, [mstEdges, nodeLookup]);

  // Map intersection signals from API response with node lookup
  const intersectionSignals = useMemo((): Record<
    string,
    IntersectionSignal
  > | null => {
    if (!signalResponse?.success) return null;

    const groups: Record<string, IntersectionSignal> = {};

    for (const intersection of signalResponse.data.intersections) {
      const key = intersection.name;
      const nodeId = nodeIdByName[key.trim().toLowerCase()] ?? null;

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
        : algorithm === "transit"
          ? loading
            ? "Calculating transit schedule..."
            : transitResponse?.success
              ? `${transitResponse.data.activeRoutes} routes active, ${(transitResponse.data.coverageRatio * 100).toFixed(1)}% coverage`
              : "Enter vehicles and calculate"
          : !startId
            ? "Click a location to set start"
            : !endId
              ? "Click a location to set destination"
              : loading
                ? "Calculating..."
                : pathNodes.length > 0 && pathDistance !== null
                  ? `Path: ${pathDistance.toFixed(1)} km`
                  : "No path found";

  // Pre-compute matched maintenance roads count for ResultsDashboard
  const matchedMaintenanceRoadsCount = useMemo(() => {
    if (!maintenanceResponse) return 0;

    return edgeSegments.filter((segment) =>
      checkRoadSelectedForMaintenance(
        segment.edge,
        maintenanceResponse,
        nodeLookup,
        nodeIdByName,
      ),
    ).length;
  }, [maintenanceResponse, edgeSegments, nodeLookup, nodeIdByName]);

  return (
    <div className="relative h-full w-full">
      {/* Dashboard Panel */}
      <div className="absolute left-4 top-4 z-[1000] max-w-xs rounded-lg bg-white p-4 shadow-lg">
        <AlgorithmSelector
          algorithm={algorithm}
          onAlgorithmChange={handleAlgorithmChange}
        />

        <AlgorithmControls
          algorithm={algorithm}
          period={period}
          onPeriodChange={setPeriod}
          budget={budget}
          onBudgetChange={setBudget}
          topN={topN}
          onTopNChange={setTopN}
          vehicles={vehicles}
          onVehiclesChange={setVehicles}
          weather={weather}
          onWeatherChange={handleWeatherChange}
          loading={loading}
          onCalculateMaintenance={handleCalculateMaintenance}
          onResetMaintenance={handleResetMaintenance}
          onCalculateSignals={handleCalculateSignals}
          onResetSignals={handleResetSignals}
          onCalculateTransit={handleCalculateTransit}
          onResetTransit={handleResetTransit}
          onResetSimulation={handleResetSimulation}
          onRefreshMetrics={handleRefreshMetrics}
          compareAlgoA={compareAlgoA}
          compareAlgoB={compareAlgoB}
          onCompareAlgoAChange={setCompareAlgoA}
          onCompareAlgoBChange={setCompareAlgoB}
        />

        {/* Compare Results Panel */}
        {algorithm === "compare" && (
          <CompareResultsPanel
            responseA={compareResponseA}
            responseB={compareResponseB}
            algoA={
              COMPARE_ALGORITHMS.find((a) => a.key === compareAlgoA)?.label ||
              compareAlgoA
            }
            algoB={
              COMPARE_ALGORITHMS.find((a) => a.key === compareAlgoB)?.label ||
              compareAlgoB
            }
            colorA="#3b82f6"
            colorB="#22c55e"
          />
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

        <SelectedInfoPanel
          selectedNode={selectedNode}
          selectedRoad={selectedRoad}
          nodeLookup={nodeLookup}
        />

        <ResultsDashboard
          algorithm={algorithm}
          response={response}
          showMst={showMst}
          mstResponse={mstResponse}
          mstRoadsData={mstRoadsData}
          nodeLookup={nodeLookup}
          maintenanceResponse={maintenanceResponse}
          matchedMaintenanceRoadsCount={matchedMaintenanceRoadsCount}
          signalResponse={signalResponse}
          transitResponse={transitResponse}
          onShowTransitRoute={handleShowTransitRoute}
        />

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

      <MapCanvas
        nodes={nodes}
        nodeLookup={nodeLookup}
        nodeIdByName={nodeIdByName}
        edgeSegments={edgeSegments}
        pathPositions={pathPositions}
        mstRoadsData={mstRoadsData}
        transitPathPositions={transitPathPositions}
        intersectionSignals={intersectionSignals}
        algorithm={algorithm}
        showMst={showMst}
        startId={startId}
        endId={endId}
        maintenanceResponse={maintenanceResponse}
        signalResponse={signalResponse}
        onRoadClick={handleRoadClick}
        onMarkerClick={handleMarkerClick}
        onSignalMarkerClick={handleSignalMarkerClick}
        comparePathPositionsA={comparePathPositionsA}
        comparePathPositionsB={comparePathPositionsB}
      />

      <PerformanceMetricsModal
        show={showMetrics}
        metrics={metrics}
        onClose={() => setShowMetrics(false)}
      />
    </div>
  );
}

// Disable SSR (CRITICAL for Leaflet)
export default dynamic(() => Promise.resolve(MapInner), {
  ssr: false,
  loading: () => <div className="h-full w-full bg-gray-100" />,
});
