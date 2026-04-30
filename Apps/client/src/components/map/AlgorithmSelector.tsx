"use client";

import type { AlgorithmType } from "./types";

interface AlgorithmSelectorProps {
  algorithm: AlgorithmType;
  onAlgorithmChange: (algo: AlgorithmType) => void;
}

const ALGORITHMS: { key: AlgorithmType; label: string }[] = [
  { key: "dijkstra", label: "Dijkstra" },
  { key: "astar", label: "A*" },
  { key: "time-varying", label: "Time-Varying" },
  { key: "maintenance", label: "Maintenance" },
  { key: "signals", label: "Signals" },
  { key: "transit", label: "Transit" },
  { key: "simulation", label: "Simulation" },
  { key: "compare", label: "Compare" },
];

export default function AlgorithmSelector({
  algorithm,
  onAlgorithmChange,
}: AlgorithmSelectorProps) {
  return (
    <div className="mb-4">
      <p className="mb-2 text-xs font-semibold uppercase text-gray-500">
        Algorithm
      </p>
      <div className="flex gap-2 flex-wrap">
        {ALGORITHMS.map(({ key, label }) => (
          <button
            key={key}
            onClick={() => onAlgorithmChange(key)}
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
  );
}
