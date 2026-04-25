"use client";

import dynamic from "next/dynamic";
import { useState, useEffect } from "react";

import ControlPanel from "./controls/ControlPanel";
import ResultsPanel from "./results/ResultsPanel";

import { fetchNetworkTopology } from "@/services/network/networkTopology";
import { getAllLocations } from "@/services/locations/cityLocations";
import { getShortestPath, getTimeVaryingShortestPath } from "@/services/routes/routePlanning";
import { getEmergencyRoute } from "@/services/routes/emergencyRouting";
import { getCheapestNetwork } from "@/services/network/networkExpansion";
import { getMaintenancePlan } from "@/services/maintenance/maintenanceStrategy";
import { getTransitSchedule } from "@/services/transit/transitOperations";
import { getSignalOptimization } from "@/services/traffic/signalOptimization";

import type { NetworkTopologyData, Location, AlgorithmResponse } from "@/types";

// ── types ──────────────────────────────────────────────────────────────────

export type AlgorithmId =
  | "dijkstra"
  | "time-dijkstra"
  | "astar"
  | "mst"
  | "maintenance"
  | "transit"
  | "signal";

export interface RunResult {
  algorithmId: AlgorithmId;
  inputs: {
    from?: string;
    to?: string;
    period?: string;
    budget?: number;
    vehicles?: number;
  };
  response: AlgorithmResponse<unknown>;
  timestamp: Date;
}

export interface SimulationComparison {
  algorithmId: AlgorithmId;
  inputs: { from?: string; to?: string; budget?: number; vehicles?: number };
  periodA: string;
  resultA: AlgorithmResponse<unknown>;
  periodB: string;
  resultB: AlgorithmResponse<unknown>;
  timestamp: Date;
}

// ── dynamic map import (no SSR — Leaflet requires browser APIs) ───────────

const MapClient = dynamic(() => import("./map/MapClient"), {
  ssr: false,
  loading: () => (
    <div className="flex h-full items-center justify-center bg-gray-100 text-gray-500">
      Loading map…
    </div>
  ),
});

// ── helpers ────────────────────────────────────────────────────────────────

async function executeAlgorithm(
  algorithmId: AlgorithmId,
  inputs: {
    from: string;
    to: string;
    period: string;
    budget: number;
    vehicles: number;
    topN: number;
    analyzeAll: boolean;
  },
): Promise<AlgorithmResponse<unknown>> {
  switch (algorithmId) {
    case "dijkstra":
      return getShortestPath(inputs.from, inputs.to) as Promise<AlgorithmResponse<unknown>>;
    case "time-dijkstra":
      return getTimeVaryingShortestPath(
        inputs.from,
        inputs.to,
        inputs.period,
      ) as Promise<AlgorithmResponse<unknown>>;
    case "astar":
      return getEmergencyRoute(inputs.from, inputs.to) as Promise<AlgorithmResponse<unknown>>;
    case "mst":
      return getCheapestNetwork() as Promise<AlgorithmResponse<unknown>>;
    case "maintenance":
      return getMaintenancePlan(inputs.budget) as Promise<AlgorithmResponse<unknown>>;
    case "transit":
      return getTransitSchedule(inputs.vehicles) as Promise<AlgorithmResponse<unknown>>;
    case "signal":
      return getSignalOptimization(
        inputs.period,
        inputs.topN,
        inputs.analyzeAll,
      ) as Promise<AlgorithmResponse<unknown>>;
  }
}

// ── main component ─────────────────────────────────────────────────────────

export default function TransportDashboard() {
  // data
  const [topology, setTopology] = useState<NetworkTopologyData | null>(null);
  const [locations, setLocations] = useState<Location[]>([]);
  const [dataError, setDataError] = useState<string | null>(null);

  // form state
  const [algorithmId, setAlgorithmId] = useState<AlgorithmId>("dijkstra");
  const [fromNode, setFromNode] = useState("");
  const [toNode, setToNode] = useState("");
  const [period, setPeriod] = useState("MORNING");
  const [budget, setBudget] = useState(500000);
  const [vehicles, setVehicles] = useState(100);
  const [topN, setTopN] = useState(10);
  const [analyzeAll, setAnalyzeAll] = useState(false);

  // runtime state
  const [loading, setLoading] = useState(false);
  const [runError, setRunError] = useState<string | null>(null);
  const [result, setResult] = useState<RunResult | null>(null);

  // simulation
  const [simulationMode, setSimulationMode] = useState(false);
  const [simPeriodA, setSimPeriodA] = useState("MORNING");
  const [simPeriodB, setSimPeriodB] = useState("EVENING");
  const [simComparisons, setSimComparisons] = useState<SimulationComparison[]>([]);

  // load graph data on mount
  useEffect(() => {
    let cancelled = false;

    async function loadData() {
      try {
        const [topo, locs] = await Promise.all([fetchNetworkTopology(), getAllLocations()]);
        if (cancelled) return;
        setTopology(topo);
        setLocations(locs);
        if (locs.length > 0) setFromNode(locs[0].id);
        if (locs.length > 1) setToNode(locs[1].id);
      } catch (err) {
        if (cancelled) return;
        setDataError(
          err instanceof Error
            ? err.message
            : "Failed to load network data. Is the server running?",
        );
      }
    }

    void loadData();
    return () => {
      cancelled = true;
    };
  }, []);

  // ── run single algorithm ──────────────────────────────────────────────────

  async function handleRun() {
    setRunError(null);
    setLoading(true);
    try {
      const response = await executeAlgorithm(algorithmId, {
        from: fromNode,
        to: toNode,
        period,
        budget,
        vehicles,
        topN,
        analyzeAll,
      });
      setResult({
        algorithmId,
        inputs: { from: fromNode, to: toNode, period, budget, vehicles },
        response,
        timestamp: new Date(),
      });
    } catch (err) {
      setRunError(
        err instanceof Error ? err.message : "An unexpected error occurred.",
      );
    } finally {
      setLoading(false);
    }
  }

  // ── run simulation comparison ─────────────────────────────────────────────

  async function handleRunSimulation() {
    setRunError(null);
    setLoading(true);
    try {
      const [resA, resB] = await Promise.all([
        executeAlgorithm(algorithmId, {
          from: fromNode,
          to: toNode,
          period: simPeriodA,
          budget,
          vehicles,
          topN,
          analyzeAll,
        }),
        executeAlgorithm(algorithmId, {
          from: fromNode,
          to: toNode,
          period: simPeriodB,
          budget,
          vehicles,
          topN,
          analyzeAll,
        }),
      ]);
      const comparison: SimulationComparison = {
        algorithmId,
        inputs: { from: fromNode, to: toNode, budget, vehicles },
        periodA: simPeriodA,
        resultA: resA,
        periodB: simPeriodB,
        resultB: resB,
        timestamp: new Date(),
      };
      setSimComparisons((prev) => [...prev, comparison]);
      // also update the main result display with period A
      setResult({
        algorithmId,
        inputs: { from: fromNode, to: toNode, period: simPeriodA, budget, vehicles },
        response: resA,
        timestamp: new Date(),
      });
    } catch (err) {
      setRunError(
        err instanceof Error ? err.message : "Simulation failed.",
      );
    } finally {
      setLoading(false);
    }
  }

  // ── render ────────────────────────────────────────────────────────────────

  return (
    <div className="flex h-screen overflow-hidden bg-gray-50">
      {/* ── sidebar ── */}
      <aside className="flex w-80 min-w-72 flex-col overflow-y-auto border-r border-gray-200 bg-white shadow-sm">
        {/* brand header */}
        <div className="border-b border-gray-200 bg-gradient-to-r from-blue-600 to-blue-700 px-4 py-4">
          <h1 className="text-sm font-bold text-white">🗺 Cairo Transportation</h1>
          <p className="text-xs text-blue-200">Network Analysis System</p>
        </div>

        {/* data error banner */}
        {dataError && (
          <div className="border-b border-orange-200 bg-orange-50 px-4 py-3 text-xs text-orange-800">
            ⚠ {dataError}
          </div>
        )}

        <div className="flex-1 space-y-4 p-4">
          <ControlPanel
            locations={locations}
            algorithmId={algorithmId}
            setAlgorithmId={setAlgorithmId}
            fromNode={fromNode}
            setFromNode={setFromNode}
            toNode={toNode}
            setToNode={setToNode}
            period={period}
            setPeriod={setPeriod}
            budget={budget}
            setBudget={setBudget}
            vehicles={vehicles}
            setVehicles={setVehicles}
            topN={topN}
            setTopN={setTopN}
            analyzeAll={analyzeAll}
            setAnalyzeAll={setAnalyzeAll}
            loading={loading}
            error={runError}
            onRun={handleRun}
            simulationMode={simulationMode}
            setSimulationMode={setSimulationMode}
            simComparisons={simComparisons}
            simPeriodA={simPeriodA}
            setSimPeriodA={setSimPeriodA}
            simPeriodB={simPeriodB}
            setSimPeriodB={setSimPeriodB}
            onRunSimulation={handleRunSimulation}
          />

          <hr className="border-gray-200" />

          <ResultsPanel
            result={result}
            loading={loading}
            simulationMode={simulationMode}
            simComparisons={simComparisons}
          />
        </div>
      </aside>

      {/* ── map area ── */}
      <main className="flex-1 relative">
        {!topology && !dataError && (
          <div className="absolute inset-0 flex items-center justify-center bg-gray-100 z-10">
            <div className="text-center">
              <div className="mb-2 text-4xl animate-spin">⏳</div>
              <p className="text-sm text-gray-600">Loading network data…</p>
            </div>
          </div>
        )}
        <MapClient topology={topology} result={result} />
      </main>
    </div>
  );
}
