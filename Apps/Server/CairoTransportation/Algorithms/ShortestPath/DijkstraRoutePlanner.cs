using CairoTransportation.Algorithms.ShortestPath.Contracts;
using CairoTransportation.Services.Algorithms.Common.DTOs;
using CairoTransportation.Services.Graph;

namespace CairoTransportation.Algorithms.ShortestPath;

public class DijkstraRoutePlanner : IDijkstraRoutePlanner
{
    /// <inheritdoc />
    public ShortestPathResultDto FindShortestPath(Graph graph, string fromNodeId, string toNodeId)
    {
        // 1. Validation and Edge Cases
        if (!graph.NodeIndex.ContainsKey(fromNodeId) || !graph.NodeIndex.ContainsKey(toNodeId))
        {
            return new ShortestPathResultDto { FromNodeId = fromNodeId, ToNodeId = toNodeId, Found = false };
        }

        if (fromNodeId == toNodeId)
        {
            return new ShortestPathResultDto { FromNodeId = fromNodeId, ToNodeId = toNodeId, Found = true, TotalDistance = 0, PathNodes = [MapNode(graph.NodeIndex[fromNodeId])] };
        }

        // 2. Initialization
        var distances = graph.Nodes.ToDictionary(n => n.Id, _ => double.PositiveInfinity);
        var previousNode = new Dictionary<string, string>();
        var previousRoad = new Dictionary<string, long>();
        var visited = new HashSet<string>();
        var queue = new PriorityQueue<string, double>();

        distances[fromNodeId] = 0;
        queue.Enqueue(fromNodeId, 0);

        // 3. Main Dijkstra Loop
        while (queue.Count > 0)
        {
            string curr = queue.Dequeue();
            if (!visited.Add(curr))
            {
                continue;
            }

            if (curr == toNodeId)
            {
                break; // Destination reached
            }

            if (!graph.AdjacencyList.TryGetValue(curr, out List<long>? edgeIds))
            {
                continue;
            }

            foreach (long edgeId in edgeIds)
            {
                if (!graph.EdgeIndex.TryGetValue(edgeId, out GraphEdge? edge))
                {
                    continue;
                }

                string neighbor = edge.ToNodeId;
                double newDist = distances[curr] + edge.Distance;
                
                // Relaxation step
                if (newDist < distances[neighbor])
                {
                    distances[neighbor] = newDist;
                    previousNode[neighbor] = curr;
                    previousRoad[neighbor] = edge.Id;
                    queue.Enqueue(neighbor, newDist);
                }
            }
        }

        // 4. Result Construction
        if (!double.IsFinite(distances[toNodeId]))
        {
            return new ShortestPathResultDto { FromNodeId = fromNodeId, ToNodeId = toNodeId, Found = false };
        }

        var nodePath = new List<string>();
        var roadPath = new List<long>();
        string pathCurr = toNodeId;

        // Trace back from destination to start
        nodePath.Add(pathCurr);
        while (previousNode.TryGetValue(pathCurr, out string? prev))
        {
            roadPath.Add(previousRoad[pathCurr]);
            nodePath.Add(prev);
            pathCurr = prev;
        }

        nodePath.Reverse(); 
        roadPath.Reverse();

        return new ShortestPathResultDto
        {
            FromNodeId = fromNodeId, ToNodeId = toNodeId, Found = true, TotalDistance = distances[toNodeId],
            PathNodes = nodePath.Select(id => MapNode(graph.NodeIndex[id])).ToList(),
            PathRoads = roadPath.Select(id => MapRoad(graph.EdgeIndex[id])).ToList()
        };
    }

    private static ShortestPathNodeDto MapNode(GraphNode n) => new() { Id = n.Id, Name = n.Name, Type = n.Type, X = n.X, Y = n.Y, Population = n.Population, IsCritical = n.IsCritical };
    private static ShortestPathRoadDto MapRoad(GraphEdge e) => new() { Id = Math.Abs(e.Id), FromNodeId = e.FromNodeId, ToNodeId = e.ToNodeId, Distance = e.Distance, Capacity = e.Capacity, Condition = e.Condition, IsExisting = e.IsExisting, ConstructionCost = e.ConstructionCost };
}
