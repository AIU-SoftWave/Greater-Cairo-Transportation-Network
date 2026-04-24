using CairoTransportation.Models;
using CairoTransportation.Services.Algorithms.Common.DTOs;
using CairoTransportation.Services.Algorithms.Common.Instrumentation;
using CairoTransportation.Services.Algorithms.TimeVaryingDijkstra.Contracts;
using CairoTransportation.Services.Graph;

namespace CairoTransportation.Services.Algorithms.TimeVaryingDijkstra;

public class TimeVaryingDijkstraService(IGraphService graphService, ITrafficService trafficService) : ITimeVaryingDijkstraService
{
    public async Task<AlgorithmResponseDto<ShortestPathResultDto>> FindShortestPathAsync(string fromNodeId, string toNodeId, string period)
    {
        AlgorithmExecutionMetrics metrics = new();
        string normalizedPeriod = period.Trim().ToUpperInvariant();

        CairoTransportation.Services.Graph.Graph graph = await graphService.GetGraphAsync();

        if (!graph.NodeIndex.ContainsKey(fromNodeId))
        {
            return CreateFailureResponse(fromNodeId, toNodeId, $"Start node '{fromNodeId}' was not found.", metrics);
        }

        if (!graph.NodeIndex.ContainsKey(toNodeId))
        {
            return CreateFailureResponse(fromNodeId, toNodeId, $"Destination node '{toNodeId}' was not found.", metrics);
        }

        TrafficPeriodMultiplier? periodMultiplier = await trafficService.GetPeriodMultiplierAsync(normalizedPeriod);
        if (periodMultiplier is null)
        {
            return CreateFailureResponse(
                fromNodeId,
                toNodeId,
                $"Unsupported period '{period}'. No multiplier is configured in database.",
                metrics);
        }

        var trafficByRoadId = (await trafficService.GetByPeriodAsync(normalizedPeriod))
            .GroupBy(x => x.RoadId)
            .ToDictionary(g => g.Key, g => g.Max(x => x.Flow));

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
                $"Start and destination are the same node for period '{normalizedPeriod}'.",
                metrics);
        }

        Dictionary<string, double> distances = [];
        Dictionary<string, string> previousNode = [];
        Dictionary<string, long> previousRoad = [];
        HashSet<string> visited = [];
        PriorityQueue<string, double> queue = new();

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

                double effectiveDistance = edge.Distance * GetTrafficAdjustment(edge, trafficByRoadId, periodMultiplier.Multiplier);
                double newDistance = distances[currentNodeId] + effectiveDistance;

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
            return CreateFailureResponse(
                fromNodeId,
                toNodeId,
                $"No traffic-aware path was found from '{fromNodeId}' to '{toNodeId}' for period '{normalizedPeriod}'.",
                metrics);
        }

        List<string> nodePath = [];
        List<long> roadPath = [];
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

        return CreateSuccessResponse(
            new ShortestPathResultDto
            {
                FromNodeId = fromNodeId,
                ToNodeId = toNodeId,
                Found = true,
                TotalDistance = distances[toNodeId],
                PathNodes = pathNodes,
                PathRoads = pathRoads
            },
            $"Traffic-aware shortest path found for period '{normalizedPeriod}' using time-varying Dijkstra.",
            metrics);
    }

    private static double GetTrafficAdjustment(GraphEdge edge, Dictionary<long, int> trafficByRoadId, double periodMultiplier)
    {
        long roadId = Math.Abs(edge.Id);
        if (!trafficByRoadId.TryGetValue(roadId, out int flow))
        {
            return periodMultiplier;
        }

        double congestionRatio = (double)flow / edge.Capacity;

        if (congestionRatio <= 0.75)
        {
            return periodMultiplier;
        }

        if (congestionRatio <= 1.0)
        {
            return periodMultiplier * 1.1;
        }

        if (congestionRatio <= 1.25)
        {
            return periodMultiplier * 1.2;
        }

        return periodMultiplier * 1.35;
    }

    private static AlgorithmResponseDto<ShortestPathResultDto> CreateSuccessResponse(
        ShortestPathResultDto result,
        string message,
        AlgorithmExecutionMetrics metrics) =>
        new()
        {
            AlgorithmName = "Time-Varying Dijkstra",
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
            AlgorithmName = "Time-Varying Dijkstra",
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
