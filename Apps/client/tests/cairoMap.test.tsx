import React from "react";
import { render, screen, waitFor } from "@testing-library/react";
import type { NetworkTopologyData } from "@/types";

jest.mock("../src/services/network/networkTopology", () => ({
  fetchNetworkTopology: jest.fn(),
}));

// Leaflet requires browser APIs not available in jsdom – mock the entire module.
jest.mock("react-leaflet", () => ({
  MapContainer: ({ children }: { children: React.ReactNode }) => (
    <div data-testid="map-container">{children}</div>
  ),
  TileLayer: () => <div data-testid="tile-layer" />,
  CircleMarker: ({ children }: { children: React.ReactNode }) => (
    <div data-testid="circle-marker">{children}</div>
  ),
  Polyline: () => <div data-testid="polyline" />,
  Tooltip: ({ children }: { children: React.ReactNode }) => (
    <span>{children}</span>
  ),
}));

jest.mock("leaflet/dist/leaflet.css", () => ({}));

import CairoMap from "../src/components/CairoMap";
const { fetchNetworkTopology } = require("../src/services/network/networkTopology");

const mockTopology: NetworkTopologyData = {
  nodes: [
    { id: "1", name: "Maadi", x: 31.25, y: 29.96, isCritical: false },
    { id: "2", name: "Nasr City", x: 31.34, y: 30.06, isCritical: true },
  ],
  edges: [{ fromNodeId: "1", toNodeId: "2", distance: 15 }],
  adjacencyList: {},
  nodeIndex: {
    "1": { id: "1", name: "Maadi", x: 31.25, y: 29.96, isCritical: false },
    "2": { id: "2", name: "Nasr City", x: 31.34, y: 30.06, isCritical: true },
  },
  edgeIndex: {},
};

describe("CairoMap", () => {
  beforeEach(() => {
    fetchNetworkTopology.mockClear();
  });

  it("shows a loading indicator while fetching", () => {
    fetchNetworkTopology.mockReturnValue(new Promise(() => {}));
    render(<CairoMap />);
    expect(screen.getByText(/loading map/i)).toBeInTheDocument();
  });

  it("renders the map container with nodes and roads after loading", async () => {
    fetchNetworkTopology.mockResolvedValue(mockTopology);
    render(<CairoMap />);

    await waitFor(() =>
      expect(screen.getByTestId("map-container")).toBeInTheDocument()
    );

    // One polyline per edge
    expect(screen.getAllByTestId("polyline")).toHaveLength(1);

    // One circle marker per node
    expect(screen.getAllByTestId("circle-marker")).toHaveLength(2);

    // Tooltips show node names
    expect(screen.getByText("Maadi")).toBeInTheDocument();
    expect(screen.getByText("Nasr City")).toBeInTheDocument();
  });

  it("shows an error message when fetching fails", async () => {
    fetchNetworkTopology.mockRejectedValue(new Error("Network error"));
    render(<CairoMap />);

    await waitFor(() =>
      expect(screen.getByText(/failed to load network data/i)).toBeInTheDocument()
    );
  });
});
