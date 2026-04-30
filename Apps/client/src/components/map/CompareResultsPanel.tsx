"use client";

import { AlgorithmResponse, ShortestPathResultDto } from "@/types";

interface CompareResultsPanelProps {
  responseA: AlgorithmResponse<ShortestPathResultDto> | null;
  responseB: AlgorithmResponse<ShortestPathResultDto> | null;
  algoA: string;
  algoB: string;
  colorA: string;
  colorB: string;
}

interface MetricRowProps {
  label: string;
  valueA: string | number;
  valueB: string | number;
  unit?: string;
  lowerIsBetter?: boolean;
  numeric?: boolean;
}

function MetricRow({
  label,
  valueA,
  valueB,
  unit,
  lowerIsBetter,
  numeric,
}: MetricRowProps) {
  let winner: "A" | "B" | "tie" = "tie";

  if (numeric && typeof valueA === "number" && typeof valueB === "number") {
    if (lowerIsBetter) {
      winner = valueA < valueB ? "A" : valueB < valueA ? "B" : "tie";
    } else {
      winner = valueA > valueB ? "A" : valueB > valueA ? "B" : "tie";
    }
  }

  const formatValue = (val: string | number) => {
    if (typeof val === "number") {
      return val.toFixed(1);
    }
    return val;
  };

  return (
    <div className="grid grid-cols-3 gap-2 py-2 text-xs border-b border-gray-100 last:border-0 text-gray-900">
      <div
        className={`text-right font-medium  ${winner === "A" ? " bg-green-50 px-1 rounded" : ""}`}
      >
        {formatValue(valueA)}
        {unit && <span className="text-gray-400 ml-1">{unit}</span>}
      </div>
      <div className="text-center text-gray-500 font-semibold">{label}</div>
      <div
        className={`text-left font-medium ${winner === "B" ? "text-gray-900 bg-green-50 px-1 rounded" : ""}`}
      >
        {formatValue(valueB)}
        {unit && <span className="text-gray-400 ml-1">{unit}</span>}
      </div>
    </div>
  );
}

export default function CompareResultsPanel({
  responseA,
  responseB,
  algoA,
  algoB,
  colorA,
  colorB,
}: CompareResultsPanelProps) {
  const dataA = responseA?.success ? responseA.data : null;
  const dataB = responseB?.success ? responseB.data : null;

  if (!dataA && !dataB) {
    return (
      <div className="mb-4 p-3 bg-gray-50 rounded border border-gray-200">
        <p className="text-xs text-gray-500 text-center">
          Select start and end points to compare algorithms
        </p>
      </div>
    );
  }

  const traceA = responseA?.trace;
  const traceB = responseB?.trace;

  return (
    <div className="mb-4 bg-white rounded border border-gray-200 overflow-hidden">
      {/* Header */}
      <div className="grid grid-cols-3 gap-2 p-3 bg-gray-50 border-b border-gray-200">
        <div className="text-right">
          <span
            className="inline-block w-3 h-3 rounded-full mr-1"
            style={{ backgroundColor: colorA }}
          />
          <span className="text-xs font-bold text-gray-700">{algoA}</span>
        </div>
        <div className="text-center">
          <span className="text-xs font-semibold text-gray-500 uppercase">
            vs
          </span>
        </div>
        <div className="text-left">
          <span
            className="inline-block w-3 h-3 rounded-full mr-1"
            style={{ backgroundColor: colorB }}
          />
          <span className="text-xs font-bold text-gray-700">{algoB}</span>
        </div>
      </div>

      {/* Metrics */}
      <div className="p-3">
        {!dataA?.found && !dataB?.found ? (
          <p className="text-xs text-red-500 text-center">No path found</p>
        ) : (
          <>
            <MetricRow
              label="Distance"
              valueA={dataA?.totalDistance ?? "—"}
              valueB={dataB?.totalDistance ?? "—"}
              unit="km"
              lowerIsBetter={true}
              numeric={true}
            />
            <MetricRow
              label="Est. Time"
              valueA={
                dataA?.estimatedTravelTimeMinutes ?? dataA?.totalDistance ?? "—"
              }
              valueB={
                dataB?.estimatedTravelTimeMinutes ?? dataB?.totalDistance ?? "—"
              }
              unit="min"
              lowerIsBetter={true}
              numeric={true}
            />
            <MetricRow
              label="Execution"
              valueA={traceA?.executionTimeMs?.toFixed(2) ?? "—"}
              valueB={traceB?.executionTimeMs?.toFixed(2) ?? "—"}
              unit="ms"
              lowerIsBetter={true}
              numeric={true}
            />
            <MetricRow
              label="Visited"
              valueA={traceA?.visitedNodes ?? "—"}
              valueB={traceB?.visitedNodes ?? "—"}
              lowerIsBetter={true}
              numeric={true}
            />
            <MetricRow
              label="Expanded"
              valueA={traceA?.expandedNodes ?? "—"}
              valueB={traceB?.expandedNodes ?? "—"}
              lowerIsBetter={true}
              numeric={true}
            />
            <MetricRow
              label="Roads"
              valueA={dataA?.pathRoads?.length ?? "—"}
              valueB={dataB?.pathRoads?.length ?? "—"}
              lowerIsBetter={true}
              numeric={true}
            />
          </>
        )}
      </div>

      {/* Legend */}
      <div className="px-3 py-2 bg-gray-50 border-t border-gray-200">
        <p className="text-[10px] text-gray-400 text-center">
          Green highlights = better performance
        </p>
      </div>
    </div>
  );
}
