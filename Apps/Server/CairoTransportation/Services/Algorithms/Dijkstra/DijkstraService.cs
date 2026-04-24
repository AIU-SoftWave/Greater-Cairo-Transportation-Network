using CairoTransportation.Services.Algorithms.Dijkstra.Contracts;
using CairoTransportation.Services.Algorithms.Dijkstra.DTOs;
using CairoTransportation.Services.Graph;

namespace CairoTransportation.Services.Algorithms.Dijkstra;

public class DijkstraService(IGraphService graphService) : IDijkstraService
{
    public async Task<ShortestPathResultDto> FindShortestPathAsync(string fromNodeId, string toNodeId)
    {
        CairoTransportation.Services.Graph.Graph graph = await graphService.GetGraphAsync();

        if (!graph.NodeIndex.ContainsKey(fromNodeId))
        {
            return new ShortestPathResultDto
            {
                FromNodeId = fromNodeId,
                ToNodeId = toNodeId,
                Found = false,
                Message = $"Start node '{fromNodeId}' was not found."
            };
        }

        if (!graph.NodeIndex.ContainsKey(toNodeId))
        {
            return new ShortestPathResultDto
            {
                FromNodeId = fromNodeId,
                ToNodeId = toNodeId,
                Found = false,
                Message = $"Destination node '{toNodeId}' was not found."
            };
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
            }
        }

        if (!double.IsFinite(distances[toNodeId]))
        {
            return new ShortestPathResultDto
            {
                FromNodeId = fromNodeId,
                ToNodeId = toNodeId,
                Found = false,
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

        return new ShortestPathResultDto
        {
            FromNodeId = fromNodeId,
            ToNodeId = toNodeId,
            Found = true,
            TotalDistance = distances[toNodeId],
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
