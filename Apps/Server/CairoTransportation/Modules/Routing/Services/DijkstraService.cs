using CairoTransportation.Algorithms.ShortestPath.Contracts;
using CairoTransportation.Services.Algorithms.Common.DTOs;
using CairoTransportation.Services.Algorithms.Common.Instrumentation;
using CairoTransportation.Services.Graph;
using CairoTransportation.Services.Routing.Contracts;
using Microsoft.Extensions.Caching.Memory;

namespace CairoTransportation.Services.Routing;

public class DijkstraService(
    IGraphService graphService, 
    IMemoryCache cache, 
    IDijkstraRoutePlanner planner,
    ISimulationService simulationService,
    AlgorithmExecutionMetrics metrics) : IDijkstraService
{
    private static readonly TimeSpan PathCacheTtl = TimeSpan.FromSeconds(60);

    public async Task<AlgorithmResponseDto<ShortestPathResultDto>> FindShortestPathAsync(string fromNodeId, string toNodeId)
    {
        // 1. Check cache for recent results
        int version = simulationService.GetStateVersion();
        string cacheKey = $"dijkstra:{fromNodeId}:{toNodeId}:v{version}";
        if (cache.TryGetValue(cacheKey, out AlgorithmResponseDto<ShortestPathResultDto>? cached) && cached is not null)
        {
            return cached;
        }

        // 2. Load the city network graph
        Graph.Graph graph = await graphService.GetGraphAsync();

        // 3. Execute Dijkstra algorithm
        ShortestPathResultDto data = planner.FindShortestPath(graph, fromNodeId, toNodeId);
        
        if (data.Found)
        {
            // Default multiplier for standard Dijkstra is 1.0 (Average traffic)
            data.EstimatedTravelTimeMinutes = data.TotalDistance * 1.0;
        }

        var trace = metrics.Complete();
        simulationService.RecordMetrics("Dijkstra", trace.ExecutionTimeMs, trace.VisitedNodes, trace.ExpandedNodes);

        var result = new AlgorithmResponseDto<ShortestPathResultDto>
        {
            AlgorithmName = "Dijkstra",
            Success = data.Found,
            Message = data.Found ? "Shortest path found." : "No path found.",
            Trace = trace,
            Data = data
        };

        // 4. Cache result if successful
        if (data.Found)
        {
            cache.Set(cacheKey, result, PathCacheTtl);
        }

        return result;
    }
}
