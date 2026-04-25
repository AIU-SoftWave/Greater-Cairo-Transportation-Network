import { TransportRoute, RouteStop } from "@/types";
import { apiFetch } from "./api";

export async function getAllRoutes(): Promise<TransportRoute[]> {
  return apiFetch<TransportRoute[]>("route-catalog");
}

export async function getRouteById(id: string): Promise<TransportRoute> {
  return apiFetch<TransportRoute>(`route-catalog/${id}`);
}

export async function getRouteStops(id: string): Promise<RouteStop[]> {
  return apiFetch<RouteStop[]>(`route-catalog/${id}/stops`);
}
