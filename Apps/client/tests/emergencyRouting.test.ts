import { getEmergencyRoute } from "../src/services/routes/emergencyRouting";
import { AlgorithmResponse, ShortestPathResultDto } from "@/types";

jest.mock("../src/services/api", () => ({
  apiFetch: jest.fn(),
}));
const { apiFetch } = require("../src/services/api");

describe("emergencyRouting", () => {
  beforeEach(() => {
    apiFetch.mockClear();
  });

  describe("getEmergencyRoute", () => {
    it("should fetch emergency route using A*", async () => {
      const mockResponse: AlgorithmResponse<ShortestPathResultDto> = {
        algorithmName: "A*",
        success: true,
        message: "Path found",
        trace: { visitedNodes: 8, expandedNodes: 4, executionTimeMs: 80 },
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

      const result = await getEmergencyRoute("L1", "L2");

      expect(apiFetch).toHaveBeenCalledWith("emergency-routing?from=L1&to=L2");
      expect(result).toEqual(mockResponse);
    });
  });
});
