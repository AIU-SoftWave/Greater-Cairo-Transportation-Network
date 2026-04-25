import {
  getAllRoads,
  getRoadById,
  getRoadsByFromLocation,
  getRoadMaintenance,
} from "../src/services/roadNetwork";
import { RoadFull, RoadMaintenance } from "@/types";

jest.mock("../src/services/api", () => ({
  apiFetch: jest.fn(),
}));
const { apiFetch } = require("../src/services/api");

describe("roadNetwork", () => {
  beforeEach(() => {
    apiFetch.mockClear();
  });

  describe("getAllRoads", () => {
    it("should fetch all roads", async () => {
      const mockRoads: RoadFull[] = [
        {
          id: 1,
          fromLocationId: "L1",
          toLocationId: "L2",
          distance: 10,
          capacity: 100,
          isExisting: true,
          isTwoWay: true,
        },
      ];
      apiFetch.mockResolvedValue(mockRoads);

      const result = await getAllRoads();

      expect(apiFetch).toHaveBeenCalledWith("road-network");
      expect(result).toEqual(mockRoads);
    });
  });

  describe("getRoadById", () => {
    it("should fetch a road by ID", async () => {
      const mockRoad: RoadFull = {
        id: 1,
        fromLocationId: "L1",
        toLocationId: "L2",
        distance: 10,
        capacity: 100,
        isExisting: true,
        isTwoWay: true,
      };
      apiFetch.mockResolvedValue(mockRoad);

      const result = await getRoadById(1);

      expect(apiFetch).toHaveBeenCalledWith("road-network/1");
      expect(result).toEqual(mockRoad);
    });
  });

  describe("getRoadsByFromLocation", () => {
    it("should fetch roads from a location", async () => {
      const mockRoads: RoadFull[] = [
        {
          id: 1,
          fromLocationId: "L1",
          toLocationId: "L2",
          distance: 10,
          capacity: 100,
          isExisting: true,
          isTwoWay: true,
        },
      ];
      apiFetch.mockResolvedValue(mockRoads);

      const result = await getRoadsByFromLocation("L1");

      expect(apiFetch).toHaveBeenCalledWith("road-network/from/L1");
      expect(result).toEqual(mockRoads);
    });
  });

  describe("getRoadMaintenance", () => {
    it("should fetch maintenance info for a road", async () => {
      const mockMaintenance: RoadMaintenance = {
        roadId: 1,
        priority: 5,
        estimatedCost: 10000,
      };
      apiFetch.mockResolvedValue(mockMaintenance);

      const result = await getRoadMaintenance(1);

      expect(apiFetch).toHaveBeenCalledWith("road-network/1/maintenance");
      expect(result).toEqual(mockMaintenance);
    });
  });
});
