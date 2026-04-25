import { getSignalOptimization } from "../src/services/traffic/signalOptimization";
import { AlgorithmResponse, TrafficSignalResultDto } from "@/types";

jest.mock("../src/services/api", () => ({
  apiFetch: jest.fn(),
}));
const { apiFetch } = require("../src/services/api");

describe("signalOptimization", () => {
  beforeEach(() => {
    apiFetch.mockClear();
  });

  describe("getSignalOptimization", () => {
    it("should fetch signal optimization with default params", async () => {
      const mockResponse: AlgorithmResponse<TrafficSignalResultDto> = {
        algorithmName: "Traffic Signal Optimization",
        success: true,
        message: "Optimization complete",
        trace: { visitedNodes: 0, expandedNodes: 0, executionTimeMs: 40 },
        data: {
          period: "MORNING",
          roadsAnalyzed: 10,
          intersectionsAnalyzed: 5,
          intersectionsWithSignalRecommendations: 5,
          signalRecommendations: 10,
          totalCongestionScore: 8.5,
          estimatedWaitTimeReductionPercent: 25,
          signalTimings: [],
        },
      };
      apiFetch.mockResolvedValue(mockResponse);

      const result = await getSignalOptimization();

      expect(apiFetch).toHaveBeenCalledWith(
        "signal-optimization?period=MORNING&topN=10&analyzeAllIntersections=false",
      );
      expect(result).toEqual(mockResponse);
    });

    it("should fetch signal optimization with custom params", async () => {
      const mockResponse: AlgorithmResponse<TrafficSignalResultDto> = {
        algorithmName: "Traffic Signal Optimization",
        success: true,
        message: "Optimization complete",
        trace: { visitedNodes: 0, expandedNodes: 0, executionTimeMs: 40 },
        data: {
          period: "EVENING",
          roadsAnalyzed: 20,
          intersectionsAnalyzed: 10,
          intersectionsWithSignalRecommendations: 10,
          signalRecommendations: 20,
          totalCongestionScore: 12.0,
          estimatedWaitTimeReductionPercent: 30,
          signalTimings: [],
        },
      };
      apiFetch.mockResolvedValue(mockResponse);

      const result = await getSignalOptimization("EVENING", 20, true);

      expect(apiFetch).toHaveBeenCalledWith(
        "signal-optimization?period=EVENING&topN=20&analyzeAllIntersections=true",
      );
      expect(result).toEqual(mockResponse);
    });
  });
});
