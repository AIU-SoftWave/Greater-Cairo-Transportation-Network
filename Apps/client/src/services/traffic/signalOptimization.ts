import { AlgorithmResponse, TrafficSignalResultDto } from "@/types";
import { apiFetch } from "../api";

export async function getSignalOptimization(
  period: string = "MORNING",
  topN: number = 10,
  analyzeAllIntersections: boolean = false
): Promise<AlgorithmResponse<TrafficSignalResultDto>> {
  const params = new URLSearchParams({
    period,
    topN: topN.toString(),
    analyzeAllIntersections: analyzeAllIntersections.toString(),
  });
  return apiFetch<AlgorithmResponse<TrafficSignalResultDto>>(`signal-optimization?${params}`);
}
