using CairoTransportation.Algorithms.ShortestPath.Contracts;
using CairoTransportation.Services.Algorithms.Common.DTOs;
using CairoTransportation.Services.Algorithms.Common.Instrumentation;
using CairoTransportation.Services.Graph;
using CairoTransportation.Services.Routing.Contracts;
using Microsoft.Extensions.Caching.Memory;

namespace CairoTransportation.Services.Routing;

public class AStarService(
    IGraphService graphService, 
    IMemoryCache cache, 
    IAStarPathFinder planner,
    ISimulationService simulationService,
    AlgorithmExecutionMetrics metrics) : IAStarService
{
    private static readonly TimeSpan PathCacheTtl = TimeSpan.FromSeconds(60);

    public async Task<AlgorithmResponseDto<ShortestPathResultDto>> FindShortestPathAsync(string from, string to)
    {
        // 1. Check cache for recent results
        int version = simulationService.GetStateVersion();
        string key = $"astar:{from}:{to}:v{version}";
        if (cache.TryGetValue(key, out AlgorithmResponseDto<ShortestPathResultDto>? c) && c != null)
        {
            return c;
        }

        // 2. Load the network graph
        Graph.Graph g = await graphService.GetGraphAsync();
        
        // 3. Execute A* search algorithm
        ShortestPathResultDto data = planner.FindShortestPath(g, from, to);
        
        if (data.Found)
        {
            data.EstimatedTravelTimeMinutes = data.TotalDistance * 1.0;
        }

        var trace = metrics.Complete();
        simulationService.RecordMetrics("A*", trace.ExecutionTimeMs, trace.VisitedNodes, trace.ExpandedNodes);

        // Enable emergency preemption for the calculated path
        if (data.Found)
        {
            foreach (var road in data.PathRoads)
            {
                await simulationService.SetEmergencyPreemptionAsync(road.Id, true);
            }
        }

        var res = new AlgorithmResponseDto<ShortestPathResultDto> 
        { 
            AlgorithmName = "A*", 
            Success = data.Found, 
            Message = data.Found ? "Path found." : "No path.", 
            Trace = trace, 
            Data = data 
        };

        // 4. Cache result if successful
        if (data.Found)
        {
            cache.Set(key, res, PathCacheTtl);
        }

        return res;
    }

    public async Task<AlgorithmResponseDto<ShortestPathResultDto>> FindNearestMedicalFacilityAsync(string from)
    {
        // 1. Check cache
        int version = simulationService.GetStateVersion();
        string key = $"astar:medical:{from}:v{version}";
        if (cache.TryGetValue(key, out AlgorithmResponseDto<ShortestPathResultDto>? c) && c != null)
        {
            return c;
        }

        var m = new AlgorithmExecutionMetrics();
        
        // 2. Load the network graph
        Graph.Graph g = await graphService.GetGraphAsync();
        
        // 3. Execute A* search for nearest critical facility
        ShortestPathResultDto data = planner.FindNearestMedicalFacility(g, from);

        if (data.Found)
        {
            // Emergency speed is higher, so multiplier is lower (0.75x)
            data.EstimatedTravelTimeMinutes = data.TotalDistance * 0.75;
        }

        var res = new AlgorithmResponseDto<ShortestPathResultDto> 
        { 
            AlgorithmName = "A* Nearest Medical", 
            Success = data.Found, 
            Message = data.Found ? "Facility found." : "No reachable facility.", 
            Trace = m.Complete(), 
            Data = data 
        };

        // 4. Cache result
        if (data.Found)
        {
            cache.Set(key, res, PathCacheTtl);
        }

        return res;
    }
}
