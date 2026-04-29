"use client";

import type {
  Road,
  Node,
  AlgorithmResponse,
  ShortestPathResultDto,
  MstResultDto,
  MaintenancePlanningResultDto,
  TrafficSignalResultDto,
  TransitSchedulingResultDto,
} from "@/types";
import type { AlgorithmType } from "./types";

interface MstRoadData {
  edge: Road;
  pos: [number, number][];
  isNewRoad: boolean;
}

const resultClassName = "ml-1 font-medium text-black";

function getIntersectionSeverityColor(maxCongestion: number): string {
  if (maxCongestion > 1.0) return "#ef4444";
  if (maxCongestion > 0.7) return "#f97316";
  if (maxCongestion > 0.5) return "#f59e0b";
  return "#3b82f6";
}

interface ResultsDashboardProps {
  algorithm: AlgorithmType;
  response: AlgorithmResponse<ShortestPathResultDto> | null;
  showMst: boolean;
  mstResponse: AlgorithmResponse<MstResultDto> | null;
  mstRoadsData: MstRoadData[];
  nodeLookup: Record<string, Node>;
  maintenanceResponse: AlgorithmResponse<MaintenancePlanningResultDto> | null;
  matchedMaintenanceRoadsCount: number;
  signalResponse: AlgorithmResponse<TrafficSignalResultDto> | null;
  transitResponse: AlgorithmResponse<TransitSchedulingResultDto> | null;
  onShowTransitRoute: (routeId: string) => void;
}

export default function ResultsDashboard({
  algorithm,
  response,
  showMst,
  mstResponse,
  mstRoadsData,
  nodeLookup,
  maintenanceResponse,
  matchedMaintenanceRoadsCount,
  signalResponse,
  transitResponse,
  onShowTransitRoute,
}: ResultsDashboardProps) {
  return (
    <>
      {/* Path Results */}
      {response && (
        <div className="mb-3 rounded-md bg-gray-50 p-3 text-xs">
          <p className="mb-1 font-semibold text-gray-700">Results:</p>
          <div className="grid grid-cols-2 gap-2">
            <div>
              <span className="text-gray-500">Algorithm:</span>
              <span className={resultClassName}>{response.algorithmName}</span>
            </div>
            <div>
              <span className="text-gray-500">Time:</span>
              <span className={resultClassName}>
                {response.trace.executionTimeMs.toFixed(2)}ms
              </span>
            </div>
            <div>
              <span className="text-gray-500">Visited:</span>
              <span className={resultClassName}>
                {response.trace.visitedNodes}
              </span>
            </div>
            <div>
              <span className="text-gray-500">Expanded:</span>
              <span className={resultClassName}>
                {response.trace.expandedNodes}
              </span>
            </div>
            <div>
              <span className="text-gray-500">Distance:</span>
              <span className={resultClassName}>
                {response.data.totalDistance.toFixed(1)}km
              </span>
            </div>
            {response.data.estimatedTravelTimeMinutes !== undefined && (
              <div>
                <span className="text-gray-500">Est. Time:</span>
                <span className={resultClassName}>
                  {response.data.estimatedTravelTimeMinutes.toFixed(1)} min
                </span>
              </div>
            )}
            <div>
              <span className="text-gray-500">Roads:</span>
              <span className={resultClassName}>
                {response.data.pathRoads.length}
              </span>
            </div>
          </div>
          {response.message && (
            <p className="mt-2 text-gray-600">{response.message}</p>
          )}
        </div>
      )}

      {/* MST Results */}
      {showMst && mstResponse && (
        <div className="mb-3 rounded-md bg-green-50 p-3 text-xs">
          <p className="mb-1 font-semibold text-gray-700">MST Results:</p>
          <div className="grid grid-cols-2 gap-2">
            <div>
              <span className="text-gray-500">Connected:</span>
              <span className={resultClassName}>
                {mstResponse.data.connected ? "Yes" : "No"}
              </span>
            </div>
            <div>
              <span className="text-gray-500">Total Cost:</span>
              <span className={resultClassName}>
                ${mstResponse.data.totalConstructionCost.toLocaleString()}
              </span>
            </div>
            <div>
              <span className="text-gray-500">Total Nodes:</span>
              <span className={resultClassName}>
                {mstResponse.data.totalNodes}
              </span>
            </div>
            <div>
              <span className="text-gray-500">Selected Roads:</span>
              <span className={resultClassName}>
                {mstResponse.data.selectedRoadCount}
              </span>
            </div>
          </div>

          {/* Road Breakdown */}
          {mstRoadsData.length > 0 && (
            <div className="mt-2 pt-2 border-t border-green-200">
              <p className="font-semibold text-gray-700 mb-1">
                Road Breakdown:
              </p>
              <div className="space-y-1">
                <div className="flex items-center gap-2">
                  <span
                    className="inline-block w-3 h-3 rounded-full"
                    style={{ backgroundColor: "#16a34a" }}
                  />
                  <span className="text-gray-600">
                    Existing Roads:{" "}
                    {mstRoadsData.filter((r) => !r.isNewRoad).length}
                  </span>
                </div>
                <div className="flex items-center gap-2">
                  <span
                    className="inline-block w-3 h-3 rounded-full"
                    style={{ backgroundColor: "#f97316" }}
                  />
                  <span className="text-gray-600">
                    New Roads (Potential):{" "}
                    {mstRoadsData.filter((r) => r.isNewRoad).length}
                  </span>
                </div>
              </div>

              {/* New roads with construction costs */}
              {mstRoadsData.filter((r) => r.isNewRoad).length > 0 && (
                <div className="mt-2 pt-2 border-t border-orange-200">
                  <p className="font-semibold text-orange-700 mb-1">
                    New Infrastructure:
                  </p>
                  {mstRoadsData
                    .filter((r) => r.isNewRoad)
                    .map((roadData, idx) => (
                      <div
                        key={idx}
                        className="text-[10px] text-gray-600 mb-1 flex justify-between"
                      >
                        <span>
                          {nodeLookup[roadData.edge.fromNodeId]?.name} →{" "}
                          {nodeLookup[roadData.edge.toNodeId]?.name}
                        </span>
                        <span className="font-semibold text-orange-600">
                          $
                          {(
                            roadData.edge.constructionCost || 0
                          ).toLocaleString()}
                        </span>
                      </div>
                    ))}
                </div>
              )}
            </div>
          )}

          {mstResponse.message && (
            <p className="mt-2 text-gray-600">{mstResponse.message}</p>
          )}
        </div>
      )}

      {/* Maintenance Results */}
      {algorithm === "maintenance" && maintenanceResponse && (
        <div className="mb-3 rounded-md bg-orange-50 p-3 text-xs">
          <p className="mb-1 font-semibold text-gray-700">Maintenance Plan:</p>
          <p className="mb-2 text-xs text-gray-600">
            Matched roads on map: {matchedMaintenanceRoadsCount}
          </p>
          <div className="grid grid-cols-2 gap-2">
            <div>
              <span className="text-gray-500">Budget:</span>
              <span className={resultClassName}>
                ${maintenanceResponse.data.budget.toLocaleString()}
              </span>
            </div>
            <div>
              <span className="text-gray-500">Condition Imp:</span>
              <span className={resultClassName}>
                +
                {maintenanceResponse.data.expectedConditionImprovement.toFixed(
                  1,
                )}
              </span>
            </div>
          </div>
          <div className="mt-2 pt-2 border-t border-orange-200">
            <p className="font-semibold text-gray-700">Selected Roads:</p>
            {maintenanceResponse.data.selectedRoads.map((r) => (
              <div key={r.roadId} className="mt-1">
                <span className="text-gray-600">
                  {r.fromLocation} → {r.toLocation} (ID: {r.roadId})
                </span>
              </div>
            ))}
          </div>
          {maintenanceResponse.message && (
            <p className="mt-2 text-gray-600">{maintenanceResponse.message}</p>
          )}
        </div>
      )}

      {/* Signal Results */}
      {algorithm === "signals" && signalResponse && (
        <div className="mb-3 rounded-md bg-blue-50 p-3 text-xs">
          <p className="mb-1 font-semibold text-gray-700">
            Signal Optimization:
          </p>
          <p className="mb-2 text-xs text-gray-600">
            Period: {signalResponse.data.period}
          </p>
          <div className="grid grid-cols-2 gap-2">
            <div>
              <span className="text-gray-500">Intersections:</span>
              <span className={resultClassName}>
                {signalResponse.data.summary.optimizedIntersections}
              </span>
            </div>
            <div>
              <span className="text-gray-500">Roads:</span>
              <span className={resultClassName}>
                {signalResponse.data.summary.roadsAnalyzed}
              </span>
            </div>
            <div>
              <span className="text-gray-500">Wait Reduction:</span>
              <span className={resultClassName}>
                {signalResponse.data.summary.estimatedWaitTimeReductionPercent.toFixed(
                  1,
                )}
                %
              </span>
            </div>
            <div>
              <span className="text-gray-500">Analyzed:</span>
              <span className={resultClassName}>
                {signalResponse.data.summary.intersectionsAnalyzed}
              </span>
            </div>
          </div>
          <div className="mt-2 pt-2 border-t border-blue-200">
            <p className="font-semibold text-gray-700">Top Intersections:</p>
            {signalResponse.data.intersections
              .sort((a, b) => {
                const maxA = Math.max(
                  ...a.roads.map((r) => r.congestionPercent / 100),
                );
                const maxB = Math.max(
                  ...b.roads.map((r) => r.congestionPercent / 100),
                );
                return maxB - maxA;
              })
              .slice(0, 5)
              .map((intersection) => {
                const maxCongestion = Math.max(
                  ...intersection.roads.map((r) => r.congestionPercent / 100),
                );
                return (
                  <div key={intersection.name} className="mt-1">
                    <span
                      className="inline-block w-2 h-2 rounded-full mr-1"
                      style={{
                        backgroundColor:
                          getIntersectionSeverityColor(maxCongestion),
                      }}
                    />
                    <span className="text-gray-600">
                      {intersection.name} ({intersection.roads.length} roads,{" "}
                      {intersection.cycleTimeSeconds}s cycle,{" "}
                      {(maxCongestion * 100).toFixed(0)}% max)
                    </span>
                  </div>
                );
              })}
          </div>
          {signalResponse.message && (
            <p className="mt-2 text-gray-600">{signalResponse.message}</p>
          )}
        </div>
      )}

      {/* Transit Results */}
      {algorithm === "transit" && transitResponse && (
        <div className="mb-3 rounded-md bg-purple-50 p-3 text-xs">
          <p className="mb-1 font-semibold text-gray-700">
            Transit Scheduling:
          </p>
          <p className="mb-2 text-xs text-gray-500 italic">
            Note: Map visualization requires route-to-road mapping data. Routes
            are shown in dashboard only.
          </p>
          <div className="grid grid-cols-2 gap-2">
            <div>
              <span className="text-gray-500">Total Vehicles:</span>
              <span className={resultClassName}>
                {transitResponse.data.totalVehicles}
              </span>
            </div>
            <div>
              <span className="text-gray-500">Assigned:</span>
              <span className={resultClassName}>
                {transitResponse.data.assignedVehicles} (
                {(
                  (transitResponse.data.assignedVehicles /
                    transitResponse.data.totalVehicles) *
                  100
                ).toFixed(1)}
                %)
              </span>
            </div>
            <div>
              <span className="text-gray-500">Coverage:</span>
              <span className={resultClassName}>
                {(transitResponse.data.coverageRatio * 100).toFixed(1)}%
              </span>
            </div>
            <div>
              <span className="text-gray-500">Active Routes:</span>
              <span className={resultClassName}>
                {transitResponse.data.activeRoutes}/
                {transitResponse.data.totalRoutes}
              </span>
            </div>
            <div>
              <span className="text-gray-500">Passengers Served:</span>
              <span className={resultClassName}>
                {transitResponse.data.estimatedPassengersServed.toLocaleString()}
              </span>
            </div>
            <div>
              <span className="text-gray-500">Total Demand:</span>
              <span className={resultClassName}>
                {transitResponse.data.totalDemand.toLocaleString()}
              </span>
            </div>
          </div>
          <div className="mt-2 pt-2 border-t border-purple-200">
            <p className="font-semibold text-gray-700">Route Allocations:</p>
            {transitResponse.data.routeAllocations
              .sort((a, b) => b.assignedVehicles - a.assignedVehicles)
              .slice(0, 5)
              .map((route) => (
                <div
                  key={route.routeId}
                  className="mt-1 border-b border-purple-100 pb-1"
                >
                  <span className="text-gray-600">
                    {route.routeId} ({route.routeType}):{" "}
                    {route.assignedVehicles} vehicles,{" "}
                    {route.estimatedServed.toLocaleString()} passengers
                  </span>
                  <button
                    onClick={() => onShowTransitRoute(route.routeId)}
                    className="block mt-1 text-[10px] text-blue-500 font-semibold hover:underline"
                  >
                    View Route Geometry
                  </button>
                </div>
              ))}
          </div>
          {transitResponse.message && (
            <p className="mt-2 text-gray-600">{transitResponse.message}</p>
          )}
        </div>
      )}
    </>
  );
}
