import { TrafficPeriodMultiplier } from "@/types";
import { apiFetch } from "./api";

export async function getAllPeriodMultipliers(): Promise<TrafficPeriodMultiplier[]> {
  return apiFetch<TrafficPeriodMultiplier[]>("traffic-policy");
}

export async function getPeriodMultiplier(period: string): Promise<TrafficPeriodMultiplier> {
  return apiFetch<TrafficPeriodMultiplier>(`traffic-policy/${period}`);
}
