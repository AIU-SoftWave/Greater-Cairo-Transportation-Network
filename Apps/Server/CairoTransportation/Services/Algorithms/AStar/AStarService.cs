using System.Diagnostics;
using CairoTransportation.Services.Algorithms.AStar.Contracts;
using CairoTransportation.Services.Algorithms.AStar.DTOs;
using CairoTransportation.Services.Graph;

namespace CairoTransportation.Services.Algorithms.AStar;

public class AStarService(IGraphService graphService) : IAStarService
{
    public async Task<ShortestPathResultDto> FindShortestPathAsync(string fromNodeId, string toNodeId)
    {
        var stopwatch = Stopwatch.StartNew();
        CairoTransportation.Services.Graph.Graph graph = await graphService.GetGraphAsync();

        if (!graph.NodeIndex.ContainsKey(fromNodeId))
        {
            stopwatch.Stop();
            return new ShortestPathResultDto
            {
                FromNodeId = fromNodeId,
                ToNodeId = toNodeId,
                Found = false,
                VisitedNodes = 0,
                ExpandedNodes = 0,
                ExecutionTimeMs = stopwatch.ElapsedMilliseconds,
                Message = $"Start node '{fromNodeId}' was not found."
            };
        }

        if (!graph.NodeIndex.ContainsKey(toNodeId))
        {
            stopwatch.Stop();
            return new ShortestPathResultDto
            {
                FromNodeId = fromNodeId,
                ToNodeId = toNodeId,
                Found = false,
                VisitedNodes = 0,
                ExpandedNodes = 0,
                ExecutionTimeMs = stopwatch.ElapsedMilliseconds,
                Message = $"Destination node '{toNodeId}' was not found."
            };
        }

        if (fromNodeId == toNodeId)
        {
            stopwatch.Stop();
            return new ShortestPathResultDto
            {
                FromNodeId = fromNodeId,
                ToNodeId = toNodeId,
                Found = true,
                TotalDistance = 0,
                VisitedNodes = 1,
                ExpandedNodes = 1,
                ExecutionTimeMs = stopwatch.ElapsedMilliseconds,
                PathNodes = [MapNode(graph.NodeIndex[fromNodeId])],
                Message = "Start and destination are the same node."
            };
        }

        var gScore = new Dictionary<string, double>();
        var cameFromNode = new Dictionary<string, string>();
        var cameFromRoad = new Dictionary<string, long>();
        var openSet = new PriorityQueue<string, double>();
        var closedSet = new HashSet<string>();
        var discovered = new HashSet<string> { fromNodeId };
        int expandedNodes = 0;

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

            expandedNodes++;

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
                discovered.Add(neighborNodeId);
            }
        }

        if (!double.IsFinite(gScore[toNodeId]))
        {
            stopwatch.Stop();
            return new ShortestPathResultDto
            {
                FromNodeId = fromNodeId,
                ToNodeId = toNodeId,
                Found = false,
                VisitedNodes = discovered.Count,
                ExpandedNodes = expandedNodes,
                ExecutionTimeMs = stopwatch.ElapsedMilliseconds,
                Message = $"No path was found from '{fromNodeId}' to '{toNodeId}'."
            };
        }

        var nodePath = new List<string>();
        var roadPath = new List<long>();
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

        stopwatch.Stop();
        return new ShortestPathResultDto
        {
            FromNodeId = fromNodeId,
            ToNodeId = toNodeId,
            Found = true,
            TotalDistance = gScore[toNodeId],
            VisitedNodes = discovered.Count,
            ExpandedNodes = expandedNodes,
            ExecutionTimeMs = stopwatch.ElapsedMilliseconds,
            PathNodes = pathNodes,
            PathRoads = pathRoads,
            Message = "Shortest path found using A* search."
        };
    }

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
