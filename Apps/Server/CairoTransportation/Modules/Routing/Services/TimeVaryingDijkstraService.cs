using CairoTransportation.Algorithms.ShortestPath.Contracts;
using CairoTransportation.Models;
using CairoTransportation.Services.Algorithms.Common.DTOs;
using CairoTransportation.Services.Algorithms.Common.Instrumentation;
using CairoTransportation.Services.Graph;
using CairoTransportation.Services.Routing.Contracts;

namespace CairoTransportation.Services.Routing;

public class TimeVaryingDijkstraService(
    IGraphService graphService, 
    ITrafficService trafficService, 
    ITimeVaryingRoutePlanner planner) : ITimeVaryingDijkstraService
{
    public async Task<AlgorithmResponseDto<ShortestPathResultDto>> FindShortestPathAsync(string from, string to, string period)
    {
        var metrics = new AlgorithmExecutionMetrics();
        
        // 1. Load city graph and traffic metadata
        Graph.Graph graph = await graphService.GetGraphAsync();
        TrafficPeriodMultiplier? periodMultiplier = await trafficService.GetPeriodMultiplierAsync(period);
        
        if (periodMultiplier == null)
        {
            return new AlgorithmResponseDto<ShortestPathResultDto> { Success = false, Message = "Invalid period." };
        }

        // 2. Fetch traffic flow data for the specified period
        var traffic = (await trafficService.GetByPeriodAsync(period))
            .GroupBy(x => x.RoadId)
            .ToDictionary(g => g.Key, g => g.Max(x => x.Flow));

        // 3. Execute traffic-aware route planning
        ShortestPathResultDto data = planner.FindShortestPath(graph, from, to, traffic, periodMultiplier.Multiplier);

        return new AlgorithmResponseDto<ShortestPathResultDto>
        {
            AlgorithmName = "Time-Varying Dijkstra",
            Success = data.Found,
            Message = data.Found ? $"Path found for {period}." : "No path.",
            Trace = metrics.Complete(),
            Data = data
        };
    }
}
