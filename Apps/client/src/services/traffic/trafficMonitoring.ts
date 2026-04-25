import { TrafficFlow } from "@/types";
import { apiFetch } from "../api";

export async function getTrafficByRoadId(roadId: number): Promise<TrafficFlow[]> {
  return apiFetch<TrafficFlow[]>(`traffic-monitoring/road/${roadId}`);
}

export async function getTrafficByPeriod(period: string): Promise<TrafficFlow[]> {
  return apiFetch<TrafficFlow[]>(`traffic-monitoring/period/${period}`);
}
