import { RoadFull, RoadMaintenance } from "@/types";
import { apiFetch } from "./api";

export async function getAllRoads(): Promise<RoadFull[]> {
  return apiFetch<RoadFull[]>("road-network");
}

export async function getRoadById(id: number): Promise<RoadFull> {
  return apiFetch<RoadFull>(`road-network/${id}`);
}

export async function getRoadsByFromLocation(locationId: string): Promise<RoadFull[]> {
  return apiFetch<RoadFull[]>(`road-network/from/${locationId}`);
}

export async function getRoadMaintenance(roadId: number): Promise<RoadMaintenance> {
  return apiFetch<RoadMaintenance>(`road-network/${roadId}/maintenance`);
}
