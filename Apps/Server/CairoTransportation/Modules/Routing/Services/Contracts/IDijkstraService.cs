using CairoTransportation.Utils.Helpers.Common.DTOs;

namespace CairoTransportation.Modules.Routing.Services.Contracts;

/// <summary>
/// Business service for shortest-path route planning using Dijkstra's algorithm.
/// Coordinates data fetching, caching, and execution metrics.
/// </summary>
public interface IDijkstraService
{
    /// <summary>
    /// Orchestrates the search for the shortest path between two neighborhoods.
    /// </summary>
    Task<AlgorithmResponseDto<ShortestPathResultDto>> FindShortestPathAsync(string fromNodeId, string toNodeId);
}
