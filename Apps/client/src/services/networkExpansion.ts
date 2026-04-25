import { AlgorithmResponse, MstResultDto } from "@/types";
import { apiFetch } from "./api";

export async function getCheapestNetwork(): Promise<AlgorithmResponse<MstResultDto>> {
  return apiFetch<AlgorithmResponse<MstResultDto>>("network-expansion");
}
