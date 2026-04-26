using CairoTransportation.Algorithms.ShortestPath.Contracts;
using CairoTransportation.Services.Algorithms.Common.DTOs;
using CairoTransportation.Services.Graph;

namespace CairoTransportation.Algorithms.ShortestPath;

public class AStarPathFinder : IAStarPathFinder
{
    public ShortestPathResultDto FindShortestPath(Graph graph, string fromNodeId, string toNodeId)
    {
        if (!graph.NodeIndex.ContainsKey(fromNodeId) || !graph.NodeIndex.ContainsKey(toNodeId))
        {
            return new ShortestPathResultDto { FromNodeId = fromNodeId, ToNodeId = toNodeId, Found = false };
        }

        return RunAStar(graph, fromNodeId, toNodeId);
    }

    public ShortestPathResultDto FindNearestMedicalFacility(Graph graph, string fromNodeId)
    {
        var facilities = graph.Nodes
            .Where(n => n.IsCritical || n.Type.Equals("FACILITY", StringComparison.OrdinalIgnoreCase))
            .Where(n => n.Id != fromNodeId)
            .ToList();

        if (facilities.Count == 0)
        {
            return new ShortestPathResultDto { FromNodeId = fromNodeId, Found = false };
        }

        ShortestPathResultDto? best = null;
        foreach (GraphNode facility in facilities)
        {
            ShortestPathResultDto candidate = RunAStar(graph, fromNodeId, facility.Id);
            if (candidate.Found && (best == null || candidate.TotalDistance < best.TotalDistance))
            {
                best = candidate;
            }
        }

        return best ?? new ShortestPathResultDto { FromNodeId = fromNodeId, Found = false };
    }

    private static ShortestPathResultDto RunAStar(Graph graph, string fromNodeId, string toNodeId)
    {
        if (fromNodeId == toNodeId)
        {
            return new ShortestPathResultDto { FromNodeId = fromNodeId, ToNodeId = toNodeId, Found = true, TotalDistance = 0, PathNodes = [MapNode(graph.NodeIndex[fromNodeId])] };
        }

        var gScore = graph.Nodes.ToDictionary(n => n.Id, _ => double.PositiveInfinity);
        var cameFromNode = new Dictionary<string, string>();
        var cameFromRoad = new Dictionary<string, long>();
        var openSet = new PriorityQueue<string, double>();
        var closedSet = new HashSet<string>();

        gScore[fromNodeId] = 0;
        // Priority = gScore (cost so far) + hScore (estimated cost to destination)
        openSet.Enqueue(fromNodeId, Heuristic(graph, fromNodeId, toNodeId));

        while (openSet.Count > 0)
        {
            string curr = openSet.Dequeue();
            if (!closedSet.Add(curr))
            {
                continue;
            }


            if (curr == toNodeId)
            {
                break;
            }


            if (!graph.AdjacencyList.TryGetValue(curr, out List<long>? edges))
            {
                continue;
            }


            foreach (long edgeId in edges)
            {
                if (!graph.EdgeIndex.TryGetValue(edgeId, out GraphEdge? edge))
                {
                    continue;
                }


                string neighbor = edge.ToNodeId;
                if (closedSet.Contains(neighbor))
                {
                    continue;
                }


                double tentG = gScore[curr] + edge.Distance;
                if (tentG < gScore[neighbor])
                {
                    cameFromNode[neighbor] = curr;
                    cameFromRoad[neighbor] = edge.Id;
                    gScore[neighbor] = tentG;
                    // fScore = gScore + heuristic
                    openSet.Enqueue(neighbor, tentG + Heuristic(graph, neighbor, toNodeId));
                }
            }
        }

        if (!double.IsFinite(gScore[toNodeId]))
        {
            return new ShortestPathResultDto { FromNodeId = fromNodeId, ToNodeId = toNodeId, Found = false };
        }

        var nodePath = new List<string>(); 
        var roadPath = new List<long>();
        string pathCurr = toNodeId;
        
        nodePath.Add(pathCurr);
        while (cameFromNode.TryGetValue(pathCurr, out string? prev)) 
        { 
            roadPath.Add(cameFromRoad[pathCurr]); 
            nodePath.Add(prev); 
            pathCurr = prev; 
        }
        
        nodePath.Reverse(); 
        roadPath.Reverse();

        return new ShortestPathResultDto 
        { 
            FromNodeId = fromNodeId, 
            ToNodeId = toNodeId, 
            Found = true, 
            TotalDistance = gScore[toNodeId], 
            PathNodes = nodePath.Select(id => MapNode(graph.NodeIndex[id])).ToList(), 
            PathRoads = roadPath.Select(id => MapRoad(graph.EdgeIndex[id])).ToList() 
        };
    }

    private static double Heuristic(Graph graph, string fromId, string toId)
    {
        GraphNode f = graph.NodeIndex[fromId];
        GraphNode t = graph.NodeIndex[toId];
        if (f.X == null || f.Y == null || t.X == null || t.Y == null)
        {
            return 0;
        }


        double dx = f.X.Value - t.X.Value; double dy = f.Y.Value - t.Y.Value;
        return Math.Sqrt(dx * dx + dy * dy);
    }

    private static ShortestPathNodeDto MapNode(GraphNode n) => new() { Id = n.Id, Name = n.Name, Type = n.Type, X = n.X, Y = n.Y, Population = n.Population, IsCritical = n.IsCritical };
    private static ShortestPathRoadDto MapRoad(GraphEdge e) => new() { Id = Math.Abs(e.Id), FromNodeId = e.FromNodeId, ToNodeId = e.ToNodeId, Distance = e.Distance, Capacity = e.Capacity, Condition = e.Condition, IsExisting = e.IsExisting, ConstructionCost = e.ConstructionCost };
}
