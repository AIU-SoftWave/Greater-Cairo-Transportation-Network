using CairoTransportation.Data;
using CairoTransportation.Models;
using CairoTransportation.Services.Algorithms.Common.DTOs;
using CairoTransportation.Services.Algorithms.Common.Instrumentation;
using CairoTransportation.Services.Algorithms.TrafficSignal.Contracts;
using CairoTransportation.Services.Algorithms.TrafficSignal.DTOs;
using Microsoft.EntityFrameworkCore;

namespace CairoTransportation.Services.Algorithms.TrafficSignal;

/// <summary>
/// Service for optimizing traffic signal timing using Greedy algorithm.
/// 
/// Greedy Strategy: Prioritize roads with highest congestion ratio (flow/capacity)
/// for immediate traffic relief. Highest congestion gets longest green light.
/// </summary>
public class TrafficSignalService(TransportationDbContext dbContext) : ITrafficSignalService
{
    /// <summary>
    /// Internal model for congestion analysis.
    /// </summary>
    private record RoadCongestion(
        long RoadId,
        string? FromLocation,
        string? ToLocation,
        string IntersectionLocationId,
        int Flow,
        int Capacity,
        double CongestionRatio);

    public async Task<AlgorithmResponseDto<TrafficSignalResultDto>> OptimizeSignalsAsync(
        string period,
        int topN,
        bool analyzeAllIntersections = false)
    {
        AlgorithmExecutionMetrics metrics = new();

        // Validate period against DB-configured multipliers.
        string normalizedPeriod = period.Trim().ToUpperInvariant();
        bool periodExists = await dbContext.TrafficPeriodMultipliers
            .AsNoTracking()
            .AnyAsync(x => x.Period == normalizedPeriod);
        if (!periodExists)
        {
            List<string> validPeriods = await dbContext.TrafficPeriodMultipliers
                .AsNoTracking()
                .OrderBy(x => x.Period)
                .Select(x => x.Period)
                .ToListAsync();

            return CreateFailureResponse(
                $"Invalid period '{period}'. Valid periods from database: {string.Join(", ", validPeriods)}",
                metrics,
                period);
        }

        // Cap topN at reasonable range
        int effectiveTopN = Math.Clamp(topN, 1, 50);

        // Load and aggregate traffic data for the period (one row per road).
        List<RoadCongestion> analyzedRoads = await LoadRoadCongestionAsync(normalizedPeriod);
        metrics.MarkExpanded();

        var congestedRoads = analyzedRoads
            .Where(r => r.CongestionRatio > 0.5)
            .ToList();

        if (congestedRoads.Count == 0)
        {
            return CreateEmptyResponse(normalizedPeriod, analyzedRoads, metrics);
        }

        // GREEDY: Sort by congestion ratio descending (highest congestion first)
        List<RoadCongestion> prioritizedRoads = analyzeAllIntersections
            ? congestedRoads.OrderByDescending(r => r.CongestionRatio).ToList()
            : congestedRoads
                .OrderByDescending(r => r.CongestionRatio)
                .Take(effectiveTopN)
                .ToList();

        metrics.MarkExpanded();

        // Generate signal timings based on congestion priority
        List<SignalTimingDto> signalTimings = GenerateSignalTimings(prioritizedRoads);

        int intersectionsAnalyzed = analyzedRoads
            .Select(r => r.IntersectionLocationId)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .Count();

        int intersectionsWithSignals = prioritizedRoads
            .Select(r => r.IntersectionLocationId)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .Count();

        // Build result
        TrafficSignalResultDto result = BuildResult(
            normalizedPeriod,
            analyzedRoads.Count,
            intersectionsAnalyzed,
            intersectionsWithSignals,
            signalTimings);
        string message = signalTimings.Count > 0
            ? analyzeAllIntersections
                ? $"Traffic signals optimized for {normalizedPeriod}: analyzed all intersections and generated {signalTimings.Count} road-level timings."
                : $"Traffic signals optimized for {normalizedPeriod}: {signalTimings.Count} roads prioritized by congestion."
            : "No signal optimizations needed - traffic flow within normal limits.";

        return CreateSuccessResponse(result, message, metrics);
    }

    private async Task<List<RoadCongestion>> LoadRoadCongestionAsync(string period)
    {
        List<RoadCongestion> roads = await dbContext.TrafficFlows
            .AsNoTracking()
            .Where(tf => tf.Period == period)
            .Where(tf => tf.Road.IsExisting)
            .Where(tf => tf.PeriodMultiplier.Period == period)
            .GroupBy(tf => new
            {
                RoadId = tf.Road.Id,
                FromLocationName = tf.Road.FromLocation.Name,
                ToLocationName = tf.Road.ToLocation.Name,
                IntersectionLocationId = tf.Road.ToLocationId,
                tf.Road.Capacity
            })
            .Select(g => new RoadCongestion(
                g.Key.RoadId,
                g.Key.FromLocationName,
                g.Key.ToLocationName,
                g.Key.IntersectionLocationId,
                g.Sum(tf => tf.Flow),
                g.Key.Capacity,
                (double)g.Sum(tf => tf.Flow) / g.Key.Capacity))
            .ToListAsync();

        return roads;
    }

    private static List<SignalTimingDto> GenerateSignalTimings(List<RoadCongestion> prioritizedRoads)
    {
        List<SignalTimingDto> timings = [];
        int rank = 1;

        foreach (RoadCongestion road in prioritizedRoads)
        {
            // Greedy allocation: higher congestion = longer green light
            // Base green: 30 seconds, max additional: 90 seconds based on congestion
            int additionalGreen = (int)((road.CongestionRatio - 0.5) * 180);
            int greenDuration = 30 + Math.Clamp(additionalGreen, 0, 90);

            // Cycle time: 60 to 120 seconds based on congestion
            int cycleTime = Math.Min(60 + rank * 5, 120);

            string reason = road.CongestionRatio > 1.0
                ? $"Critical congestion ({road.CongestionRatio:P0} of capacity) - maximum green time allocated"
                : $"High congestion ({road.CongestionRatio:P0} of capacity) - priority green time";

            timings.Add(new SignalTimingDto
            {
                RoadId = road.RoadId,
                FromLocation = road.FromLocation,
                ToLocation = road.ToLocation,
                CurrentFlow = road.Flow,
                RoadCapacity = road.Capacity,
                CongestionRatio = road.CongestionRatio,
                PriorityRank = rank++,
                RecommendedGreenDurationSeconds = greenDuration,
                RecommendedCycleTimeSeconds = cycleTime,
                Reason = reason
            });
        }

        return timings;
    }

    private static TrafficSignalResultDto BuildResult(
        string period,
        int roadsAnalyzed,
        int intersectionsAnalyzed,
        int intersectionsWithSignalRecommendations,
        List<SignalTimingDto> signalTimings)
    {
        double totalCongestion = signalTimings.Sum(s => s.CongestionRatio);

        // Estimate wait time reduction: each optimized signal reduces wait by ~10-20%
        double estimatedReduction = signalTimings.Count > 0
            ? signalTimings.Average(s => Math.Min((s.CongestionRatio - 0.5) * 30, 20))
            : 0;

        return new TrafficSignalResultDto
        {
            Period = period,
            RoadsAnalyzed = roadsAnalyzed,
            IntersectionsAnalyzed = intersectionsAnalyzed,
            IntersectionsWithSignalRecommendations = intersectionsWithSignalRecommendations,
            SignalRecommendations = signalTimings.Count,
            TotalCongestionScore = totalCongestion,
            EstimatedWaitTimeReductionPercent = estimatedReduction,
            SignalTimings = signalTimings
        };
    }

    private static AlgorithmResponseDto<TrafficSignalResultDto> CreateEmptyResponse(
        string period,
        List<RoadCongestion> analyzedRoads,
        AlgorithmExecutionMetrics metrics) =>
        CreateSuccessResponse(
            new TrafficSignalResultDto
            {
                Period = period,
                RoadsAnalyzed = analyzedRoads.Count,
                IntersectionsAnalyzed = analyzedRoads
                    .Select(r => r.IntersectionLocationId)
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .Count(),
                IntersectionsWithSignalRecommendations = 0,
                SignalRecommendations = 0,
                TotalCongestionScore = 0,
                EstimatedWaitTimeReductionPercent = 0,
                SignalTimings = []
            },
            $"No congested roads found for period '{period}'.",
            metrics);

    private static AlgorithmResponseDto<TrafficSignalResultDto> CreateSuccessResponse(
        TrafficSignalResultDto result, string message, AlgorithmExecutionMetrics metrics) =>
        new()
        {
            AlgorithmName = "Traffic Signal Optimization (Greedy)",
            Success = true,
            Message = message,
            Trace = metrics.Complete(),
            Data = result
        };

    private static AlgorithmResponseDto<TrafficSignalResultDto> CreateFailureResponse(
        string message, AlgorithmExecutionMetrics metrics, string period) =>
        new()
        {
            AlgorithmName = "Traffic Signal Optimization (Greedy)",
            Success = false,
            Message = message,
            Trace = metrics.Complete(),
            Data = new TrafficSignalResultDto { Period = period }
        };
}
