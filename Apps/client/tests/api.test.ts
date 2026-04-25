import { apiFetch } from "../src/services/api";

// Mock the global fetch
global.fetch = jest.fn();

describe("apiFetch", () => {
  beforeEach(() => {
    (global.fetch as jest.Mock).mockClear();
  });

  it("should make a successful API call", async () => {
    const mockData = { test: "data" };
    (global.fetch as jest.Mock).mockResolvedValueOnce({
      ok: true,
      json: async () => mockData,
    });

    const result = await apiFetch("test-endpoint");
    const baseUrl = process.env.NEXT_PUBLIC_API_BASE_URL || "http://localhost:8080";

    expect(fetch).toHaveBeenCalledWith(
      `${baseUrl}/api/test-endpoint`,
      {
        headers: { "Content-Type": "application/json" },
      },
    );
    expect(result).toEqual(mockData);
  });

  it("should handle HTTP errors", async () => {
    (global.fetch as jest.Mock).mockResolvedValueOnce({
      ok: false,
      status: 404,
      text: async () => "Error message",
    });

    await expect(apiFetch("test-endpoint")).rejects.toThrow(
      "API Error 404: Error message",
    );
  });

  it("should pass custom options", async () => {
    const mockData = { test: "data" };
    (global.fetch as jest.Mock).mockResolvedValueOnce({
      ok: true,
      json: async () => mockData,
    });

    await apiFetch("test-endpoint", { method: "POST" });
    const baseUrl = process.env.NEXT_PUBLIC_API_BASE_URL || "http://localhost:8080";

    expect(fetch).toHaveBeenCalledWith(
      `${baseUrl}/api/test-endpoint`,
      {
        headers: { "Content-Type": "application/json" },
        method: "POST",
      },
    );
  });
});
