"use client";

import type {
  AlgorithmResponse,
  ShortestPathResultDto,
  MstResultDto,
  MaintenancePlanningResultDto,
  TransitSchedulingResultDto,
  TrafficSignalResultDto,
} from "@/types";
import type { AlgorithmId, RunResult, SimulationComparison } from "../TransportDashboard";

// ── small stat card ──────────────────────────────────────────────────────────

function StatCard({ label, value }: { label: string; value: string | number }) {
  return (
    <div className="rounded-md border border-gray-200 bg-gray-50 px-3 py-2">
      <p className="text-xs text-gray-500">{label}</p>
      <p className="text-sm font-semibold text-gray-800">{value}</p>
    </div>
  );
}

// ── path results ─────────────────────────────────────────────────────────────

function PathResults({ data }: { data: ShortestPathResultDto }) {
  if (!data.found) {
    return (
      <div className="rounded-md border border-red-200 bg-red-50 px-3 py-2 text-sm text-red-700">
        No path found between the selected nodes.
      </div>
    );
  }
  return (
    <div className="space-y-3">
      <div className="grid grid-cols-2 gap-2">
        <StatCard label="Total Distance" value={`${data.totalDistance.toFixed(2)} km`} />
        <StatCard label="Nodes in Path" value={data.pathNodes.length} />
      </div>
      <div>
        <p className="mb-1 text-xs font-semibold text-gray-600">Path</p>
        <div className="flex flex-wrap gap-1">
          {data.pathNodes.map((node, idx) => (
            <span key={node.id} className="flex items-center gap-1">
              <span className="rounded bg-blue-100 px-2 py-0.5 text-xs font-medium text-blue-800">
                {node.name}
              </span>
              {idx < data.pathNodes.length - 1 && (
                <span className="text-gray-400 text-xs">→</span>
              )}
            </span>
          ))}
        </div>
      </div>
      {data.pathRoads.length > 0 && (
        <div>
          <p className="mb-1 text-xs font-semibold text-gray-600">Road Segments</p>
          <div className="max-h-32 overflow-y-auto rounded border border-gray-200 bg-white text-xs">
            {data.pathRoads.map((road, idx) => (
              <div key={idx} className="flex justify-between border-b border-gray-100 px-2 py-1 last:border-0">
                <span className="text-gray-600">
                  {road.fromNodeId} → {road.toNodeId}
                </span>
                <span className="font-medium text-gray-800">{road.distance.toFixed(1)} km</span>
              </div>
            ))}
          </div>
        </div>
      )}
    </div>
  );
}

// ── mst results ──────────────────────────────────────────────────────────────

function MstResults({ data }: { data: MstResultDto }) {
  return (
    <div className="space-y-3">
      <div className="grid grid-cols-2 gap-2">
        <StatCard
          label="Connected"
          value={data.connected ? "✅ Yes" : "❌ No"}
        />
        <StatCard label="Total Nodes" value={data.totalNodes} />
        <StatCard
          label="Construction Cost"
          value={`EGP ${data.totalConstructionCost.toLocaleString()}`}
        />
        <StatCard label="Roads Selected" value={data.selectedRoadCount} />
      </div>
      <div>
        <p className="mb-1 text-xs font-semibold text-gray-600">Selected Roads</p>
        <div className="max-h-32 overflow-y-auto rounded border border-gray-200 bg-white text-xs">
          {data.selectedRoads.map((road, idx) => (
            <div
              key={idx}
              className="flex justify-between border-b border-gray-100 px-2 py-1 last:border-0"
            >
              <span className="text-gray-600">
                {road.fromNodeId} → {road.toNodeId}
              </span>
              <span className="font-medium text-gray-800">
                {road.constructionCost != null
                  ? `EGP ${road.constructionCost.toLocaleString()}`
                  : `${road.distance.toFixed(1)} km`}
              </span>
            </div>
          ))}
        </div>
      </div>
    </div>
  );
}

// ── maintenance results ──────────────────────────────────────────────────────

function MaintenanceResults({ data }: { data: MaintenancePlanningResultDto }) {
  return (
    <div className="space-y-3">
      <div className="grid grid-cols-2 gap-2">
        <StatCard label="Budget" value={`EGP ${data.budget.toLocaleString()}`} />
        <StatCard label="Total Cost" value={`EGP ${data.totalCost.toLocaleString()}`} />
        <StatCard label="Remaining" value={`EGP ${data.remainingBudget.toLocaleString()}`} />
        <StatCard label="Roads Selected" value={`${data.selectedRoadCount} / ${data.totalCandidateRoads}`} />
        <StatCard
          label="Condition Improvement"
          value={`+${data.expectedConditionImprovement.toFixed(1)}`}
        />
        <StatCard label="Priority Score" value={data.totalPriorityScore.toFixed(1)} />
      </div>
      <div>
        <p className="mb-1 text-xs font-semibold text-gray-600">Selected Roads</p>
        <div className="max-h-40 overflow-y-auto rounded border border-gray-200 bg-white text-xs">
          {data.selectedRoads.map((road, idx) => (
            <div key={idx} className="border-b border-gray-100 px-2 py-1 last:border-0">
              <div className="flex justify-between">
                <span className="font-medium text-gray-700">
                  {road.fromLocation} → {road.toLocation}
                </span>
                <span className="text-gray-500">
                  {road.estimatedCost != null ? `EGP ${road.estimatedCost.toLocaleString()}` : "—"}
                </span>
              </div>
              <div className="text-gray-500">{road.reason}</div>
            </div>
          ))}
        </div>
      </div>
    </div>
  );
}

// ── transit results ──────────────────────────────────────────────────────────

function TransitResults({ data }: { data: TransitSchedulingResultDto }) {
  return (
    <div className="space-y-3">
      <div className="grid grid-cols-2 gap-2">
        <StatCard label="Total Vehicles" value={data.totalVehicles} />
        <StatCard label="Assigned" value={data.assignedVehicles} />
        <StatCard label="Active Routes" value={`${data.activeRoutes} / ${data.totalRoutes}`} />
        <StatCard label="Coverage" value={`${(data.coverageRatio * 100).toFixed(1)}%`} />
        <StatCard label="Est. Passengers Served" value={data.estimatedPassengersServed.toLocaleString()} />
        <StatCard label="Total Demand" value={data.totalDemand.toLocaleString()} />
      </div>
      <div>
        <p className="mb-1 text-xs font-semibold text-gray-600">Route Allocations</p>
        <div className="max-h-40 overflow-y-auto rounded border border-gray-200 bg-white text-xs">
          {data.routeAllocations.map((alloc, idx) => (
            <div key={idx} className="border-b border-gray-100 px-2 py-1 last:border-0">
              <div className="flex justify-between">
                <span className="font-medium text-gray-700">
                  {alloc.routeId} ({alloc.routeType})
                </span>
                <span className="text-gray-600">{alloc.assignedVehicles} vehicles</span>
              </div>
              <div className="text-gray-500">
                Est. served: {alloc.estimatedServed.toLocaleString()} · Eff:{" "}
                {alloc.efficiencyScore.toFixed(2)}
              </div>
            </div>
          ))}
        </div>
      </div>
    </div>
  );
}

// ── signal results ────────────────────────────────────────────────────────────

function SignalResults({ data }: { data: TrafficSignalResultDto }) {
  return (
    <div className="space-y-3">
      <div className="grid grid-cols-2 gap-2">
        <StatCard label="Period" value={data.period} />
        <StatCard label="Roads Analyzed" value={data.roadsAnalyzed} />
        <StatCard label="Intersections" value={data.intersectionsAnalyzed} />
        <StatCard label="Signal Recommendations" value={data.signalRecommendations} />
        <StatCard label="Congestion Score" value={data.totalCongestionScore.toFixed(1)} />
        <StatCard
          label="Wait Time Reduction"
          value={`${data.estimatedWaitTimeReductionPercent.toFixed(1)}%`}
        />
      </div>
      <div>
        <p className="mb-1 text-xs font-semibold text-gray-600">Signal Timings</p>
        <div className="max-h-40 overflow-y-auto rounded border border-gray-200 bg-white text-xs">
          {data.signalTimings.map((sig, idx) => (
            <div key={idx} className="border-b border-gray-100 px-2 py-1 last:border-0">
              <div className="flex justify-between">
                <span className="font-medium text-gray-700">
                  {sig.fromLocation} → {sig.toLocation}
                </span>
                <span className="text-gray-500">Rank #{sig.priorityRank}</span>
              </div>
              <div className="text-gray-500">
                Green: {sig.recommendedGreenDurationSeconds}s · Cycle:{" "}
                {sig.recommendedCycleTimeSeconds}s · Congestion:{" "}
                {(sig.congestionRatio * 100).toFixed(0)}%
              </div>
            </div>
          ))}
        </div>
      </div>
    </div>
  );
}

// ── trace section ────────────────────────────────────────────────────────────

function TraceSection({ result }: { result: RunResult }) {
  const { trace } = result.response;
  return (
    <div className="grid grid-cols-3 gap-2">
      <StatCard label="Visited Nodes" value={trace.visitedNodes} />
      <StatCard label="Expanded Nodes" value={trace.expandedNodes} />
      <StatCard label="Time" value={`${trace.executionTimeMs.toFixed(2)} ms`} />
    </div>
  );
}

// ── algorithm data renderer ──────────────────────────────────────────────────

function AlgorithmData({
  algorithmId,
  response,
}: {
  algorithmId: AlgorithmId;
  response: AlgorithmResponse<unknown>;
}) {
  if (!response.success) {
    return (
      <div className="rounded-md border border-red-200 bg-red-50 px-3 py-2 text-sm text-red-700">
        {response.message}
      </div>
    );
  }
  const data = response.data;

  switch (algorithmId) {
    case "dijkstra":
    case "time-dijkstra":
    case "astar":
      return <PathResults data={data as ShortestPathResultDto} />;
    case "mst":
      return <MstResults data={data as MstResultDto} />;
    case "maintenance":
      return <MaintenanceResults data={data as MaintenancePlanningResultDto} />;
    case "transit":
      return <TransitResults data={data as TransitSchedulingResultDto} />;
    case "signal":
      return <SignalResults data={data as TrafficSignalResultDto} />;
    default:
      return null;
  }
}

// ── simulation comparison ─────────────────────────────────────────────────────

function SimulationComparisonCard({ comp }: { comp: SimulationComparison }) {
  const getDistance = (r: AlgorithmResponse<unknown>): string => {
    if (!r.success) return "—";
    const d = r.data as { totalDistance?: number } | null;
    return d?.totalDistance != null ? `${d.totalDistance.toFixed(2)} km` : "—";
  };
  return (
    <div className="rounded-lg border border-indigo-200 bg-indigo-50 p-3">
      <p className="text-xs font-semibold text-indigo-700 mb-2">
        {comp.algorithmId.toUpperCase()} comparison
      </p>
      <div className="grid grid-cols-2 gap-2">
        <div className="rounded bg-white border border-indigo-200 p-2">
          <p className="text-xs font-bold text-indigo-600">{comp.periodA}</p>
          <p className="text-xs text-gray-600">
            {comp.resultA.success
              ? `${getDistance(comp.resultA)} · ${comp.resultA.trace.executionTimeMs.toFixed(1)} ms`
              : comp.resultA.message}
          </p>
        </div>
        <div className="rounded bg-white border border-indigo-200 p-2">
          <p className="text-xs font-bold text-indigo-600">{comp.periodB}</p>
          <p className="text-xs text-gray-600">
            {comp.resultB.success
              ? `${getDistance(comp.resultB)} · ${comp.resultB.trace.executionTimeMs.toFixed(1)} ms`
              : comp.resultB.message}
          </p>
        </div>
      </div>
    </div>
  );
}

// ── main ResultsPanel ────────────────────────────────────────────────────────

interface ResultsPanelProps {
  result: RunResult | null;
  loading: boolean;
  simulationMode: boolean;
  simComparisons: SimulationComparison[];
}

export default function ResultsPanel({
  result,
  loading,
  simulationMode,
  simComparisons,
}: ResultsPanelProps) {
  if (loading) {
    return (
      <div className="flex h-24 items-center justify-center rounded-lg border border-gray-200 bg-gray-50">
        <div className="flex items-center gap-2 text-sm text-gray-500">
          <span className="animate-spin">⏳</span>
          Running algorithm…
        </div>
      </div>
    );
  }

  return (
    <div className="flex flex-col gap-4">
      <h2 className="text-base font-bold text-gray-800">Results</h2>

      {!result && !simulationMode && (
        <p className="text-sm text-gray-400">Run an algorithm to see results here.</p>
      )}

      {result && (
        <div className="rounded-lg border border-gray-200 bg-white p-4 shadow-sm">
          {/* header */}
          <div className="mb-3 flex items-center justify-between">
            <div>
              <p className="text-sm font-semibold text-gray-800">
                {result.response.algorithmName}
              </p>
              <p className="text-xs text-gray-500">
                {result.timestamp.toLocaleTimeString()}
              </p>
            </div>
            <span
              className={`rounded-full px-2 py-0.5 text-xs font-semibold ${
                result.response.success
                  ? "bg-green-100 text-green-700"
                  : "bg-red-100 text-red-700"
              }`}
            >
              {result.response.success ? "✓ Success" : "✗ Failed"}
            </span>
          </div>

          {/* message */}
          {result.response.message && (
            <p className="mb-3 text-xs text-gray-500">{result.response.message}</p>
          )}

          {/* trace */}
          <div className="mb-3">
            <p className="mb-1 text-xs font-semibold text-gray-600 uppercase tracking-wide">
              Debug Trace
            </p>
            <TraceSection result={result} />
          </div>

          {/* algorithm-specific data */}
          <div>
            <p className="mb-2 text-xs font-semibold text-gray-600 uppercase tracking-wide">
              Output
            </p>
            <AlgorithmData algorithmId={result.algorithmId} response={result.response} />
          </div>
        </div>
      )}

      {/* simulation comparisons */}
      {simulationMode && simComparisons.length > 0 && (
        <div className="space-y-3">
          <p className="text-xs font-semibold text-gray-600 uppercase tracking-wide">
            ⚡ Simulation History
          </p>
          {[...simComparisons].reverse().map((comp, idx) => (
            <SimulationComparisonCard key={idx} comp={comp} />
          ))}
        </div>
      )}
    </div>
  );
}
