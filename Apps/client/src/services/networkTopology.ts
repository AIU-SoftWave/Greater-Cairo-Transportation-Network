import {  NetworkTopologyData } from "@/types";
import { apiFetch } from "./api";

export async function fetchNetworkTopology(): Promise<NetworkTopologyData> {
  return apiFetch<NetworkTopologyData>("network-topology");
}
