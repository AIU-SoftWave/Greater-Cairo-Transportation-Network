import { AlgorithmResponse, TransitSchedulingResultDto, ShortestPathNodeDto } from "@/types";
import { apiFetch } from "../api";

export async function getTransitSchedule(vehicles: number): Promise<AlgorithmResponse<TransitSchedulingResultDto>> {
  const params = new URLSearchParams({ vehicles: vehicles.toString() });
  return apiFetch<AlgorithmResponse<TransitSchedulingResultDto>>(`transit-operations?${params}`);
}

export async function getRouteGeometry(id: string): Promise<ShortestPathNodeDto[]> {
  return apiFetch<ShortestPathNodeDto[]>(`transit-operations/route/${id}/geometry`);
}
