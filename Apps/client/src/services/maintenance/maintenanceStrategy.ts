import { AlgorithmResponse, MaintenancePlanningResultDto } from "@/types";
import { apiFetch } from "../api";

export async function getMaintenancePlan(budget: number): Promise<AlgorithmResponse<MaintenancePlanningResultDto>> {
  const params = new URLSearchParams({ budget: budget.toString() });
  return apiFetch<AlgorithmResponse<MaintenancePlanningResultDto>>(`maintenance-strategy?${params}`);
}
