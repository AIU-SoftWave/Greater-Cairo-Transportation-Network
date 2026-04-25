import { Location } from "@/types";
import { apiFetch } from "../api";

export async function getAllLocations(): Promise<Location[]> {
  return apiFetch<Location[]>("city-locations");
}

export async function getLocationById(id: string): Promise<Location> {
  return apiFetch<Location>(`city-locations/${id}`);
}
