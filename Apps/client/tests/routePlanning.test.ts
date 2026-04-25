import {
  getShortestPath,
  getTimeVaryingShortestPath,
} from "../src/services/routes/routePlanning";
import { AlgorithmResponse, ShortestPathResultDto } from "@/types";

jest.mock("../src/services/api", () => ({
  apiFetch: jest.fn(),
}));
const { apiFetch } = require("../src/services/api");

describe("routePlanning", () => {
  beforeEach(() => {
    apiFetch.mockClear();
  });

  describe("getShortestPath", () => {
    it("should fetch shortest path using Dijkstra", async () => {
      const mockResponse: AlgorithmResponse<ShortestPathResultDto> = {
        algorithmName: "Dijkstra",
        success: true,
        message: "Path found",
        trace: { visitedNodes: 10, expandedNodes: 5, executionTimeMs: 100 },
        data: {
          fromNodeId: "L1",
          toNodeId: "L2",
          found: true,
          totalDistance: 10,
          pathNodes: [],
          pathRoads: [],
        },
      };
      apiFetch.mockResolvedValue(mockResponse);

      const result = await getShortestPath("L1", "L2");

      expect(apiFetch).toHaveBeenCalledWith(
        "route-planning/shortest-path?from=L1&to=L2",
      );
      expect(result).toEqual(mockResponse);
    });
  });

  describe("getTimeVaryingShortestPath", () => {
    it("should fetch time-varying shortest path", async () => {
      const mockResponse: AlgorithmResponse<ShortestPathResultDto> = {
        algorithmName: "Time-Varying Dijkstra",
        success: true,
        message: "Path found",
        trace: { visitedNodes: 10, expandedNodes: 5, executionTimeMs: 100 },
        data: {
          fromNodeId: "L1",
          toNodeId: "L2",
          found: true,
          totalDistance: 10,
          pathNodes: [],
          pathRoads: [],
        },
      };
      apiFetch.mockResolvedValue(mockResponse);

      const result = await getTimeVaryingShortestPath("L1", "L2", "MORNING");

      expect(apiFetch).toHaveBeenCalledWith(
        "route-planning/time-route?from=L1&to=L2&period=MORNING",
      );
      expect(result).toEqual(mockResponse);
    });
  });
});
