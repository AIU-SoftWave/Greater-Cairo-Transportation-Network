export type AlgorithmType =
  | "dijkstra"
  | "astar"
  | "time-varying"
  | "maintenance"
  | "signals"
  | "transit"
  | "simulation"
  | "compare";

export const PERIODS = ["morning", "evening", "night"];

export type CompareAlgorithmType = "dijkstra" | "astar" | "time-varying";

export const COMPARE_ALGORITHMS: {
  key: CompareAlgorithmType;
  label: string;
}[] = [
  { key: "dijkstra", label: "Dijkstra" },
  { key: "astar", label: "A*" },
  { key: "time-varying", label: "Time-Varying" },
];

export interface IntersectionSignal {
  intersectionName: string;
  nodeId: string | null;
  cycleTimeSeconds: number;
  signals: Array<{
    from: string;
    to: string;
    congestionPercent: number;
    priority: number;
    greenTimeSeconds: number;
  }>;
  maxCongestion: number;
}
