using CairoTransportation.Services.Algorithms.AStar.Contracts;
using CairoTransportation.Services.Algorithms.Common.DTOs;
using CairoTransportation.Services.Algorithms.Common.Instrumentation;
using CairoTransportation.Services.Graph;
using Microsoft.Extensions.Caching.Memory;

namespace CairoTransportation.Services.Algorithms.AStar;

/// <summary>
/// A* search algorithm with top-down memoization and medical-facility targeting.
///
/// A* improves on Dijkstra by using an admissible heuristic h(n) that estimates the remaining
/// distance from node n to the goal. The Euclidean straight-line distance in coordinate space
/// is used as h(n), which is admissible (never overestimates) and consistent (satisfies the
/// triangle inequality), guaranteeing optimal paths.
///
/// Medical facility targeting:
///   FindNearestMedicalFacilityAsync runs A* from a given origin to every node with
///   category = "Medical" or is_critical = true and returns the path to the closest one.
///   This directly models emergency vehicle dispatch in Cairo.
///
/// Memoization strategy (top-down):
///   Completed A* paths are cached in IMemoryCache with a 60-second TTL under the key
///   "astar:{from}:{to}". The nearest-medical result is cached under "astar:medical:{from}".
///   This is top-down memoization — paths are stored on first computation and reused lazily.
///
/// Complexity:
///   Time  – O((V + E) log V) worst case; typically far fewer nodes expanded than Dijkstra
///           because the heuristic prunes large parts of the search space.
///   Space – O(V + E) for the graph + O(P) per cached path entry.
/// </summary>
public class AStarService(IGraphService graphService, IMemoryCache cache) : IAStarService
{
    private static readonly TimeSpan PathCacheTtl = TimeSpan.FromSeconds(60);

    public async Task<AlgorithmResponseDto<ShortestPathResultDto>> FindShortestPathAsync(string fromNodeId, string toNodeId)
    {
        // Top-down memoization
        string cacheKey = $"astar:{fromNodeId}:{toNodeId}";
        if (cache.TryGetValue(cacheKey, out AlgorithmResponseDto<ShortestPathResultDto>? cached) && cached is not null)
        {
            return cached;
        }

        AlgorithmExecutionMetrics metrics = new();
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

        AlgorithmResponseDto<ShortestPathResultDto> result = RunAStar(graph, fromNodeId, toNodeId, metrics);

        // Store in top-down memo cache
        if (result.Success)
        {
            cache.Set(cacheKey, result, PathCacheTtl);
        }

        return result;
    }

    /// <summary>
    /// Finds the shortest emergency route from <paramref name="fromNodeId"/> to the nearest
    /// medical facility (nodes with category = "Medical" or is_critical = true).
    ///
    /// Algorithm: run A* to each reachable medical facility in parallel (using the cached graph),
    /// then return the result with the minimum total distance.  Because A* expands far fewer nodes
    /// than Dijkstra toward a specific target, this is efficient even when there are multiple
    /// candidate hospitals.
    /// </summary>
    public async Task<AlgorithmResponseDto<ShortestPathResultDto>> FindNearestMedicalFacilityAsync(string fromNodeId)
    {
        // Top-down memoization for the nearest-medical query
        string cacheKey = $"astar:medical:{fromNodeId}";
        if (cache.TryGetValue(cacheKey, out AlgorithmResponseDto<ShortestPathResultDto>? cached) && cached is not null)
        {
            return cached;
        }

        AlgorithmExecutionMetrics metrics = new();
        CairoTransportation.Services.Graph.Graph graph = await graphService.GetGraphAsync();

        if (!graph.NodeIndex.ContainsKey(fromNodeId))
        {
            return CreateFailureResponse(fromNodeId, "nearest-medical",
                $"Start node '{fromNodeId}' was not found.", metrics);
        }

        // Collect all medical facility nodes
        List<GraphNode> medicalNodes = graph.Nodes
            .Where(n => n.IsCritical ||
                        n.Type.Equals("FACILITY", StringComparison.OrdinalIgnoreCase))
            .Where(n => n.Id != fromNodeId)
            .ToList();

        if (medicalNodes.Count == 0)
        {
            return CreateFailureResponse(fromNodeId, "nearest-medical",
                "No medical facilities found in the network.", metrics);
        }

        // Run A* to each candidate; keep the shortest result
        AlgorithmResponseDto<ShortestPathResultDto>? best = null;

        foreach (GraphNode facility in medicalNodes)
        {
            AlgorithmExecutionMetrics facilityMetrics = new();
            AlgorithmResponseDto<ShortestPathResultDto> candidate =
                RunAStar(graph, fromNodeId, facility.Id, facilityMetrics);

            if (!candidate.Success || !candidate.Data!.Found)
            {
                continue;
            }

            if (best is null || candidate.Data.TotalDistance < best.Data!.TotalDistance)
            {
                best = candidate;
            }
        }

        if (best is null)
        {
            return CreateFailureResponse(fromNodeId, "nearest-medical",
                "No reachable medical facility found from the given origin.", metrics);
        }

        AlgorithmResponseDto<ShortestPathResultDto> result = new()
        {
            AlgorithmName = "A* – Nearest Medical Facility",
            Success = true,
            Message = $"Nearest medical facility: {best.Data!.PathNodes.LastOrDefault()?.Name ?? best.Data.ToNodeId} " +
                      $"({best.Data.TotalDistance:F2} km via emergency route).",
            Trace = metrics.Complete(),
            Data = best.Data
        };

        cache.Set(cacheKey, result, PathCacheTtl);
        return result;
    }

    // ─── Core A* implementation ──────────────────────────────────────────────

    private static AlgorithmResponseDto<ShortestPathResultDto> RunAStar(
        CairoTransportation.Services.Graph.Graph graph,
        string fromNodeId,
        string toNodeId,
        AlgorithmExecutionMetrics metrics)
    {
        metrics.MarkDiscovered(fromNodeId);

        Dictionary<string, double> gScore = [];
        Dictionary<string, string> cameFromNode = [];
        Dictionary<string, long> cameFromRoad = [];
        PriorityQueue<string, double> openSet = new();
        HashSet<string> closedSet = [];

        foreach (GraphNode node in graph.Nodes)
        {
            gScore[node.Id] = double.PositiveInfinity;
        }

        gScore[fromNodeId] = 0;
        openSet.Enqueue(fromNodeId, Heuristic(graph, fromNodeId, toNodeId));

        while (openSet.Count > 0)
        {
            string currentNodeId = openSet.Dequeue();

            if (!closedSet.Add(currentNodeId))
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
                if (!graph.NodeIndex.ContainsKey(neighborNodeId) || closedSet.Contains(neighborNodeId))
                {
                    continue;
                }

                double tentativeGScore = gScore[currentNodeId] + edge.Distance;
                if (tentativeGScore >= gScore[neighborNodeId])
                {
                    continue;
                }

                cameFromNode[neighborNodeId] = currentNodeId;
                cameFromRoad[neighborNodeId] = edge.Id;
                gScore[neighborNodeId] = tentativeGScore;

                double fScore = tentativeGScore + Heuristic(graph, neighborNodeId, toNodeId);
                openSet.Enqueue(neighborNodeId, fScore);
                metrics.MarkDiscovered(neighborNodeId);
            }
        }

        if (!double.IsFinite(gScore[toNodeId]))
        {
            return CreateFailureResponse(fromNodeId, toNodeId,
                $"No path was found from '{fromNodeId}' to '{toNodeId}'.", metrics);
        }

        List<string> nodePath = [];
        List<long> roadPath = [];
        string current = toNodeId;

        nodePath.Add(current);
        while (cameFromNode.TryGetValue(current, out string? prevNode))
        {
            roadPath.Add(cameFromRoad[current]);
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

        return CreateSuccessResponse(
            new ShortestPathResultDto
            {
                FromNodeId = fromNodeId,
                ToNodeId = toNodeId,
                Found = true,
                TotalDistance = gScore[toNodeId],
                PathNodes = pathNodes,
                PathRoads = pathRoads
            },
            "Shortest path found using A* search.",
            metrics);
    }

    // ─── Helpers ─────────────────────────────────────────────────────────────

    private static AlgorithmResponseDto<ShortestPathResultDto> CreateSuccessResponse(
        ShortestPathResultDto result,
        string message,
        AlgorithmExecutionMetrics metrics) =>
        new()
        {
            AlgorithmName = "A*",
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
            AlgorithmName = "A*",
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

    /// <summary>
    /// Euclidean distance heuristic in geographic coordinate space.
    /// Admissible: always ≤ actual road distance (straight line ≤ path along roads).
    /// Consistent: satisfies h(n) ≤ w(n,m) + h(m) for all edges (n,m).
    /// </summary>
    private static double Heuristic(CairoTransportation.Services.Graph.Graph graph, string fromNodeId, string toNodeId)
    {
        GraphNode from = graph.NodeIndex[fromNodeId];
        GraphNode to = graph.NodeIndex[toNodeId];

        if (from.X is null || from.Y is null || to.X is null || to.Y is null)
        {
            return 0;
        }

        double dx = from.X.Value - to.X.Value;
        double dy = from.Y.Value - to.Y.Value;
        return Math.Sqrt(dx * dx + dy * dy);
    }

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
