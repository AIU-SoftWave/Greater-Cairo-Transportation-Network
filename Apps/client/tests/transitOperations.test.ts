import { getTransitSchedule } from "../src/services/transit/transitOperations";
import { AlgorithmResponse, TransitSchedulingResultDto } from "@/types";

jest.mock("../src/services/api", () => ({
  apiFetch: jest.fn(),
}));
const { apiFetch } = require("../src/services/api");

describe("transitOperations", () => {
  beforeEach(() => {
    apiFetch.mockClear();
  });

  describe("getTransitSchedule", () => {
    it("should fetch transit schedule", async () => {
      const mockResponse: AlgorithmResponse<TransitSchedulingResultDto> = {
        algorithmName: "Transit Scheduling",
        success: true,
        message: "Schedule generated",
        trace: { visitedNodes: 0, expandedNodes: 0, executionTimeMs: 60 },
        data: {
          totalVehicles: 50,
          assignedVehicles: 48,
          remainingVehicles: 2,
          totalDemand: 100000,
          estimatedPassengersServed: 95000,
          coverageRatio: 0.95,
          totalRoutes: 20,
          activeRoutes: 18,
          routeAllocations: [],
        },
      };
      apiFetch.mockResolvedValue(mockResponse);

      const result = await getTransitSchedule(50);

      expect(apiFetch).toHaveBeenCalledWith("transit-operations?vehicles=50");
      expect(result).toEqual(mockResponse);
    });
  });
});
