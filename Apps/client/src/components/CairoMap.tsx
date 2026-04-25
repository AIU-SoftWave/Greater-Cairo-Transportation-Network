"use client";

import { useEffect, useState } from "react";
import { MapContainer, TileLayer, CircleMarker, Polyline, Tooltip } from "react-leaflet";
import "leaflet/dist/leaflet.css";
import { fetchNetworkTopology } from "@/services/network/networkTopology";
import type { NetworkTopologyData } from "@/types";

const CAIRO_CENTER: [number, number] = [30.05, 31.25];
const DEFAULT_ZOOM = 11;

export default function CairoMap() {
  const [topology, setTopology] = useState<NetworkTopologyData | null>(null);
  const [error, setError] = useState<string | null>(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    fetchNetworkTopology()
      .then(setTopology)
      .catch((err: Error) => setError(err.message))
      .finally(() => setLoading(false));
  }, []);

  if (loading) {
    return (
      <div className="flex h-full items-center justify-center text-gray-500">
        Loading map…
      </div>
    );
  }

  if (error) {
    return (
      <div className="flex h-full items-center justify-center text-red-600">
        Failed to load network data: {error}
      </div>
    );
  }

  const nodes = topology?.nodes ?? [];
  const edges = topology?.edges ?? [];
  const nodeIndex = topology?.nodeIndex ?? {};

  return (
    <MapContainer
      center={CAIRO_CENTER}
      zoom={DEFAULT_ZOOM}
      style={{ height: "100%", width: "100%" }}
    >
      <TileLayer
        attribution='&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors'
        url="https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png"
      />

      {edges.map((road, index) => {
        const from = nodeIndex[road.fromNodeId];
        const to = nodeIndex[road.toNodeId];
        if (!from || !to) return null;
        return (
          <Polyline
            key={`road-${road.fromNodeId}-${road.toNodeId}-${index}`}
            positions={[
              [from.y, from.x],
              [to.y, to.x],
            ]}
            color="#3B82F6"
            weight={2}
            opacity={0.7}
          />
        );
      })}

      {nodes.map((node) => (
        <CircleMarker
          key={node.id}
          center={[node.y, node.x]}
          radius={node.isCritical ? 8 : 5}
          color={node.isCritical ? "#DC2626" : "#1D4ED8"}
          fillColor={node.isCritical ? "#EF4444" : "#3B82F6"}
          fillOpacity={0.8}
          weight={2}
        >
          <Tooltip>{node.name}</Tooltip>
        </CircleMarker>
      ))}
    </MapContainer>
  );
}
