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
    IGreedySignalOptimizer optimizer) : ITrafficSignalService
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
        List<SignalRoadCongestion> roads = await dbContext.TrafficFlows
            .AsNoTracking()
            .Where(tf => tf.Period == normalizedPeriod && tf.Road.IsExisting)
            .Select(tf => new SignalRoadCongestion(
                tf.Road.Id,
                tf.Road.FromLocation.Name,
                tf.Road.ToLocation.Name,
                tf.Road.ToLocation.Name,
                tf.Flow,
                tf.Road.Capacity,
                (double)tf.Flow / tf.Road.Capacity,
                tf.Road.FromLocation.IsCritical || tf.Road.ToLocation.IsCritical
            ))
            .ToListAsync();

        // 3. Run Greedy optimization to calculate signal timings and green light priority
        TrafficSignalResultDto data = optimizer.OptimizeSignals(roads, normalizedPeriod, topN, analyzeAllIntersections);

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
