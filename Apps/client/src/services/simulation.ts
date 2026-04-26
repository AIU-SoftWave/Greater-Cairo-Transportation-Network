import { apiFetch } from "./api";

export interface PerformanceMetric {
  algorithmName: string;
  executionTimeMs: number;
  visitedNodes: number;
  expandedNodes: number;
  timestamp: string;
}

export async function toggleRoadClosure(roadId: number): Promise<void> {
  await apiFetch(`simulation/toggle-road-closure/${roadId}`, { method: "POST" });
}

export async function resetSimulation(): Promise<void> {
  await apiFetch(`simulation/reset`, { method: "POST" });
}

export async function getClosedRoads(): Promise<number[]> {
  return apiFetch<number[]>(`simulation/closed-roads`);
}

export async function setPreemption(roadId: number, active: boolean): Promise<void> {
  await apiFetch(`simulation/preemption/${roadId}?active=${active}`, { method: "POST" });
}

export async function getMetrics(): Promise<PerformanceMetric[]> {
  return apiFetch<PerformanceMetric[]>(`simulation/metrics`);
}

export async function setWeather(weather: number): Promise<void> {
  await apiFetch(`simulation/weather?weather=${weather}`, { method: "POST" });
}
