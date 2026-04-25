using CairoTransportation.Services.Algorithms.AStar.Contracts;
using CairoTransportation.Services.Algorithms.Common.DTOs;
using CairoTransportation.Services.Algorithms.Common.Instrumentation;
using CairoTransportation.Services.Graph;

namespace CairoTransportation.Services.Algorithms.AStar;

public class AStarService(IGraphService graphService) : IAStarService
{
    public async Task<AlgorithmResponseDto<ShortestPathResultDto>> FindShortestPathAsync(string fromNodeId, string toNodeId)
    {
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
            return CreateFailureResponse(fromNodeId, toNodeId, $"No path was found from '{fromNodeId}' to '{toNodeId}'.", metrics);
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
