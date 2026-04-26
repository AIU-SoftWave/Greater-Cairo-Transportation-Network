using CairoTransportation.Services.Algorithms.Common.DTOs;

namespace CairoTransportation.Services.Routing.Contracts;

/// <summary>
/// Business service for traffic-aware routing.
/// Adjusts route planning based on time-of-day congestion patterns and real-time flow data.
/// </summary>
public interface ITimeVaryingDijkstraService
{
    /// <summary>
    /// Finds a traffic-aware shortest path for a specific time period (e.g., Morning Rush).
    /// </summary>
    /// <param name="fromNodeId">Starting location identifier.</param>
    /// <param name="toNodeId">Destination location identifier.</param>
    /// <param name="period">The time period to analyze (MORNING, EVENING, etc.).</param>
    Task<AlgorithmResponseDto<ShortestPathResultDto>> FindShortestPathAsync(string fromNodeId, string toNodeId, string period);
}
