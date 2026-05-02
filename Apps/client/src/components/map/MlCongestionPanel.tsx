"use client";

import { useState } from "react";

export interface MlPrediction {
  road_id: number;
  period: string;
  predicted_congestion: number;
}

interface MlCongestionPanelProps {
  predictions: MlPrediction[];
  period: string;
  onPeriodChange: (period: string) => void;
  loading?: boolean;
  error?: string | null;
}

const PERIODS = [
  { key: "MORNING", label: "Morning" },
  { key: "EVENING", label: "Evening" },
  { key: "NIGHT", label: "Night" },
];

export default function MlCongestionPanel({
  predictions,
  period,
  onPeriodChange,
  loading,
  error,
}: MlCongestionPanelProps) {
  const [selectedPeriod, setSelectedPeriod] = useState(period);

  const handlePeriodChange = (newPeriod: string) => {
    setSelectedPeriod(newPeriod);
    onPeriodChange(newPeriod.toLowerCase());
  };

  const getCongestionColor = (congestion: number): string => {
    if (congestion >= 1.5) return "bg-red-500";
    if (congestion >= 1.0) return "bg-orange-500";
    if (congestion >= 0.7) return "bg-yellow-500";
    return "bg-green-500";
  };

  const getCongestionLabel = (congestion: number): string => {
    if (congestion >= 1.5) return "Very High";
    if (congestion >= 1.0) return "High";
    if (congestion >= 0.7) return "Moderate";
    return "Low";
  };

  const filteredPredictions = predictions.filter(
    (p) => p.period.toUpperCase() === selectedPeriod.toUpperCase()
  );

  const sortedPredictions = [...filteredPredictions].sort(
    (a, b) => b.predicted_congestion - a.predicted_congestion
  );

  if (loading) {
    return (
      <div className="p-4 bg-white rounded-lg shadow-lg">
        <p className="text-sm text-gray-500">Loading ML predictions...</p>
      </div>
    );
  }

  if (error) {
    return (
      <div className="p-4 bg-white rounded-lg shadow-lg">
        <p className="text-sm text-red-500">Error: {error}</p>
      </div>
    );
  }

  return (
    <div className="p-4 bg-white rounded-lg shadow-lg">
      <h3 className="mb-3 text-sm font-semibold text-gray-800">
        ML Road Congestion
      </h3>

      <div className="mb-3 flex gap-1">
        {PERIODS.map((p) => (
          <button
            key={p.key}
            onClick={() => handlePeriodChange(p.key)}
            className={`rounded px-2 py-1 text-xs font-medium ${
              selectedPeriod.toUpperCase() === p.key
                ? "bg-blue-500 text-white"
                : "bg-gray-100 text-gray-700 hover:bg-gray-200"
            }`}
          >
            {p.label}
          </button>
        ))}
      </div>

      <div className="mb-3 text-xs text-gray-500">
        <span className="inline-block w-3 h-3 mr-1 bg-red-500 rounded"></span> Very High
        <span className="inline-block w-3 h-3 ml-2 mr-1 bg-orange-500 rounded"></span> High
        <span className="inline-block w-3 h-3 ml-2 mr-1 bg-yellow-500 rounded"></span> Moderate
        <span className="inline-block w-3 h-3 ml-2 mr-1 bg-green-500 rounded"></span> Low
      </div>

      <div className="space-y-2 max-h-96 overflow-y-auto">
        {sortedPredictions.length === 0 ? (
          <p className="text-sm text-gray-500">No predictions for this period.</p>
        ) : (
          sortedPredictions.map((prediction) => (
            <div
              key={`${prediction.road_id}-${prediction.period}`}
              className="flex items-center justify-between p-2 bg-gray-50 rounded"
            >
              <div className="flex items-center gap-2">
                <div
                  className={`w-3 h-3 rounded ${getCongestionColor(
                    prediction.predicted_congestion
                  )}`}
                />
                <span className="text-xs font-medium text-gray-700">
                  Road {prediction.road_id}
                </span>
              </div>
              <div className="text-right">
                <span className="text-xs font-semibold text-gray-900">
                  {prediction.predicted_congestion.toFixed(2)}
                </span>
                <span className="text-[10px] text-gray-500 ml-1">
                  ({getCongestionLabel(prediction.predicted_congestion)})
                </span>
              </div>
            </div>
          ))
        )}
      </div>

      <div className="mt-3 pt-3 border-t border-gray-200">
        <p className="text-[10px] text-gray-400">
          Predictions from ML model (Gradient Boosting, R² = 0.94)
        </p>
      </div>
    </div>
  );
}
