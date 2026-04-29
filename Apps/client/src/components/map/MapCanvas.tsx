"use client";

import { useEffect, useMemo, useState } from "react";
import dynamic from "next/dynamic";

import type {
  Node,
  Road,
  AlgorithmResponse,
  MaintenancePlanningResultDto,
  TrafficSignalResultDto,
} from "@/types";
import type { AlgorithmType, IntersectionSignal } from "./types";

// Dynamically import Leaflet components only on client side
const MapContainer = dynamic(
  () => import("react-leaflet").then((mod) => mod.MapContainer),
  { ssr: false },
);
const TileLayer = dynamic(
  () => import("react-leaflet").then((mod) => mod.TileLayer),
  { ssr: false },
);
const Marker = dynamic(
  () => import("react-leaflet").then((mod) => mod.Marker),
  { ssr: false },
);
const Popup = dynamic(() => import("react-leaflet").then((mod) => mod.Popup), {
  ssr: false,
});
const Polyline = dynamic(
  () => import("react-leaflet").then((mod) => mod.Polyline),
  { ssr: false },
);

interface EdgeSegment {
  edge: Road;
  isClosed: boolean;
  pos: [number, number][];
}

interface MstRoadData {
  edge: Road;
  pos: [number, number][];
  isNewRoad: boolean;
}

interface MapCanvasProps {
  nodes: Node[];
  nodeLookup: Record<string, Node>;
  nodeIdByName: Record<string, string>;
  edgeSegments: EdgeSegment[];
  pathPositions: [number, number][];
  mstRoadsData: MstRoadData[];
  transitPathPositions: [number, number][];
  intersectionSignals: Record<string, IntersectionSignal> | null;
  algorithm: AlgorithmType;
  showMst: boolean;
  startId: string | null;
  endId: string | null;
  maintenanceResponse: AlgorithmResponse<MaintenancePlanningResultDto> | null;
  signalResponse: AlgorithmResponse<TrafficSignalResultDto> | null;
  onRoadClick: (edge: Road) => void;
  onMarkerClick: (nodeId: string) => void;
  onSignalMarkerClick: (node: Node, signalData: IntersectionSignal) => void;
}

// Get road color based on priority (for maintenance mode)
function getRoadPriorityColor(road: Road): string {
  if (!road.maintenancePriority) return "#9ca3af";
  if (road.maintenancePriority >= 7) return "#ef4444";
  if (road.maintenancePriority >= 4) return "#f59e0b";
  return "#22c55e";
}

// Get road congestion color
function getRoadCongestionColor(congestionPercent: number): string {
  if (congestionPercent > 100) return "#ef4444";
  if (congestionPercent > 70) return "#f97316";
  if (congestionPercent > 50) return "#f59e0b";
  return "#22c55e";
}

// Get intersection severity color
function getIntersectionSeverityColor(maxCongestion: number): string {
  if (maxCongestion > 1.0) return "#ef4444";
  if (maxCongestion > 0.7) return "#f97316";
  if (maxCongestion > 0.5) return "#f59e0b";
  return "#3b82f6";
}

export default function MapCanvas({
  nodes,
  nodeLookup,
  nodeIdByName,
  edgeSegments,
  pathPositions,
  mstRoadsData,
  transitPathPositions,
  intersectionSignals,
  algorithm,
  showMst,
  startId,
  endId,
  maintenanceResponse,
  signalResponse,
  onRoadClick,
  onMarkerClick,
  onSignalMarkerClick,
}: MapCanvasProps) {
  const [leaflet, setLeaflet] = useState<unknown | null>(null);

  // Import Leaflet only on client side
  useEffect(() => {
    import("leaflet").then((L) => {
      setLeaflet(L);
      // Fix default Leaflet icons
      // eslint-disable-next-line @typescript-eslint/no-explicit-any
      delete (L.Icon.Default.prototype as any)._getIconUrl;

      L.Icon.Default.mergeOptions({
        iconRetinaUrl:
          "https://unpkg.com/leaflet@1.9.4/dist/images/marker-icon-2x.png",
        iconUrl: "https://unpkg.com/leaflet@1.9.4/dist/images/marker-icon.png",
        shadowUrl:
          "https://unpkg.com/leaflet@1.9.4/dist/images/marker-shadow.png",
      });
    });
  }, []);

  // Icons (IMPORTANT: always return a valid icon)
  const icons = useMemo(() => {
    if (!leaflet) return null;

    // eslint-disable-next-line @typescript-eslint/no-explicit-any
    const L = leaflet as any;
    return {
      default: new L.Icon.Default(),

      green: L.divIcon({
        className: "custom-marker",
        html: `<div style="background-color:#22c55e;width:12px;height:12px;border-radius:50%;border:2px solid white;"></div>`,
        iconSize: [12, 12],
        iconAnchor: [6, 6],
      }),

      red: L.divIcon({
        className: "custom-marker",
        html: `<div style="background-color:#ef4444;width:12px;height:12px;border-radius:50%;border:2px solid white;"></div>`,
        iconSize: [12, 12],
        iconAnchor: [6, 6],
      }),
    };
  }, [leaflet]);

  const getMarkerIcon = (id: string) => {
    if (!icons) return undefined;
    if (id === startId) return icons.green;
    if (id === endId) return icons.red;
    return icons.default;
  };

  // Check if road is selected for maintenance
  const isRoadSelectedForMaintenance = (road: Road): boolean => {
    if (!maintenanceResponse) return false;

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
  };

  // Check if road is in signal response and get its congestion
  const getRoadCongestionFromSignal = (road: Road): number | null => {
    if (!signalResponse?.success) return null;

    for (const intersection of signalResponse.data.intersections) {
      for (const signalRoad of intersection.roads) {
        const fromNode = nodeLookup[road.fromNodeId];
        const toNode = nodeLookup[road.toNodeId];
        if (!fromNode || !toNode) continue;

        const fromMatch =
          fromNode.name.trim().toLowerCase() ===
          signalRoad.from.trim().toLowerCase();
        const toMatch =
          toNode.name.trim().toLowerCase() ===
          signalRoad.to.trim().toLowerCase();

        if (toMatch && fromMatch) {
          return signalRoad.congestionPercent;
        }
      }
    }

    return null;
  };

  return (
    <MapContainer
      center={[30.0444, 31.2357]}
      zoom={12}
      className="h-full w-full"
    >
      <TileLayer
        url="https://{s}.basemaps.cartocdn.com/light_all/{z}/{x}/{y}{r}.png"
        attribution="&copy; OpenStreetMap contributors &copy; CARTO"
      />

      {/* Roads */}
      {edgeSegments.map((segment, i) => {
        const { edge, pos, isClosed } = segment;
        let color = "#9ca3af";
        let weight = 2;
        let opacity = 0.6;

        const highlightSelected =
          algorithm === "maintenance" &&
          !!maintenanceResponse &&
          isRoadSelectedForMaintenance(edge);

        if (algorithm === "maintenance") {
          if (maintenanceResponse) {
            if (isRoadSelectedForMaintenance(edge)) {
              color = "#f97316";
              weight = 6;
              opacity = 1.0;
            } else {
              color = "#d1d5db";
              weight = 1;
              opacity = 0.2;
            }
          } else {
            color = getRoadPriorityColor(edge);
            weight = 3;
            opacity = 0.8;
          }
        } else if (algorithm === "signals" && signalResponse) {
          const congestion = getRoadCongestionFromSignal(edge);
          if (congestion !== null) {
            color = getRoadCongestionColor(congestion);
            weight =
              congestion > 100
                ? 5
                : congestion > 70
                  ? 4
                  : congestion > 50
                    ? 3
                    : 2;
            opacity = 0.9;
          } else {
            color = "#d1d5db";
            weight = 1;
            opacity = 0.3;
          }
        } else if (algorithm === "transit") {
          // Transit routes cannot be visualized on road map without route-to-road mapping
          color = "#9ca3af";
          weight = 2;
          opacity = 0.4;
        }

        // Override for closed roads
        let dashArray: string | undefined = undefined;
        if (isClosed) {
          color = "#ef4444";
          weight = 4;
          opacity = 1.0;
          dashArray = "10, 10";
        }

        return (
          <Polyline
            key={`${i}-${algorithm}-${maintenanceResponse?.data?.selectedRoadCount ?? 0}-${highlightSelected ? 1 : 0}-${signalResponse?.data.summary.optimizedIntersections ?? 0}-${isClosed ? 1 : 0}`}
            positions={pos}
            pathOptions={{ color, weight, opacity, dashArray }}
            eventHandlers={{
              click: () => onRoadClick(edge),
            }}
          />
        );
      })}

      {/* MST */}
      {showMst &&
        mstRoadsData.map((roadData, i) => {
          const color = roadData.isNewRoad ? "#f97316" : "#16a34a";
          const weight = roadData.isNewRoad ? 5 : 4;
          return (
            <Polyline
              key={`mst-${i}-${roadData.isNewRoad ? "new" : "existing"}`}
              positions={roadData.pos}
              color={color}
              weight={weight}
              opacity={0.9}
            />
          );
        })}

      {/* Shortest Path */}
      {pathPositions.length > 1 && (
        <Polyline
          positions={pathPositions}
          color="#3b82f6"
          weight={5}
          opacity={0.9}
        />
      )}

      {/* Transit Path */}
      {transitPathPositions.length > 0 && algorithm === "transit" && (
        <Polyline
          positions={transitPathPositions}
          color="#a855f7"
          weight={7}
          opacity={0.8}
        />
      )}

      {/* Signal Intersection Markers (when signals algorithm active) */}
      {algorithm === "signals" &&
        intersectionSignals &&
        Object.values(intersectionSignals).map((intersection) => {
          if (!intersection.nodeId) return null;
          const node = nodeLookup[intersection.nodeId];
          if (!node) return null;

          const color = getIntersectionSeverityColor(
            intersection.maxCongestion,
          );
          const size = 12 + intersection.signals.length * 2;

          return (
            <Marker
              key={`signal-${intersection.nodeId}`}
              position={[node.y, node.x]}
              icon={
                leaflet
                  ? // eslint-disable-next-line @typescript-eslint/no-explicit-any
                    (leaflet as any).divIcon({
                      className: "custom-signal-marker",
                      html: `<div style="background-color:${color};width:${size}px;height:${size}px;border-radius:50%;border:3px solid white;box-shadow:0 0 0 2px ${color};"></div>`,
                      iconSize: [size, size],
                      iconAnchor: [size / 2, size / 2],
                    })
                  : undefined
              }
              eventHandlers={{
                click: () => onSignalMarkerClick(node, intersection),
              }}
            >
              <Popup>
                <div className="text-sm">
                  <p className="font-semibold">
                    {intersection.intersectionName}
                  </p>
                  <p className="text-xs text-gray-600">
                    {intersection.signals.length} roads need optimization
                  </p>
                  <p className="text-xs text-gray-600">
                    Max congestion:{" "}
                    {(intersection.maxCongestion * 100).toFixed(0)}%
                  </p>
                </div>
              </Popup>
            </Marker>
          );
        })}

      {/* Nodes */}
      {nodes.map((node) => (
        <Marker
          key={node.id}
          position={[node.y, node.x]}
          icon={getMarkerIcon(node.id)}
          eventHandlers={{
            click: () => onMarkerClick(node.id),
          }}
        >
          <Popup>
            <p className="font-medium">{node.name}</p>
          </Popup>
        </Marker>
      ))}
    </MapContainer>
  );
}
