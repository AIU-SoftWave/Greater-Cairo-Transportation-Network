using CairoTransportation.Algorithms.ShortestPath.Contracts;
using CairoTransportation.Services.Algorithms.Common.DTOs;
using CairoTransportation.Services.Graph;

using CairoTransportation.Services.Algorithms.Common.Instrumentation;

namespace CairoTransportation.Algorithms.ShortestPath;

public class DijkstraRoutePlanner(AlgorithmExecutionMetrics metrics) : IDijkstraRoutePlanner
{
    public ShortestPathResultDto FindShortestPath(Graph graph, string fromNodeId, string toNodeId)
    {
        // 1. Basic validation: check if start and end nodes exist
        if (!graph.NodeIndex.ContainsKey(fromNodeId) || !graph.NodeIndex.ContainsKey(toNodeId))
        {
            return new ShortestPathResultDto { FromNodeId = fromNodeId, ToNodeId = toNodeId, Found = false };
        }

        // 2. If start and end are the same, distance is zero
        if (fromNodeId == toNodeId)
        {
            return new ShortestPathResultDto { FromNodeId = fromNodeId, ToNodeId = toNodeId, Found = true, TotalDistance = 0, PathNodes = [MapNode(graph.NodeIndex[fromNodeId])] };
        }

        // 3. Setup Dijkstra data structures
        var distances = graph.Nodes.ToDictionary(n => n.Id, _ => double.PositiveInfinity);
        var previousNode = new Dictionary<string, string>();
        var previousRoad = new Dictionary<string, long>();
        var visited = new HashSet<string>();
        var queue = new PriorityQueue<string, double>();

        // Start from the beginning
        distances[fromNodeId] = 0;
        queue.Enqueue(fromNodeId, 0);
        metrics.MarkDiscovered(fromNodeId);

        // 4. Main Loop: process nodes by their current shortest distance
        while (queue.Count > 0)
        {
            string curr = queue.Dequeue();
            if (!visited.Add(curr))
            {
                continue; // Skip if already visited
            }
            metrics.MarkExpanded();


            if (curr == toNodeId)
            {
                break; // Optimization: stop early if we reached the target
            }

            // Look at all connected roads from the current node

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
                
                // Relaxation: if we found a shorter way to reach the neighbor, update it
                if (newDist < distances[neighbor])
                {
                    distances[neighbor] = newDist;
                    previousNode[neighbor] = curr;
                    previousRoad[neighbor] = edge.Id;
                    queue.Enqueue(neighbor, newDist);
                    metrics.MarkDiscovered(neighbor);
                }
            }
        }

        // 5. Check if a path was actually found
        if (!double.IsFinite(distances[toNodeId]))
        {
            return new ShortestPathResultDto { FromNodeId = fromNodeId, ToNodeId = toNodeId, Found = false };
        }

        // 6. Trace back from destination to start to build the path
        var nodePath = new List<string>();
        var roadPath = new List<long>();
        string pathCurr = toNodeId;

        nodePath.Add(pathCurr);
        while (previousNode.TryGetValue(pathCurr, out string? prev))
        {
            roadPath.Add(previousRoad[pathCurr]);
            nodePath.Add(prev);
            pathCurr = prev;
        }

        // Reverse paths because we traced them backwards
        nodePath.Reverse(); 
        roadPath.Reverse();

        return new ShortestPathResultDto
        {
            FromNodeId = fromNodeId, 
            ToNodeId = toNodeId, 
            Found = true, 
            TotalDistance = distances[toNodeId],
            PathNodes = nodePath.Select(id => MapNode(graph.NodeIndex[id])).ToList(),
            PathRoads = roadPath.Select(id => MapRoad(graph.EdgeIndex[id])).ToList()
        };
    }

    private static ShortestPathNodeDto MapNode(GraphNode n) => new() { Id = n.Id, Name = n.Name, Type = n.Type, X = n.X, Y = n.Y, Population = n.Population, IsCritical = n.IsCritical };
    private static ShortestPathRoadDto MapRoad(GraphEdge e) => new() { Id = Math.Abs(e.Id), FromNodeId = e.FromNodeId, ToNodeId = e.ToNodeId, Distance = e.Distance, Capacity = e.Capacity, Condition = e.Condition, IsExisting = e.IsExisting, ConstructionCost = e.ConstructionCost };
}
