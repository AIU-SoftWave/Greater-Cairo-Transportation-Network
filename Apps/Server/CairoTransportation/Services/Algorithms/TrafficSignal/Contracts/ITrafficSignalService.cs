using CairoTransportation.Services.Algorithms.Common.DTOs;
using CairoTransportation.Services.Algorithms.TrafficSignal.DTOs;

namespace CairoTransportation.Services.Algorithms.TrafficSignal.Contracts;

/// <summary>
/// Service for optimizing traffic signal timing using Greedy algorithm.
/// Prioritizes directions with highest congestion for immediate relief.
/// </summary>
public interface ITrafficSignalService
{
    /// <summary>
    /// Generates optimal traffic signal timing for congested roads.
    /// </summary>
    /// <param name="period">Time period (MORNING, AFTERNOON, EVENING, NIGHT)</param>
    /// <param name="topN">Number of highest-congestion roads to optimize (default 10)</param>
    /// <param name="analyzeAllIntersections">When true, ignores topN and evaluates all intersections with congestion data</param>
    /// <returns>Signal timing recommendations ordered by priority</returns>
    Task<AlgorithmResponseDto<TrafficSignalResultDto>> OptimizeSignalsAsync(
        string period,
        int topN,
        bool analyzeAllIntersections = false);
}
