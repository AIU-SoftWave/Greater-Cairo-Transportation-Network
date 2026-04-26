using CairoTransportation.Services.Algorithms.Common.DTOs;
using CairoTransportation.Services.TrafficControl.Contracts;
using CairoTransportation.Services.Algorithms.TrafficSignal.DTOs;
using CairoTransportation.Services;
using Microsoft.AspNetCore.Mvc;

namespace CairoTransportation.Controllers;

/// <summary>
/// Provides traffic signal timing optimization using Greedy algorithm.
/// Prioritizes directions with highest congestion for immediate relief.
/// </summary>
[ApiController]
[Route("api/signal-optimization")]
public class TrafficSignalController(ITrafficSignalService signalService, ITrafficService trafficService) : ControllerBase
{
    /// <summary>
    /// Generates optimal traffic signal timing for congested roads.
    /// </summary>
    /// <remarks>
    /// Uses Greedy algorithm to prioritize roads with highest congestion ratio.
    /// Higher congestion = longer green light duration.
    /// </remarks>
    /// <param name="period">Time period configured in traffic_period_multipliers.</param>
    /// <param name="topN">Number of highest-congestion roads to optimize (1-50, default 10)</param>
    /// <param name="analyzeAllIntersections">When true, analyzes all intersections and ignores topN</param>
    /// <returns>Signal timing recommendations ordered by priority.</returns>
    [HttpGet]
    public async Task<IActionResult> GetSignalOptimization(
        [FromQuery] string period = "MORNING",
        [FromQuery] int topN = 10,
        [FromQuery] bool analyzeAllIntersections = false)
    {
        // Validate period against DB-configured multipliers only.
        string normalizedPeriod = period.Trim().ToUpperInvariant();

        if (await trafficService.GetPeriodMultiplierAsync(normalizedPeriod) is null)
        {
            var validPeriods = (await trafficService.GetPeriodMultipliersAsync())
                .Select(x => x.Period)
                .OrderBy(x => x)
                .ToList();

            return BadRequest(new AlgorithmResponseDto<TrafficSignalResultDto>
            {
                AlgorithmName = "Traffic Signal Optimization",
                Success = false,
                Message = $"Invalid period '{period}'. Valid values from database: {string.Join(", ", validPeriods)}",
                Data = new TrafficSignalResultDto
                {
                    Period = period,
                    Summary = new SignalSummary
                    {
                        RoadsAnalyzed = 0,
                        IntersectionsAnalyzed = 0,
                        OptimizedIntersections = 0,
                        EstimatedWaitTimeReductionPercent = 0
                    },
                    Intersections = new List<IntersectionSignalPlan>()
                }
            });
        }

        // Validate topN range
        if (!analyzeAllIntersections && (topN < 1 || topN > 50))
        {
            return BadRequest(new AlgorithmResponseDto<TrafficSignalResultDto>
            {
                AlgorithmName = "Traffic Signal Optimization",
                Success = false,
                Message = "topN must be between 1 and 50.",
                Data = new TrafficSignalResultDto
                {
                    Period = period,
                    Summary = new SignalSummary
                    {
                        RoadsAnalyzed = 0,
                        IntersectionsAnalyzed = 0,
                        OptimizedIntersections = 0,
                        EstimatedWaitTimeReductionPercent = 0
                    },
                    Intersections = new List<IntersectionSignalPlan>()
                }
            });
        }

        AlgorithmResponseDto<TrafficSignalResultDto> result = await signalService.OptimizeSignalsAsync(
            normalizedPeriod,
            topN,
            analyzeAllIntersections);
        return result.Success ? Ok(result) : BadRequest(result);
    }
}

