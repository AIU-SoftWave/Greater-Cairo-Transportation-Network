import { AlgorithmResponse, ShortestPathResultDto } from "@/types";
import { apiFetch } from "../api";

export async function getShortestPath(from: string, to: string): Promise<AlgorithmResponse<ShortestPathResultDto>> {
  const params = new URLSearchParams({ from, to });
  return apiFetch<AlgorithmResponse<ShortestPathResultDto>>(`route-planning/shortest-path?${params}`);
}

export async function getTimeVaryingShortestPath(from: string, to: string, period: string, useMl: boolean = true): Promise<AlgorithmResponse<ShortestPathResultDto>> {
  const params = new URLSearchParams({ from, to, period, useMl: useMl.toString() });
  return apiFetch<AlgorithmResponse<ShortestPathResultDto>>(`route-planning/time-route?${params}`);
}
