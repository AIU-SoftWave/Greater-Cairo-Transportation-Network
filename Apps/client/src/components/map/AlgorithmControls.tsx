"use client";

import { PERIODS, type AlgorithmType } from "./types";

interface AlgorithmControlsProps {
  algorithm: AlgorithmType;
  period: string;
  onPeriodChange: (period: string) => void;
  budget: number;
  onBudgetChange: (budget: number) => void;
  topN: number;
  onTopNChange: (topN: number) => void;
  vehicles: number;
  onVehiclesChange: (vehicles: number) => void;
  weather: number;
  onWeatherChange: (weather: number) => void;
  loading: boolean;
  onCalculateMaintenance: () => void;
  onResetMaintenance: () => void;
  onCalculateSignals: () => void;
  onResetSignals: () => void;
  onCalculateTransit: () => void;
  onResetTransit: () => void;
  onResetSimulation: () => void;
  onRefreshMetrics: () => void;
}

export default function AlgorithmControls({
  algorithm,
  period,
  onPeriodChange,
  budget,
  onBudgetChange,
  topN,
  onTopNChange,
  vehicles,
  onVehiclesChange,
  weather,
  onWeatherChange,
  loading,
  onCalculateMaintenance,
  onResetMaintenance,
  onCalculateSignals,
  onResetSignals,
  onCalculateTransit,
  onResetTransit,
  onResetSimulation,
  onRefreshMetrics,
}: AlgorithmControlsProps) {
  return (
    <>
      {/* Period Selection (only for time-varying) */}
      {algorithm === "time-varying" && (
        <div className="mb-4">
          <p className="mb-2 text-xs font-semibold uppercase text-gray-500">
            Period
          </p>
          <select
            value={period}
            onChange={(e) => onPeriodChange(e.target.value)}
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
            onChange={(e) => onBudgetChange(Number(e.target.value))}
            className="w-full rounded border border-gray-300 px-2 py-1 text-sm text-black"
          />
          <div className="mt-2 flex gap-2">
            <button
              onClick={onCalculateMaintenance}
              className="flex-1 rounded bg-orange-500 px-2 py-2 text-xs font-medium text-white hover:bg-orange-600"
              disabled={loading}
            >
              Calculate Plan
            </button>
            <button
              onClick={onResetMaintenance}
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
            onChange={(e) => onPeriodChange(e.target.value)}
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
            onChange={(e) => onTopNChange(Number(e.target.value))}
            className="w-full rounded border border-gray-300 px-2 py-1 text-sm text-black"
          />
          <div className="mt-2 flex gap-2">
            <button
              onClick={onCalculateSignals}
              className="flex-1 rounded bg-blue-500 px-2 py-2 text-xs font-medium text-white hover:bg-blue-600"
              disabled={loading}
            >
              Calculate
            </button>
            <button
              onClick={onResetSignals}
              className="flex-1 rounded bg-gray-100 px-2 py-2 text-xs font-medium text-gray-700 hover:bg-gray-200"
              disabled={loading}
            >
              Reset
            </button>
          </div>
        </div>
      )}

      {/* Transit Scheduling (only for transit) */}
      {algorithm === "transit" && (
        <div className="mb-4">
          <p className="mb-2 text-xs font-semibold uppercase text-gray-500">
            Vehicles
          </p>
          <input
            type="number"
            min="1"
            max="1000"
            step="1"
            value={vehicles}
            onChange={(e) => onVehiclesChange(Number(e.target.value))}
            className="w-full rounded border border-gray-300 px-2 py-1 text-sm text-black"
          />
          <div className="mt-2 flex gap-2">
            <button
              onClick={onCalculateTransit}
              className="flex-1 rounded bg-purple-500 px-2 py-2 text-xs font-medium text-white hover:bg-purple-600"
              disabled={loading}
            >
              Calculate
            </button>
            <button
              onClick={onResetTransit}
              className="flex-1 rounded bg-gray-100 px-2 py-2 text-xs font-medium text-gray-700 hover:bg-gray-200"
              disabled={loading}
            >
              Reset
            </button>
          </div>
        </div>
      )}

      {/* Simulation Panel (only for simulation) */}
      {algorithm === "simulation" && (
        <div className="mb-4">
          <p className="mb-2 text-xs font-semibold uppercase text-gray-500">
            Simulation Controls
          </p>
          <div className="space-y-2">
            <button
              onClick={onResetSimulation}
              className="w-full rounded bg-red-500 px-2 py-2 text-xs font-medium text-white hover:bg-red-600"
              disabled={loading}
            >
              Reset All Closures
            </button>
            <button
              onClick={onRefreshMetrics}
              className="w-full rounded bg-emerald-500 px-2 py-2 text-xs font-medium text-white hover:bg-emerald-600"
              disabled={loading}
            >
              Performance Metrics
            </button>
            <div className="mt-2">
              <p className="mb-1 text-[10px] font-semibold text-gray-500 uppercase">
                Weather Condition
              </p>
              <select
                value={weather}
                onChange={(e) => onWeatherChange(parseInt(e.target.value))}
                className="w-full rounded border border-gray-300 px-2 py-1 text-xs"
              >
                <option value={0}>Clear Sky</option>
                <option value={1}>Heavy Rain (1.3x delay)</option>
                <option value={2}>Severe Storm (1.8x delay)</option>
              </select>
            </div>
            <p className="text-[10px] text-gray-500 italic">
              * Click any road on the map to simulate an accident (close road).
            </p>
          </div>
        </div>
      )}
    </>
  );
}
