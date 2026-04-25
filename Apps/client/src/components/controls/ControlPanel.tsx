"use client";

import type { Location } from "@/types";
import type { AlgorithmId, SimulationComparison } from "../TransportDashboard";

export const ALGORITHMS: Array<{
  id: AlgorithmId;
  name: string;
  hasFrom: boolean;
  hasTo: boolean;
  hasPeriod: boolean;
  hasBudget: boolean;
  hasVehicles: boolean;
  hasTopN: boolean;
}> = [
  {
    id: "dijkstra",
    name: "Dijkstra — Shortest Path",
    hasFrom: true,
    hasTo: true,
    hasPeriod: false,
    hasBudget: false,
    hasVehicles: false,
    hasTopN: false,
  },
  {
    id: "time-dijkstra",
    name: "Time-Varying Dijkstra",
    hasFrom: true,
    hasTo: true,
    hasPeriod: true,
    hasBudget: false,
    hasVehicles: false,
    hasTopN: false,
  },
  {
    id: "astar",
    name: "A* — Emergency Routing",
    hasFrom: true,
    hasTo: true,
    hasPeriod: false,
    hasBudget: false,
    hasVehicles: false,
    hasTopN: false,
  },
  {
    id: "mst",
    name: "MST — Network Expansion (Prim)",
    hasFrom: false,
    hasTo: false,
    hasPeriod: false,
    hasBudget: false,
    hasVehicles: false,
    hasTopN: false,
  },
  {
    id: "maintenance",
    name: "Maintenance Planning (DP)",
    hasFrom: false,
    hasTo: false,
    hasPeriod: false,
    hasBudget: true,
    hasVehicles: false,
    hasTopN: false,
  },
  {
    id: "transit",
    name: "Transit Scheduling (DP)",
    hasFrom: false,
    hasTo: false,
    hasPeriod: false,
    hasBudget: false,
    hasVehicles: true,
    hasTopN: false,
  },
  {
    id: "signal",
    name: "Signal Optimization (Greedy)",
    hasFrom: false,
    hasTo: false,
    hasPeriod: true,
    hasBudget: false,
    hasVehicles: false,
    hasTopN: true,
  },
];

export const TRAFFIC_PERIODS = ["MORNING", "AFTERNOON", "EVENING", "NIGHT", "WEEKEND"];

// ── Legend ──────────────────────────────────────────────────────────────────

function Legend() {
  return (
    <div className="mt-4 rounded-lg border border-gray-200 bg-white p-3">
      <p className="mb-2 text-xs font-semibold uppercase tracking-wide text-gray-500">Map Legend</p>
      <div className="space-y-1 text-xs">
        <div className="flex items-center gap-2">
          <span className="inline-block h-3 w-3 rounded-full bg-red-500" />
          <span>Start node</span>
        </div>
        <div className="flex items-center gap-2">
          <span className="inline-block h-3 w-3 rounded-full bg-violet-500" />
          <span>End node</span>
        </div>
        <div className="flex items-center gap-2">
          <span className="inline-block h-3 w-3 rounded-full bg-green-500" />
          <span>Path / MST node</span>
        </div>
        <div className="flex items-center gap-2">
          <span className="inline-block h-3 w-3 rounded-full bg-orange-500" />
          <span>Critical node</span>
        </div>
        <div className="flex items-center gap-2">
          <span className="inline-block h-3 w-3 rounded-full bg-blue-500" />
          <span>Normal node</span>
        </div>
        <div className="flex items-center gap-2">
          <span className="inline-block h-3 w-5 rounded bg-blue-600" />
          <span>Path road</span>
        </div>
        <div className="flex items-center gap-2">
          <span className="inline-block h-3 w-5 rounded bg-green-600" />
          <span>MST road</span>
        </div>
        <div className="flex items-center gap-2">
          <span className="inline-block h-3 w-5 rounded bg-gray-400" />
          <span>Normal road</span>
        </div>
      </div>
    </div>
  );
}

// ── ControlPanel ─────────────────────────────────────────────────────────────

interface ControlPanelProps {
  locations: Location[];
  algorithmId: AlgorithmId;
  setAlgorithmId: (id: AlgorithmId) => void;
  fromNode: string;
  setFromNode: (id: string) => void;
  toNode: string;
  setToNode: (id: string) => void;
  period: string;
  setPeriod: (p: string) => void;
  budget: number;
  setBudget: (b: number) => void;
  vehicles: number;
  setVehicles: (v: number) => void;
  topN: number;
  setTopN: (n: number) => void;
  analyzeAll: boolean;
  setAnalyzeAll: (v: boolean) => void;
  loading: boolean;
  error: string | null;
  onRun: () => void;
  simulationMode: boolean;
  setSimulationMode: (v: boolean) => void;
  simComparisons: SimulationComparison[];
  simPeriodA: string;
  setSimPeriodA: (p: string) => void;
  simPeriodB: string;
  setSimPeriodB: (p: string) => void;
  onRunSimulation: () => void;
}

export default function ControlPanel({
  locations,
  algorithmId,
  setAlgorithmId,
  fromNode,
  setFromNode,
  toNode,
  setToNode,
  period,
  setPeriod,
  budget,
  setBudget,
  vehicles,
  setVehicles,
  topN,
  setTopN,
  analyzeAll,
  setAnalyzeAll,
  loading,
  error,
  onRun,
  simulationMode,
  setSimulationMode,
  simComparisons,
  simPeriodA,
  setSimPeriodA,
  simPeriodB,
  setSimPeriodB,
  onRunSimulation,
}: ControlPanelProps) {
  const algo = ALGORITHMS.find((a) => a.id === algorithmId)!;

  const labelClass = "block text-xs font-semibold text-gray-600 mb-1";
  const inputClass =
    "w-full rounded-md border border-gray-300 bg-white px-3 py-2 text-sm text-gray-800 shadow-sm focus:border-blue-500 focus:outline-none focus:ring-1 focus:ring-blue-500";

  return (
    <div className="flex flex-col gap-4">
      {/* header */}
      <div className="flex items-center justify-between">
        <h2 className="text-base font-bold text-gray-800">Control Panel</h2>
        <button
          onClick={() => setSimulationMode(!simulationMode)}
          className={`rounded-full px-3 py-1 text-xs font-semibold transition-colors ${
            simulationMode
              ? "bg-indigo-600 text-white"
              : "border border-indigo-300 text-indigo-600 hover:bg-indigo-50"
          }`}
        >
          {simulationMode ? "⚡ Simulation" : "Simulation"}
        </button>
      </div>

      {/* algorithm selector */}
      <div>
        <label className={labelClass}>Algorithm</label>
        <select
          className={inputClass}
          value={algorithmId}
          onChange={(e) => setAlgorithmId(e.target.value as AlgorithmId)}
          disabled={loading}
        >
          {ALGORITHMS.map((a) => (
            <option key={a.id} value={a.id}>
              {a.name}
            </option>
          ))}
        </select>
      </div>

      {/* from node */}
      {algo.hasFrom && (
        <div>
          <label className={labelClass}>From Node</label>
          <select
            className={inputClass}
            value={fromNode}
            onChange={(e) => setFromNode(e.target.value)}
            disabled={loading}
          >
            <option value="">— select start —</option>
            {locations.map((loc) => (
              <option key={loc.id} value={loc.id}>
                {loc.name}
              </option>
            ))}
          </select>
        </div>
      )}

      {/* to node */}
      {algo.hasTo && (
        <div>
          <label className={labelClass}>To Node</label>
          <select
            className={inputClass}
            value={toNode}
            onChange={(e) => setToNode(e.target.value)}
            disabled={loading}
          >
            <option value="">— select destination —</option>
            {locations.map((loc) => (
              <option key={loc.id} value={loc.id}>
                {loc.name}
              </option>
            ))}
          </select>
        </div>
      )}

      {/* traffic period */}
      {algo.hasPeriod && (
        <div>
          <label className={labelClass}>Traffic Period</label>
          <select
            className={inputClass}
            value={period}
            onChange={(e) => setPeriod(e.target.value)}
            disabled={loading}
          >
            {TRAFFIC_PERIODS.map((p) => (
              <option key={p} value={p}>
                {p}
              </option>
            ))}
          </select>
        </div>
      )}

      {/* budget */}
      {algo.hasBudget && (
        <div>
          <label className={labelClass}>Budget (EGP)</label>
          <input
            type="number"
            className={inputClass}
            value={budget}
            min={0}
            step={1000}
            onChange={(e) => setBudget(Number(e.target.value))}
            disabled={loading}
          />
        </div>
      )}

      {/* vehicles */}
      {algo.hasVehicles && (
        <div>
          <label className={labelClass}>Available Vehicles</label>
          <input
            type="number"
            className={inputClass}
            value={vehicles}
            min={1}
            step={1}
            onChange={(e) => setVehicles(Number(e.target.value))}
            disabled={loading}
          />
        </div>
      )}

      {/* topN + analyzeAll */}
      {algo.hasTopN && (
        <div className="flex gap-3">
          <div className="flex-1">
            <label className={labelClass}>Top N Signals</label>
            <input
              type="number"
              className={inputClass}
              value={topN}
              min={1}
              max={50}
              onChange={(e) => setTopN(Number(e.target.value))}
              disabled={loading}
            />
          </div>
          <div className="flex flex-col justify-end pb-1">
            <label className="flex items-center gap-1 text-xs text-gray-600 cursor-pointer">
              <input
                type="checkbox"
                checked={analyzeAll}
                onChange={(e) => setAnalyzeAll(e.target.checked)}
                disabled={loading}
                className="rounded"
              />
              All intersections
            </label>
          </div>
        </div>
      )}

      {/* error */}
      {error && (
        <div className="rounded-md border border-red-200 bg-red-50 px-3 py-2 text-xs text-red-700">
          {error}
        </div>
      )}

      {/* run button */}
      <button
        onClick={onRun}
        disabled={loading}
        className="w-full rounded-md bg-blue-600 px-4 py-2 text-sm font-semibold text-white shadow-sm transition-colors hover:bg-blue-700 disabled:cursor-not-allowed disabled:opacity-50"
      >
        {loading ? "Running…" : "▶ Run Algorithm"}
      </button>

      {/* simulation section */}
      {simulationMode && (
        <div className="rounded-lg border border-indigo-200 bg-indigo-50 p-3">
          <p className="mb-2 text-xs font-semibold text-indigo-700 uppercase tracking-wide">
            ⚡ Simulation: Compare Periods
          </p>
          <p className="mb-3 text-xs text-indigo-600">
            Runs the current algorithm with two traffic periods and compares results.
          </p>
          <div className="flex gap-2 mb-2">
            <div className="flex-1">
              <label className="block text-xs font-semibold text-indigo-600 mb-1">Period A</label>
              <select
                className={inputClass}
                value={simPeriodA}
                onChange={(e) => setSimPeriodA(e.target.value)}
                disabled={loading}
              >
                {TRAFFIC_PERIODS.map((p) => (
                  <option key={p} value={p}>
                    {p}
                  </option>
                ))}
              </select>
            </div>
            <div className="flex-1">
              <label className="block text-xs font-semibold text-indigo-600 mb-1">Period B</label>
              <select
                className={inputClass}
                value={simPeriodB}
                onChange={(e) => setSimPeriodB(e.target.value)}
                disabled={loading}
              >
                {TRAFFIC_PERIODS.map((p) => (
                  <option key={p} value={p}>
                    {p}
                  </option>
                ))}
              </select>
            </div>
          </div>
          <button
            onClick={onRunSimulation}
            disabled={loading || !algo.hasPeriod}
            className="w-full rounded-md bg-indigo-600 px-4 py-2 text-xs font-semibold text-white shadow-sm transition-colors hover:bg-indigo-700 disabled:cursor-not-allowed disabled:opacity-50"
          >
            {loading ? "Running…" : "Compare Periods"}
          </button>
          {!algo.hasPeriod && (
            <p className="mt-1 text-xs text-indigo-500">
              Select an algorithm that supports traffic periods.
            </p>
          )}
          {simComparisons.length > 0 && (
            <p className="mt-2 text-xs text-indigo-600">
              {simComparisons.length} comparison(s) in history ↓
            </p>
          )}
        </div>
      )}

      <Legend />
    </div>
  );
}
