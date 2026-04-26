using CairoTransportation.Algorithms.ShortestPath.Contracts;
using CairoTransportation.Services.Algorithms.Common.DTOs;
using CairoTransportation.Services.Graph;

namespace CairoTransportation.Algorithms.ShortestPath;

public class TimeVaryingRoutePlanner : ITimeVaryingRoutePlanner
{
    public ShortestPathResultDto FindShortestPath(
        Graph graph,
        string fromNodeId,
        string toNodeId,
        Dictionary<long, int> trafficByRoadId,
        double periodMultiplier)
    {
        // 1. Validation
        if (!graph.NodeIndex.ContainsKey(fromNodeId) || !graph.NodeIndex.ContainsKey(toNodeId))
        {
            return new ShortestPathResultDto { FromNodeId = fromNodeId, ToNodeId = toNodeId, Found = false };
        }

        if (fromNodeId == toNodeId)
        {
            return new ShortestPathResultDto { FromNodeId = fromNodeId, ToNodeId = toNodeId, Found = true, TotalDistance = 0, PathNodes = [MapNode(graph.NodeIndex[fromNodeId])] };
        }

        // 2. Setup
        var distances = graph.Nodes.ToDictionary(n => n.Id, _ => double.PositiveInfinity);
        var previousNode = new Dictionary<string, string>();
        var previousRoad = new Dictionary<string, long>();
        var visited = new HashSet<string>();
        var queue = new PriorityQueue<string, double>();

        distances[fromNodeId] = 0;
        queue.Enqueue(fromNodeId, 0);

        // 3. Search Loop
        while (queue.Count > 0)
        {
            string curr = queue.Dequeue();
            if (!visited.Add(curr))
            {
                continue;
            }


            if (curr == toNodeId)
            {
                break;
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
                
                // Adjustment: physical distance * traffic impact
                double trafficFactor = GetTrafficAdjustment(edge, trafficByRoadId, periodMultiplier);
                double adjustedDist = edge.Distance * trafficFactor;
                double newDist = distances[curr] + adjustedDist;

                if (newDist < distances[neighbor])
                {
                    distances[neighbor] = newDist;
                    previousNode[neighbor] = curr;
                    previousRoad[neighbor] = edge.Id;
                    queue.Enqueue(neighbor, newDist);
                }
            }
        }

        // 4. Result
        if (!double.IsFinite(distances[toNodeId]))
        {
            return new ShortestPathResultDto { FromNodeId = fromNodeId, ToNodeId = toNodeId, Found = false };
        }

        var nodePath = new List<string>(); 
        var roadPath = new List<long>();
        string pathCurr = toNodeId;
        nodePath.Add(pathCurr);
        while (previousNode.TryGetValue(pathCurr, out string? prev)) { roadPath.Add(previousRoad[pathCurr]); nodePath.Add(prev); pathCurr = prev; }
        nodePath.Reverse(); roadPath.Reverse();

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

    private static double GetTrafficAdjustment(GraphEdge edge, Dictionary<long, int> trafficByRoadId, double multiplier)
    {
        long roadId = Math.Abs(edge.Id);
        int flow = trafficByRoadId.TryGetValue(roadId, out int f) ? f : 0;
        double ratio = (double)flow / Math.Max(edge.Capacity, 1);
        
        // Threshold-based multiplier adjustment
        if (ratio <= 0.75)
        {
            return multiplier; // Free flow
        }


        if (ratio <= 1.0)
        {
            return multiplier * 1.1; // Moderate
        }


        if (ratio <= 1.25)
        {
            return multiplier * 1.2; // Heavy
        }


        return multiplier * 1.35; // Oversaturated
    }

    private static ShortestPathNodeDto MapNode(GraphNode n) => new() { Id = n.Id, Name = n.Name, Type = n.Type, X = n.X, Y = n.Y, Population = n.Population, IsCritical = n.IsCritical };
    private static ShortestPathRoadDto MapRoad(GraphEdge e) => new() { Id = Math.Abs(e.Id), FromNodeId = e.FromNodeId, ToNodeId = e.ToNodeId, Distance = e.Distance, Capacity = e.Capacity, Condition = e.Condition, IsExisting = e.IsExisting, ConstructionCost = e.ConstructionCost };
}
