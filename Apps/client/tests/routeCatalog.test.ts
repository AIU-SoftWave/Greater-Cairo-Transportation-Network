import {
  getAllRoutes,
  getRouteById,
  getRouteStops,
} from "../src/services/routes/routeCatalog";
import { TransportRoute, RouteStop } from "@/types";

jest.mock("../src/services/api", () => ({
  apiFetch: jest.fn(),
}));
const { apiFetch } = require("../src/services/api");

describe("routeCatalog", () => {
  beforeEach(() => {
    apiFetch.mockClear();
  });

  describe("getAllRoutes", () => {
    it("should fetch all routes", async () => {
      const mockRoutes: TransportRoute[] = [
        {
          id: "R1",
          type: "METRO",
          dailyPassengers: 1000,
          vehiclesAssigned: 5,
          capacityPerUnit: 50,
        },
      ];
      apiFetch.mockResolvedValue(mockRoutes);

      const result = await getAllRoutes();

      expect(apiFetch).toHaveBeenCalledWith("route-catalog");
      expect(result).toEqual(mockRoutes);
    });
  });

  describe("getRouteById", () => {
    it("should fetch a route by ID", async () => {
      const mockRoute: TransportRoute = {
        id: "R1",
        type: "METRO",
        dailyPassengers: 1000,
        vehiclesAssigned: 5,
        capacityPerUnit: 50,
      };
      apiFetch.mockResolvedValue(mockRoute);

      const result = await getRouteById("R1");

      expect(apiFetch).toHaveBeenCalledWith("route-catalog/R1");
      expect(result).toEqual(mockRoute);
    });
  });

  describe("getRouteStops", () => {
    it("should fetch stops for a route", async () => {
      const mockStops: RouteStop[] = [
        { routeId: "R1", locationId: "L1", stopOrder: 1 },
        { routeId: "R1", locationId: "L2", stopOrder: 2 },
      ];
      apiFetch.mockResolvedValue(mockStops);

      const result = await getRouteStops("R1");

      expect(apiFetch).toHaveBeenCalledWith("route-catalog/R1/stops");
      expect(result).toEqual(mockStops);
    });
  });
});
