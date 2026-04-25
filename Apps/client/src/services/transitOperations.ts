import { AlgorithmResponse, TransitSchedulingResultDto } from "@/types";
import { apiFetch } from "./api";

export async function getTransitSchedule(vehicles: number): Promise<AlgorithmResponse<TransitSchedulingResultDto>> {
  const params = new URLSearchParams({ vehicles: vehicles.toString() });
  return apiFetch<AlgorithmResponse<TransitSchedulingResultDto>>(`transit-operations?${params}`);
}
