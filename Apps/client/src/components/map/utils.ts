import type { Road } from "@/types";
import type { MaintenancePlanningResultDto, AlgorithmResponse } from "@/types";

/**
 * Checks whether a road is selected in the maintenance plan response.
 * Matches by road ID, by location names (forward and reverse), and by
 * mapping location names to node IDs as a final fallback.
 */
export function isRoadSelectedForMaintenance(
  road: Road,
  maintenanceResponse: AlgorithmResponse<MaintenancePlanningResultDto>,
  nodeLookup: Record<string, { name: string }>,
  nodeIdByName: Record<string, string>,
): boolean {
  const matchById = maintenanceResponse.data.selectedRoads.some(
    (r) => r.roadId === Math.abs(road.id),
  );
  if (matchById) return true;

  const fromNode = nodeLookup[road.fromNodeId];
  const toNode = nodeLookup[road.toNodeId];
  if (fromNode && toNode) {
    const fromName = fromNode.name.trim().toLowerCase();
    const toName = toNode.name.trim().toLowerCase();

    const matchByLocation = maintenanceResponse.data.selectedRoads.some(
      (r) =>
        (r.fromLocation ?? "").trim().toLowerCase() === fromName &&
        (r.toLocation ?? "").trim().toLowerCase() === toName,
    );
    if (matchByLocation) return true;

    const matchByLocationReverse =
      maintenanceResponse.data.selectedRoads.some(
        (r) =>
          (r.fromLocation ?? "").trim().toLowerCase() === toName &&
          (r.toLocation ?? "").trim().toLowerCase() === fromName,
      );
    if (matchByLocationReverse) return true;
  }

  for (const r of maintenanceResponse.data.selectedRoads) {
    const fromId = nodeIdByName[(r.fromLocation ?? "").trim().toLowerCase()];
    const toId = nodeIdByName[(r.toLocation ?? "").trim().toLowerCase()];

    if (!fromId || !toId) continue;

    if (road.fromNodeId === fromId && road.toNodeId === toId) return true;
    if (road.fromNodeId === toId && road.toNodeId === fromId) return true;
  }

  return false;
}
