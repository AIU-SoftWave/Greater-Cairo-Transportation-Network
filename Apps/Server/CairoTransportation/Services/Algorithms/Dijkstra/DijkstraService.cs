using System.Diagnostics;
using CairoTransportation.Services.Algorithms.Dijkstra.Contracts;
using CairoTransportation.Services.Algorithms.Dijkstra.DTOs;
using CairoTransportation.Services.Graph;

namespace CairoTransportation.Services.Algorithms.Dijkstra;

public class DijkstraService(IGraphService graphService) : IDijkstraService
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

        var distances = new Dictionary<string, double>();
        var previousNode = new Dictionary<string, string>();
        var previousRoad = new Dictionary<string, long>();
        var visited = new HashSet<string>();
        var discovered = new HashSet<string> { fromNodeId };
        var queue = new PriorityQueue<string, double>();
        int expandedNodes = 0;

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
                discovered.Add(neighborNodeId);
            }
        }

        if (!double.IsFinite(distances[toNodeId]))
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

        stopwatch.Stop();
        return new ShortestPathResultDto
        {
            FromNodeId = fromNodeId,
            ToNodeId = toNodeId,
            Found = true,
            TotalDistance = distances[toNodeId],
            VisitedNodes = discovered.Count,
            ExpandedNodes = expandedNodes,
            ExecutionTimeMs = stopwatch.ElapsedMilliseconds,
            PathNodes = pathNodes,
            PathRoads = pathRoads,
            Message = "Shortest path found using Dijkstra's algorithm."
        };
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
