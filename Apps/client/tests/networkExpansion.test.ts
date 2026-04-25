import { getCheapestNetwork } from "../src/services/networkExpansion";
import { AlgorithmResponse, MstResultDto } from "@/types";

jest.mock("../src/services/api", () => ({
  apiFetch: jest.fn(),
}));
const { apiFetch } = require("../src/services/api");

describe("networkExpansion", () => {
  beforeEach(() => {
    apiFetch.mockClear();
  });

  describe("getCheapestNetwork", () => {
    it("should fetch MST network expansion", async () => {
      const mockResponse: AlgorithmResponse<MstResultDto> = {
        algorithmName: "MST",
        success: true,
        message: "Network built",
        trace: { visitedNodes: 20, expandedNodes: 15, executionTimeMs: 150 },
        data: {
          connected: true,
          totalConstructionCost: 1000000,
          totalNodes: 20,
          selectedRoadCount: 19,
          nodes: [],
          selectedRoads: [],
        },
      };
      apiFetch.mockResolvedValue(mockResponse);

      const result = await getCheapestNetwork();

      expect(apiFetch).toHaveBeenCalledWith("network-expansion");
      expect(result).toEqual(mockResponse);
    });
  });
});
