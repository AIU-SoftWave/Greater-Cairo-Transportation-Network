export type AlgorithmType =
  | "dijkstra"
  | "astar"
  | "time-varying"
  | "maintenance"
  | "signals"
  | "transit"
  | "simulation";

export const PERIODS = ["morning", "evening", "night"];

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
