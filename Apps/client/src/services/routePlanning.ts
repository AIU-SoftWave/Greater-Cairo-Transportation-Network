import { AlgorithmResponse, ShortestPathResultDto } from "@/types";
import { apiFetch } from "./api";

export async function getShortestPath(from: string, to: string): Promise<AlgorithmResponse<ShortestPathResultDto>> {
  const params = new URLSearchParams({ from, to });
  return apiFetch<AlgorithmResponse<ShortestPathResultDto>>(`route-planning/shortest-path?${params}`);
}

export async function getTimeVaryingShortestPath(from: string, to: string, period: string): Promise<AlgorithmResponse<ShortestPathResultDto>> {
  const params = new URLSearchParams({ from, to, period });
  return apiFetch<AlgorithmResponse<ShortestPathResultDto>>(`route-planning/time-route?${params}`);
}
