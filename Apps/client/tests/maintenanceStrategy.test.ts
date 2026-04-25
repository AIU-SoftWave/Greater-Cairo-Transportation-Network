import { getMaintenancePlan } from "../src/services/maintenanceStrategy";
import { AlgorithmResponse, MaintenancePlanningResultDto } from "@/types";

jest.mock("../src/services/api", () => ({
  apiFetch: jest.fn(),
}));
const { apiFetch } = require("../src/services/api");

describe("maintenanceStrategy", () => {
  beforeEach(() => {
    apiFetch.mockClear();
  });

  describe("getMaintenancePlan", () => {
    it("should fetch maintenance plan", async () => {
      const mockResponse: AlgorithmResponse<MaintenancePlanningResultDto> = {
        algorithmName: "Maintenance Planning",
        success: true,
        message: "Plan generated",
        trace: { visitedNodes: 0, expandedNodes: 0, executionTimeMs: 50 },
        data: {
          budget: 1000000,
          totalCost: 500000,
          remainingBudget: 500000,
          totalPriorityScore: 100,
          selectedRoadCount: 10,
          totalCandidateRoads: 50,
          expectedConditionImprovement: 0.5,
          selectedRoads: [],
          notSelectedRoads: [],
        },
      };
      apiFetch.mockResolvedValue(mockResponse);

      const result = await getMaintenancePlan(1000000);

      expect(apiFetch).toHaveBeenCalledWith(
        "maintenance-strategy?budget=1000000",
      );
      expect(result).toEqual(mockResponse);
    });
  });
});
