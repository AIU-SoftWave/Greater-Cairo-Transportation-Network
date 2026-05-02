using CairoTransportation.Modules.Routing.Services.Contracts;
using CairoTransportation.Modules.Simulation.Services;
using CairoTransportation.Modules.TrafficControl.Models;
using CairoTransportation.Modules.TrafficControl.Services;
using CairoTransportation.Utils.Algorithms.ShortestPath.Contracts;
using CairoTransportation.Utils.Helpers.Common.DTOs;
using CairoTransportation.Utils.Helpers.Common.Instrumentation;
using CairoTransportation.Utils.Helpers.Graph;

namespace CairoTransportation.Modules.Routing.Services;

public class TimeVaryingDijkstraService(
    IGraphService graphService,
    ITrafficService trafficService,
    IMlPredictionService mlPredictionService,
    ITimeVaryingRoutePlanner planner,
    ISimulationService simulationService,
    AlgorithmExecutionMetrics metrics) : ITimeVaryingDijkstraService
{
    public async Task<AlgorithmResponseDto<ShortestPathResultDto>> FindShortestPathAsync(string from, string to, string period, bool useMl = true)
    {
        // 1. Load city graph and traffic metadata
        Graph graph = await graphService.GetGraphAsync();
        TrafficPeriodMultiplier? periodMultiplier = await trafficService.GetPeriodMultiplierAsync(period);

        if (periodMultiplier == null)
        {
            return new AlgorithmResponseDto<ShortestPathResultDto> { Success = false, Message = "Invalid period." };
        }

        // 2. Fetch traffic flow data for the specified period
        var traffic = (await trafficService.GetByPeriodAsync(period))
            .GroupBy(x => x.RoadId)
            .ToDictionary(g => g.Key, g => g.Max(x => x.Flow));

        // 3. Load ML predictions for the specified period (only if useMl is true)
        Dictionary<(long RoadId, string Period), double>? mlPredictions = useMl 
            ? await mlPredictionService.GetCongestionMapAsync() 
            : null;

        // 4. Execute traffic-aware route planning with ML predictions
        ShortestPathResultDto data = planner.FindShortestPath(
            graph,
            from,
            to,
            traffic,
            periodMultiplier.Multiplier,
            mlPredictions,
            period);

        AlgorithmTraceDto trace = metrics.Complete();
        simulationService.RecordMetrics("Time-Varying Dijkstra", trace.ExecutionTimeMs, trace.VisitedNodes, trace.ExpandedNodes);

        return new AlgorithmResponseDto<ShortestPathResultDto>
        {
            AlgorithmName = "Time-Varying Dijkstra",
            Success = data.Found,
            Message = data.Found ? $"Path found for {period.ToUpper()}." : "No path.",
            Trace = trace,
            Data = data
        };
    }
}
