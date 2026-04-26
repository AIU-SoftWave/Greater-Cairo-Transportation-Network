"use client";

import { useEffect, useSyncExternalStore } from "react";
import type { Node, Road } from "@/types";

interface MapViewProps {
  nodes: Node[];
  edges: Road[];
}

// Fix Leaflet icon issues in Next.js
const fixLeafletIcons = () => {
  const L = require("leaflet");
  // eslint-disable-next-line @typescript-eslint/no-explicit-any
  const proto = L.Icon.Default.prototype as any;
  delete proto._getIconUrl;
  L.Icon.Default.mergeOptions({
    iconRetinaUrl:
      "https://unpkg.com/leaflet@1.9.4/dist/images/marker-icon-2x.png",
    iconUrl: "https://unpkg.com/leaflet@1.9.4/dist/images/marker-icon.png",
    shadowUrl: "https://unpkg.com/leaflet@1.9.4/dist/images/marker-shadow.png",
  });
};

// Simple store for hydration tracking
const getServerSnapshot = () => false;
const getClientSnapshot = () => true;
const subscribe = () => () => {};

export default function MapView({ nodes, edges }: MapViewProps) {
  const isClient = useSyncExternalStore(
    subscribe,
    getClientSnapshot,
    getServerSnapshot,
  );

  useEffect(() => {
    if (isClient) {
      fixLeafletIcons();
    }
  }, [isClient]);

  if (!isClient) {
    return <div className="h-full w-full bg-gray-100" />;
  }

  const {
    MapContainer,
    TileLayer,
    Marker,
    Popup,
    Polyline,
  } = require("react-leaflet");

  // Build node lookup for edge drawing
  const nodeLookup: Record<string, Node> = {};
  nodes.forEach((node) => {
    nodeLookup[node.id] = node;
  });

  // Create polyline positions for edges (lat = y, lng = x)
  const edgePositions = edges
    .map((edge) => {
      const from = nodeLookup[edge.fromNodeId];
      const to = nodeLookup[edge.toNodeId];
      if (!from || !to) return null;
      return [
        [from.y, from.x],
        [to.y, to.x],
      ];
    })
    .filter(Boolean);

  return (
    <MapContainer
      center={[30.0444, 31.2357]}
      zoom={12}
      className="h-full w-full"
    >
      <TileLayer
        attribution='&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors'
        url="https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png"
      />
      {edgePositions.map((positions, idx) => (
        <Polyline
          key={idx}
          positions={positions}
          color="#9ca3af"
          weight={3}
          opacity={0.8}
        />
      ))}
      {nodes.map((node) => (
        <Marker key={node.id} position={[node.y, node.x]}>
          <Popup>{node.name}</Popup>
        </Marker>
      ))}
    </MapContainer>
  );
}
