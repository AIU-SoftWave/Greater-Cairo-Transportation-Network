using CairoTransportation.Services.Algorithms.Common.DTOs;
using CairoTransportation.Services.Algorithms.Common.Instrumentation;
using CairoTransportation.Services.Algorithms.Dijkstra.Contracts;
using CairoTransportation.Services.Graph;
using Microsoft.Extensions.Caching.Memory;

namespace CairoTransportation.Services.Algorithms.Dijkstra;

/// <summary>
/// Dijkstra's shortest-path algorithm with top-down memoization.
///
/// Memoization strategy (top-down):
///   The computed path for any (from, to) pair is stored in IMemoryCache with a 60-second TTL.
///   On a cache hit the full result is returned immediately without re-running the algorithm.
///   This is the standard top-down memoization pattern applied to the route-planning problem:
///     memo[(u,v)] = cheapest path from u to v
///   Unlike pure bottom-up DP (which pre-computes all pairs), this approach is lazy — only
///   pairs that are actually requested are ever stored.
///
/// Complexity:
///   Time  – O((V + E) log V) per unique (from, to) query; O(1) on subsequent cache hits.
///   Space – O(V + E) for the graph + O(P) for each cached path result (P = path length ≤ V).
/// </summary>
public class DijkstraService(IGraphService graphService, IMemoryCache cache) : IDijkstraService
{
    private static readonly TimeSpan PathCacheTtl = TimeSpan.FromSeconds(60);

    public async Task<AlgorithmResponseDto<ShortestPathResultDto>> FindShortestPathAsync(string fromNodeId, string toNodeId)
    {
        // Top-down memoization: return cached path if available
        string cacheKey = $"dijkstra:{fromNodeId}:{toNodeId}";
        if (cache.TryGetValue(cacheKey, out AlgorithmResponseDto<ShortestPathResultDto>? cachedResult) && cachedResult is not null)
        {
            return cachedResult;
        }

        var metrics = new AlgorithmExecutionMetrics();
        CairoTransportation.Services.Graph.Graph graph = await graphService.GetGraphAsync();

        if (!graph.NodeIndex.ContainsKey(fromNodeId))
        {
            return CreateFailureResponse(fromNodeId, toNodeId, $"Start node '{fromNodeId}' was not found.", metrics);
        }

        if (!graph.NodeIndex.ContainsKey(toNodeId))
        {
            return CreateFailureResponse(fromNodeId, toNodeId, $"Destination node '{toNodeId}' was not found.", metrics);
        }

        metrics.MarkDiscovered(fromNodeId);

        if (fromNodeId == toNodeId)
        {
            metrics.MarkExpanded();
            return CreateSuccessResponse(
                new ShortestPathResultDto
                {
                    FromNodeId = fromNodeId,
                    ToNodeId = toNodeId,
                    Found = true,
                    TotalDistance = 0,
                    PathNodes = [MapNode(graph.NodeIndex[fromNodeId])]
                },
                "Start and destination are the same node.",
                metrics);
        }

        var distances = new Dictionary<string, double>();
        var previousNode = new Dictionary<string, string>();
        var previousRoad = new Dictionary<string, long>();
        var visited = new HashSet<string>();
        var queue = new PriorityQueue<string, double>();

        foreach (GraphNode node in graph.Nodes)
        {
            distances[node.Id] = double.PositiveInfinity;
        }

        distances[fromNodeId] = 0;
        queue.Enqueue(fromNodeId, 0);

        while (queue.Count > 0)
        {
            string currentNodeId = queue.Dequeue();

            if (!visited.Add(currentNodeId))
            {
                continue;
            }

            metrics.MarkExpanded();

            if (currentNodeId == toNodeId)
            {
                break;
            }

            if (!graph.AdjacencyList.TryGetValue(currentNodeId, out List<long>? edgeIds))
            {
                continue;
            }

            foreach (long edgeId in edgeIds)
            {
                if (!graph.EdgeIndex.TryGetValue(edgeId, out GraphEdge? edge))
                {
                    continue;
                }

                string neighborNodeId = edge.ToNodeId;
                if (!graph.NodeIndex.ContainsKey(neighborNodeId))
                {
                    continue;
                }

                double newDistance = distances[currentNodeId] + edge.Distance;
                if (newDistance >= distances[neighborNodeId])
                {
                    continue;
                }

                distances[neighborNodeId] = newDistance;
                previousNode[neighborNodeId] = currentNodeId;
                previousRoad[neighborNodeId] = edge.Id;
                queue.Enqueue(neighborNodeId, newDistance);
                metrics.MarkDiscovered(neighborNodeId);
            }
        }

        if (!double.IsFinite(distances[toNodeId]))
        {
            return CreateFailureResponse(fromNodeId, toNodeId, $"No path was found from '{fromNodeId}' to '{toNodeId}'.", metrics);
        }

        var nodePath = new List<string>();
        var roadPath = new List<long>();
        string current = toNodeId;

        nodePath.Add(current);
        while (previousNode.TryGetValue(current, out string? prevNode))
        {
            roadPath.Add(previousRoad[current]);
            nodePath.Add(prevNode);
            current = prevNode;
        }

        nodePath.Reverse();
        roadPath.Reverse();

        var pathNodes = nodePath
            .Select(nodeId => MapNode(graph.NodeIndex[nodeId]))
            .ToList();

        var pathRoads = roadPath
            .Select(roadId => MapRoad(graph.EdgeIndex[roadId]))
            .ToList();

        AlgorithmResponseDto<ShortestPathResultDto> result = CreateSuccessResponse(
            new ShortestPathResultDto
            {
                FromNodeId = fromNodeId,
                ToNodeId = toNodeId,
                Found = true,
                TotalDistance = distances[toNodeId],
                PathNodes = pathNodes,
                PathRoads = pathRoads
            },
            "Shortest path found using Dijkstra's algorithm.",
            metrics);

        // Store computed path in top-down memo cache
        cache.Set(cacheKey, result, PathCacheTtl);

        return result;
    }

    private static AlgorithmResponseDto<ShortestPathResultDto> CreateSuccessResponse(
        ShortestPathResultDto result,
        string message,
        AlgorithmExecutionMetrics metrics) =>
        new()
        {
            AlgorithmName = "Dijkstra",
            Success = true,
            Message = message,
            Trace = metrics.Complete(),
            Data = result
        };

    private static AlgorithmResponseDto<ShortestPathResultDto> CreateFailureResponse(
        string fromNodeId,
        string toNodeId,
        string message,
        AlgorithmExecutionMetrics metrics) =>
        new()
        {
            AlgorithmName = "Dijkstra",
            Success = false,
            Message = message,
            Trace = metrics.Complete(),
            Data = new ShortestPathResultDto
            {
                FromNodeId = fromNodeId,
                ToNodeId = toNodeId,
                Found = false
            }
        };

    private static ShortestPathNodeDto MapNode(GraphNode node) => new()
    {
        Id = node.Id,
        Name = node.Name,
        Type = node.Type,
        X = node.X,
        Y = node.Y,
        Population = node.Population,
        IsCritical = node.IsCritical
    };

    private static ShortestPathRoadDto MapRoad(GraphEdge edge) => new()
    {
        Id = Math.Abs(edge.Id),
        FromNodeId = edge.FromNodeId,
        ToNodeId = edge.ToNodeId,
        Distance = edge.Distance,
        Capacity = edge.Capacity,
        Condition = edge.Condition,
        IsExisting = edge.IsExisting,
        ConstructionCost = edge.ConstructionCost
    };
}
