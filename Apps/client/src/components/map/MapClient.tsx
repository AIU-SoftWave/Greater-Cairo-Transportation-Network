"use client";

import "leaflet/dist/leaflet.css";
import { MapContainer, TileLayer, CircleMarker, Polyline, Tooltip, useMap } from "react-leaflet";
import { useEffect } from "react";
import type { NetworkTopologyData, Node, Road } from "@/types";
import type { RunResult } from "../TransportDashboard";

// ── helpers ────────────────────────────────────────────────────────────────

function latLng(node: Node): [number, number] {
  // database stores x = longitude, y = latitude
  return [node.y, node.x];
}

function buildNodeSet(ids: string[]): Set<string> {
  return new Set(ids);
}

function buildRoadKey(from: string, to: string): string {
  return `${from}|${to}`;
}

// ── auto-fit map bounds ────────────────────────────────────────────────────

interface FitBoundsProps {
  nodes: Node[];
}

function FitBounds({ nodes }: FitBoundsProps) {
  const map = useMap();
  useEffect(() => {
    if (nodes.length === 0) return;
    const lats = nodes.map((n) => n.y);
    const lngs = nodes.map((n) => n.x);
    const minLat = Math.min(...lats);
    const maxLat = Math.max(...lats);
    const minLng = Math.min(...lngs);
    const maxLng = Math.max(...lngs);
    map.fitBounds(
      [
        [minLat - 0.02, minLng - 0.02],
        [maxLat + 0.02, maxLng + 0.02],
      ],
      { animate: false },
    );
  }, [map, nodes]);
  return null;
}

// ── colour helpers ─────────────────────────────────────────────────────────

function nodeColor(
  id: string,
  isCritical: boolean,
  pathNodeIds: Set<string>,
  visitedNodeIds: Set<string>,
  startId: string | undefined,
  endId: string | undefined,
): { fillColor: string; color: string } {
  if (id === startId) return { fillColor: "#EF4444", color: "#B91C1C" };
  if (id === endId) return { fillColor: "#8B5CF6", color: "#6D28D9" };
  if (pathNodeIds.has(id)) return { fillColor: "#22C55E", color: "#15803D" };
  if (visitedNodeIds.has(id)) return { fillColor: "#EAB308", color: "#A16207" };
  if (isCritical) return { fillColor: "#F97316", color: "#C2410C" };
  return { fillColor: "#3B82F6", color: "#1D4ED8" };
}

type RoadStyle = { color: string; weight: number; opacity: number };

function roadStyle(
  fromId: string,
  toId: string,
  pathRoadKeys: Set<string>,
  mstRoadKeys: Set<string>,
  maintenanceRoadKeys: Set<string>,
): RoadStyle {
  const key = buildRoadKey(fromId, toId);
  const revKey = buildRoadKey(toId, fromId);
  if (pathRoadKeys.has(key) || pathRoadKeys.has(revKey))
    return { color: "#2563EB", weight: 4, opacity: 0.9 };
  if (mstRoadKeys.has(key) || mstRoadKeys.has(revKey))
    return { color: "#16A34A", weight: 3, opacity: 0.85 };
  if (maintenanceRoadKeys.has(key) || maintenanceRoadKeys.has(revKey))
    return { color: "#EA580C", weight: 3, opacity: 0.85 };
  return { color: "#6B7280", weight: 1.5, opacity: 0.5 };
}

// ── main map component ─────────────────────────────────────────────────────

interface MapClientProps {
  topology: NetworkTopologyData | null;
  result: RunResult | null;
}

export default function MapClient({ topology, result }: MapClientProps) {
  if (!topology) {
    return (
      <div className="flex h-full items-center justify-center bg-gray-100 text-gray-500">
        Loading map data…
      </div>
    );
  }

  const { nodes, edges } = topology;

  // ── extract highlighted sets from result ─────────────────────────────────
  let pathNodeIds = new Set<string>();
  let visitedNodeIds = new Set<string>();
  let pathRoadKeys = new Set<string>();
  let mstRoadKeys = new Set<string>();
  let maintenanceRoadKeys = new Set<string>();
  let startId: string | undefined;
  let endId: string | undefined;

  if (result) {
    const { algorithmId, response, inputs } = result;

    if (algorithmId === "dijkstra" || algorithmId === "time-dijkstra" || algorithmId === "astar") {
      const data = response.data as {
        pathNodes?: Array<{ id: string }>;
        pathRoads?: Array<{ fromNodeId: string; toNodeId: string }>;
      } | null;
      if (data) {
        pathNodeIds = buildNodeSet((data.pathNodes ?? []).map((n) => n.id));
        pathRoadKeys = new Set(
          (data.pathRoads ?? []).map((r) => buildRoadKey(r.fromNodeId, r.toNodeId)),
        );
      }
      startId = inputs.from;
      endId = inputs.to;
    }

    if (algorithmId === "mst") {
      const data = response.data as {
        nodes?: Array<{ id: string }>;
        selectedRoads?: Array<{ fromNodeId: string; toNodeId: string }>;
      } | null;
      if (data) {
        pathNodeIds = buildNodeSet((data.nodes ?? []).map((n) => n.id));
        mstRoadKeys = new Set(
          (data.selectedRoads ?? []).map((r) => buildRoadKey(r.fromNodeId, r.toNodeId)),
        );
      }
    }

    if (algorithmId === "maintenance") {
      // Maintenance roads are highlighted by roadId, but the topology edges don't carry
      // a road id. Map highlighting for maintenance is limited to the results panel list.
    }
  }

  // ── node/road arrays ─────────────────────────────────────────────────────
  const nodeMap = new Map<string, Node>(nodes.map((n) => [n.id, n]));

  return (
    <MapContainer
      center={[30.05, 31.25]}
      zoom={11}
      style={{ height: "100%", width: "100%" }}
      zoomControl={true}
    >
      <TileLayer
        attribution='&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors'
        url="https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png"
      />

      <FitBounds nodes={nodes} />

      {/* roads / edges */}
      {edges.map((edge: Road, idx: number) => {
        const from = nodeMap.get(edge.fromNodeId);
        const to = nodeMap.get(edge.toNodeId);
        if (!from || !to) return null;
        const style = roadStyle(
          edge.fromNodeId,
          edge.toNodeId,
          pathRoadKeys,
          mstRoadKeys,
          maintenanceRoadKeys,
        );
        return (
          <Polyline
            key={idx}
            positions={[latLng(from), latLng(to)]}
            pathOptions={style}
          />
        );
      })}

      {/* nodes / markers */}
      {nodes.map((node: Node) => {
        const { fillColor, color } = nodeColor(
          node.id,
          node.isCritical,
          pathNodeIds,
          visitedNodeIds,
          startId,
          endId,
        );
        const isHighlighted =
          node.id === startId ||
          node.id === endId ||
          pathNodeIds.has(node.id) ||
          visitedNodeIds.has(node.id);
        return (
          <CircleMarker
            key={node.id}
            center={latLng(node)}
            radius={isHighlighted ? 9 : node.isCritical ? 7 : 5}
            pathOptions={{
              fillColor,
              color,
              fillOpacity: 0.85,
              weight: 2,
            }}
          >
            <Tooltip direction="top" offset={[0, -6]} opacity={0.95}>
              <span className="text-xs font-medium">
                {node.name}
                {node.isCritical ? " ⭐" : ""}
              </span>
            </Tooltip>
          </CircleMarker>
        );
      })}
    </MapContainer>
  );
}
