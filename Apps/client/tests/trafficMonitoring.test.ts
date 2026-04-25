import {
  getTrafficByRoadId,
  getTrafficByPeriod,
} from "../src/services/trafficMonitoring";
import { TrafficFlow } from "@/types";

jest.mock("../src/services/api", () => ({
  apiFetch: jest.fn(),
}));
const { apiFetch } = require("../src/services/api");

describe("trafficMonitoring", () => {
  beforeEach(() => {
    apiFetch.mockClear();
  });

  describe("getTrafficByRoadId", () => {
    it("should fetch traffic by road ID", async () => {
      const mockTraffic: TrafficFlow[] = [
        { id: 1, roadId: 1, period: "MORNING", flow: 500 },
        { id: 2, roadId: 1, period: "EVENING", flow: 800 },
      ];
      apiFetch.mockResolvedValue(mockTraffic);

      const result = await getTrafficByRoadId(1);

      expect(apiFetch).toHaveBeenCalledWith("traffic-monitoring/road/1");
      expect(result).toEqual(mockTraffic);
    });
  });

  describe("getTrafficByPeriod", () => {
    it("should fetch traffic by period", async () => {
      const mockTraffic: TrafficFlow[] = [
        { id: 1, roadId: 1, period: "MORNING", flow: 500 },
        { id: 2, roadId: 2, period: "MORNING", flow: 600 },
      ];
      apiFetch.mockResolvedValue(mockTraffic);

      const result = await getTrafficByPeriod("MORNING");

      expect(apiFetch).toHaveBeenCalledWith(
        "traffic-monitoring/period/MORNING",
      );
      expect(result).toEqual(mockTraffic);
    });
  });
});
