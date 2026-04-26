using CairoTransportation.Services.Algorithms.Common.DTOs;
using CairoTransportation.Services.Algorithms.TrafficSignal.DTOs;

namespace CairoTransportation.Services.TrafficControl.Contracts;

/// <summary>
/// Business service for real-time traffic signal optimization.
/// Provides green-light timing recommendations for congested intersections.
/// </summary>
public interface ITrafficSignalService
{
    /// <summary>
    /// Optimizes signal cycles for intersections based on current traffic flow.
    /// Prioritizes congested roads and emergency routes using a Greedy approach.
    /// </summary>
    /// <param name="period">Traffic period (e.g., MORNING).</param>
    /// <param name="topN">Number of intersections to focus on.</param>
    /// <param name="analyzeAllIntersections">When true, analyzes the entire city network.</param>
    Task<AlgorithmResponseDto<TrafficSignalResultDto>> OptimizeSignalsAsync(string period, int topN, bool analyzeAllIntersections = false);
}
