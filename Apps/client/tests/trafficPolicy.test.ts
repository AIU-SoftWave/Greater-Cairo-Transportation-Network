import {
  getAllPeriodMultipliers,
  getPeriodMultiplier,
} from "../src/services/trafficPolicy";
import { TrafficPeriodMultiplier } from "@/types";

jest.mock("../src/services/api", () => ({
  apiFetch: jest.fn(),
}));
const { apiFetch } = require("../src/services/api");

describe("trafficPolicy", () => {
  beforeEach(() => {
    apiFetch.mockClear();
  });

  describe("getAllPeriodMultipliers", () => {
    it("should fetch all period multipliers", async () => {
      const mockMultipliers: TrafficPeriodMultiplier[] = [
        { period: "MORNING", multiplier: 1.5 },
        { period: "EVENING", multiplier: 1.8 },
        { period: "NIGHT", multiplier: 0.8 },
      ];
      apiFetch.mockResolvedValue(mockMultipliers);

      const result = await getAllPeriodMultipliers();

      expect(apiFetch).toHaveBeenCalledWith("traffic-policy");
      expect(result).toEqual(mockMultipliers);
    });
  });

  describe("getPeriodMultiplier", () => {
    it("should fetch a period multiplier by period", async () => {
      const mockMultiplier: TrafficPeriodMultiplier = {
        period: "MORNING",
        multiplier: 1.5,
      };
      apiFetch.mockResolvedValue(mockMultiplier);

      const result = await getPeriodMultiplier("MORNING");

      expect(apiFetch).toHaveBeenCalledWith("traffic-policy/MORNING");
      expect(result).toEqual(mockMultiplier);
    });
  });
});
