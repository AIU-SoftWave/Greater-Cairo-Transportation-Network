import {
  getAllLocations,
  getLocationById,
} from "../src/services/locations/cityLocations";
import { Location } from "@/types";

// Mock apiFetch
jest.mock("../src/services/api", () => ({
  apiFetch: jest.fn(),
}));
const { apiFetch } = require("../src/services/api");

describe("cityLocations", () => {
  beforeEach(() => {
    apiFetch.mockClear();
  });

  describe("getAllLocations", () => {
    it("should fetch all locations", async () => {
      const mockLocations: Location[] = [
        {
          id: "L1",
          name: "Location 1",
          type: "NEIGHBORHOOD",
          x: 0,
          y: 0,
          isCritical: false,
        },
        {
          id: "L2",
          name: "Location 2",
          type: "FACILITY",
          x: 10,
          y: 10,
          isCritical: true,
        },
      ];
      apiFetch.mockResolvedValue(mockLocations);

      const result = await getAllLocations();

      expect(apiFetch).toHaveBeenCalledWith("city-locations");
      expect(result).toEqual(mockLocations);
    });
  });

  describe("getLocationById", () => {
    it("should fetch a location by ID", async () => {
      const mockLocation: Location = {
        id: "L1",
        name: "Location 1",
        type: "NEIGHBORHOOD",
        x: 0,
        y: 0,
        isCritical: false,
      };
      apiFetch.mockResolvedValue(mockLocation);

      const result = await getLocationById("L1");

      expect(apiFetch).toHaveBeenCalledWith("city-locations/L1");
      expect(result).toEqual(mockLocation);
    });
  });
});
