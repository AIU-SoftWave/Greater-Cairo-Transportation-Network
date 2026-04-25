import { fetchNetworkTopology } from "../src/services/network/networkTopology";
import { NetworkTopologyData } from "@/types";

jest.mock("../src/services/api", () => ({
  apiFetch: jest.fn(),
}));
const { apiFetch } = require("../src/services/api");

describe("networkTopology", () => {
  beforeEach(() => {
    apiFetch.mockClear();
  });
  describe("fetchNetworkTopology", () => {
    it("should fetch network topology", async () => {
      const mockTopology: NetworkTopologyData = {
        nodes: [],
        edges: [],
        adjacencyList: {},
        nodeIndex: {},
        edgeIndex: {},
      };
      apiFetch.mockResolvedValue(mockTopology);

      const result = await fetchNetworkTopology();

      expect(apiFetch).toHaveBeenCalledWith("network-topology");
      expect(result).toEqual(mockTopology);
    });
  });
});
