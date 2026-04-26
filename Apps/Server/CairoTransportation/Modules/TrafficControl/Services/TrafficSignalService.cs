using CairoTransportation.Algorithms.Greedy.Contracts;
using CairoTransportation.Data;
using CairoTransportation.Services.Algorithms.Common.DTOs;
using CairoTransportation.Services.Algorithms.Common.Instrumentation;
using CairoTransportation.Services.Algorithms.TrafficSignal.DTOs;
using CairoTransportation.Services.TrafficControl.Contracts;
using Microsoft.EntityFrameworkCore;

namespace CairoTransportation.Services.TrafficControl;

public class TrafficSignalService(
    TransportationDbContext dbContext, 
    IGreedySignalOptimizer optimizer,
    ISimulationService simulationService) : ITrafficSignalService
{
    public async Task<AlgorithmResponseDto<TrafficSignalResultDto>> OptimizeSignalsAsync(string period, int topN, bool analyzeAllIntersections = false)
    {
        var metrics = new AlgorithmExecutionMetrics();
        string normalizedPeriod = period.Trim().ToUpperInvariant();

        // 1. Validate the requested time period against the database
        if (!await dbContext.TrafficPeriodMultipliers.AsNoTracking().AnyAsync(x => x.Period == normalizedPeriod))
        {
            return new AlgorithmResponseDto<TrafficSignalResultDto> { Success = false, Message = $"Invalid period '{period}'." };
        }

        // 2. Fetch traffic flow data for the specific time period
        var trafficFlows = await dbContext.TrafficFlows
            .AsNoTracking()
            .Where(tf => tf.Period == normalizedPeriod && tf.Road.IsExisting)
            .Select(tf => new { 
                tf.RoadId, 
                FromName = tf.Road.FromLocation.Name, 
                ToName = tf.Road.ToLocation.Name, 
                tf.Flow, 
                tf.Road.Capacity, 
                FromCritical = tf.Road.FromLocation.IsCritical, 
                ToCritical = tf.Road.ToLocation.IsCritical 
            })
            .ToListAsync();

        var roads = new List<SignalRoadCongestion>();
        foreach (var tf in trafficFlows)
        {
            bool isPreempted = await simulationService.IsPreemptedAsync(tf.RoadId);
            roads.Add(new SignalRoadCongestion(
                tf.RoadId,
                tf.FromName,
                tf.ToName,
                tf.ToName,
                tf.Flow,
                tf.Capacity,
                (double)tf.Flow / tf.Capacity,
                tf.FromCritical || tf.ToCritical || isPreempted
            ) { IsEmergencyRoute = isPreempted });
        }

        // 3. Run Greedy optimization to calculate signal timings and green light priority
        TrafficSignalResultDto data = optimizer.OptimizeSignals(roads, normalizedPeriod, topN, analyzeAllIntersections);

        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        stopwatch.Stop();
        simulationService.RecordMetrics("Greedy Signal Optimizer", stopwatch.ElapsedMilliseconds, roads.Count, data.Intersections.Count);

        return new AlgorithmResponseDto<TrafficSignalResultDto>
        {
            AlgorithmName = "Greedy Signal Optimizer",
            Success = true,
            Message = data.Intersections.Count > 0 ? $"Optimized {data.Intersections.Count} intersections." : "No optimizations needed.",
            Trace = metrics.Complete(),
            Data = data
        };
    }
}
