using CairoTransportation.Algorithms.NetworkExpansion.Contracts;
using CairoTransportation.Services.Algorithms.Mst.DTOs;
using CairoTransportation.Services.Algorithms.Common.DTOs;
using CairoTransportation.Services.Graph;

namespace CairoTransportation.Algorithms.NetworkExpansion;

public class PrimNetworkExpander : IPrimNetworkExpander
{
    private sealed record UndirectedEdge(string A, string B, double Cost, GraphEdge Representative);

    public MstResultDto BuildCheapestNetwork(Graph graph)
    {
        if (graph.NodeCount == 0)
        {
            return new MstResultDto { Connected = true };
        }

        // 1. Prepare undirected representation with custom weights

        Dictionary<long, UndirectedEdge> edgeByRoadId = [];
        foreach (GraphEdge edge in graph.Edges)
        {
            if (!graph.NodeIndex.ContainsKey(edge.FromNodeId) || !graph.NodeIndex.ContainsKey(edge.ToNodeId))
            {
                continue;
            }


            long roadId = Math.Abs(edge.Id);
            if (edgeByRoadId.ContainsKey(roadId))
            {
                continue;
            }


            GraphNode from = graph.NodeIndex[edge.FromNodeId];
            GraphNode to = graph.NodeIndex[edge.ToNodeId];
            
            // Weight accounts for cost + population priority
            double cost = GetMstWeight(edge, from, to);
            if (!double.IsFinite(cost))
            {
                continue;
            }


            (string a, string b) = NormalizePair(edge.FromNodeId, edge.ToNodeId);
            edgeByRoadId[roadId] = new UndirectedEdge(a, b, cost, edge);
        }

        // 2. Build adjacency for quick lookups
        var adjacency = graph.Nodes.ToDictionary(n => n.Id, _ => new List<UndirectedEdge>());
        foreach (UndirectedEdge e in edgeByRoadId.Values)
        {
            adjacency[e.A].Add(e);
            adjacency[e.B].Add(e);
        }

        // 3. Main Prim's Algorithm Loop
        HashSet<string> visited = [];
        PriorityQueue<UndirectedEdge, double> frontier = new();
        List<GraphEdge> selectedEdges = [];
        double totalCost = 0;

        string startNodeId = graph.Nodes[0].Id;
        visited.Add(startNodeId);

        foreach (UndirectedEdge e in adjacency[startNodeId])
        {
            frontier.Enqueue(e, e.Cost);
        }


        while (frontier.Count > 0 && visited.Count < graph.NodeCount)
        {
            UndirectedEdge candidate = frontier.Dequeue();

            bool aVisited = visited.Contains(candidate.A);
            bool bVisited = visited.Contains(candidate.B);

            if (aVisited && bVisited)
            {
                continue; // Cycle detected
            }


            string next = aVisited ? candidate.B : candidate.A;
            if (!visited.Add(next))
            {
                continue;
            }


            selectedEdges.Add(candidate.Representative);
            totalCost += candidate.Cost;

            foreach (UndirectedEdge e in adjacency[next])
            {
                if (visited.Contains(e.A) && visited.Contains(e.B))
                {
                    continue;
                }


                frontier.Enqueue(e, e.Cost);
            }
        }

        return new MstResultDto
        {
            Connected = visited.Count == graph.NodeCount,
            TotalConstructionCost = totalCost,
            TotalNodes = graph.NodeCount,
            SelectedRoadCount = selectedEdges.Count,
            Nodes = graph.Nodes.Select(MapNode).ToList(),
            SelectedRoads = selectedEdges.Select(MapRoad).ToList()
        };
    }

    private static double GetMstWeight(GraphEdge edge, GraphNode from, GraphNode to)
    {
        double baseCost = edge.IsExisting ? 0 : (edge.ConstructionCost ?? double.PositiveInfinity);
        if (double.IsInfinity(baseCost))
        {
            return baseCost;
        }


        double priorityMultiplier = 1.0;
        
        // Requirement: Prioritize high population or critical facility areas
        if (from.IsCritical || to.IsCritical)
        {
            priorityMultiplier -= 0.25;
        }


        if ((from.Population ?? 0) > 100000 || (to.Population ?? 0) > 100000)
        {
            priorityMultiplier -= 0.2;
        }


        return baseCost * Math.Max(0.1, priorityMultiplier);
    }

    private static (string a, string b) NormalizePair(string x, string y)
        => string.CompareOrdinal(x, y) <= 0 ? (x, y) : (y, x);

    private static ShortestPathNodeDto MapNode(GraphNode n) => new() { Id = n.Id, Name = n.Name, Type = n.Type, X = n.X, Y = n.Y, Population = n.Population, IsCritical = n.IsCritical };
    private static ShortestPathRoadDto MapRoad(GraphEdge e) => new() { Id = Math.Abs(e.Id), FromNodeId = e.FromNodeId, ToNodeId = e.ToNodeId, Distance = e.Distance, Capacity = e.Capacity, Condition = e.Condition, IsExisting = e.IsExisting, ConstructionCost = e.ConstructionCost };
}
