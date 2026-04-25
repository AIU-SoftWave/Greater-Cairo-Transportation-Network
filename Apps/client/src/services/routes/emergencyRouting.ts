import { AlgorithmResponse, ShortestPathResultDto } from "@/types";
import { apiFetch } from "../api";

export async function getEmergencyRoute(from: string, to: string): Promise<AlgorithmResponse<ShortestPathResultDto>> {
  const params = new URLSearchParams({ from, to });
  return apiFetch<AlgorithmResponse<ShortestPathResultDto>>(`emergency-routing?${params}`);
}
