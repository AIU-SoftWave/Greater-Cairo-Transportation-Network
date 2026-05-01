using CairoTransportation.Data;
using CairoTransportation.Modules.TrafficControl.Services.TrafficSignal.Contracts;
using CairoTransportation.Modules.TrafficControl.Services.TrafficSignal.DTOs;
using CairoTransportation.Utils.Helpers.Common.DTOs;
using CairoTransportation.Utils.Helpers.Common.Instrumentation;
using Microsoft.EntityFrameworkCore;

namespace CairoTransportation.Modules.TrafficControl.Services.TrafficSignal;

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

        // Generate intersection signal plans based on congestion priority
        List<IntersectionSignalPlan> intersectionPlans = GenerateIntersectionSignalPlans(prioritizedRoads);

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
            intersectionPlans);
        string message = intersectionPlans.Count > 0
            ? analyzeAllIntersections
                ? $"Traffic signals optimized for {normalizedPeriod}: analyzed all intersections and generated {intersectionPlans.Count} intersection plans."
                : $"Traffic signals optimized for {normalizedPeriod}: {intersectionPlans.Count} intersections prioritized by congestion."
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
                IntersectionLocationId = tf.Road.ToLocation.Name,
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

    private static List<IntersectionSignalPlan> GenerateIntersectionSignalPlans(List<RoadCongestion> prioritizedRoads)
    {
        // Group roads by intersection (ToLocation)
        var intersectionGroups = prioritizedRoads
            .GroupBy(r => r.IntersectionLocationId)
            .ToDictionary(g => g.Key, g => g.ToList());

        var plans = new List<IntersectionSignalPlan>();

        foreach (KeyValuePair<string, List<RoadCongestion>> kvp in intersectionGroups)
        {
            string intersectionId = kvp.Key;
            List<RoadCongestion> roads = kvp.Value;

            // Sort roads by congestion ratio descending
            var sortedRoads = roads.OrderByDescending(r => r.CongestionRatio).ToList();

            // Calculate cycle time based on total congestion at this intersection
            // Higher total congestion = longer cycle time (60-120 seconds)
            double totalCongestion = sortedRoads.Sum(r => r.CongestionRatio);
            int cycleTime = Math.Clamp(60 + (int)(totalCongestion * 10), 60, 120);

            // Calculate green time allocation based on congestion priority
            // Higher congestion = proportionally more green time
            List<(RoadCongestion road, int greenTime)> greenTimes = new List<(RoadCongestion, int)>();
            int rank = 1;

            foreach (RoadCongestion road in sortedRoads)
            {
                // Base green time proportional to congestion ratio
                // Minimum 10 seconds, scales with congestion
                double congestionWeight = road.CongestionRatio / totalCongestion;
                int rawGreenTime = (int)(cycleTime * congestionWeight);

                // Ensure minimum green time (10s) and cap at reasonable max
                int greenTime = Math.Clamp(rawGreenTime, 10, cycleTime / 2);

                greenTimes.Add((road, greenTime));
                rank++;
            }

            // Normalize green times to sum exactly to cycle time
            int totalGreen = greenTimes.Sum(g => g.greenTime);
            if (totalGreen != cycleTime)
            {
                // Adjust proportionally
                for (int i = 0; i < greenTimes.Count; i++)
                {
                    (RoadCongestion road, int originalGreen) = greenTimes[i];
                    double ratio = (double)originalGreen / totalGreen;
                    int adjustedGreen = (int)(cycleTime * ratio);
                    greenTimes[i] = (road, adjustedGreen);
                }

                // Handle rounding errors by adding/subtracting from the highest priority road
                int finalTotal = greenTimes.Sum(g => g.greenTime);
                int diff = cycleTime - finalTotal;
                if (diff != 0)
                {
                    (RoadCongestion road, int green) = greenTimes[0];
                    greenTimes[0] = (road, green + diff);
                }
            }

            // Create signal phases
            var phases = new List<SignalPhaseDto>();
            int priority = 1;

            foreach ((RoadCongestion road, int greenTime) in greenTimes)
            {
                phases.Add(new SignalPhaseDto
                {
                    From = road.FromLocation ?? "Unknown",
                    To = road.ToLocation ?? "Unknown",
                    CongestionPercent = road.CongestionRatio * 100,
                    Priority = priority++,
                    GreenTimeSeconds = greenTime
                });
            }

            plans.Add(new IntersectionSignalPlan
            {
                Name = intersectionId,
                CycleTimeSeconds = cycleTime,
                Roads = phases
            });
        }

        return plans;
    }

    private static TrafficSignalResultDto BuildResult(
        string period,
        int roadsAnalyzed,
        int intersectionsAnalyzed,
        int intersectionsWithSignalRecommendations,
        List<IntersectionSignalPlan> intersectionPlans)
    {
        // Estimate wait time reduction based on total congestion and number of optimized intersections
        double totalCongestion = intersectionPlans
            .Sum(p => p.Roads.Sum(r => r.CongestionPercent) / 100);

        double estimatedReduction = intersectionPlans.Count > 0
            ? Math.Min(totalCongestion * 2, 15) // Cap at 15% reduction
            : 0;

        return new TrafficSignalResultDto
        {
            Period = period,
            Summary = new SignalSummary
            {
                RoadsAnalyzed = roadsAnalyzed,
                IntersectionsAnalyzed = intersectionsAnalyzed,
                OptimizedIntersections = intersectionsWithSignalRecommendations,
                EstimatedWaitTimeReductionPercent = estimatedReduction
            },
            Intersections = intersectionPlans
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
                Summary = new SignalSummary
                {
                    RoadsAnalyzed = analyzedRoads.Count,
                    IntersectionsAnalyzed = analyzedRoads
                        .Select(r => r.IntersectionLocationId)
                        .Distinct(StringComparer.OrdinalIgnoreCase)
                        .Count(),
                    OptimizedIntersections = 0,
                    EstimatedWaitTimeReductionPercent = 0
                },
                Intersections = new List<IntersectionSignalPlan>()
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

