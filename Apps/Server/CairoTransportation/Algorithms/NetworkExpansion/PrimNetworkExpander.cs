using CairoTransportation.Algorithms.NetworkExpansion.Contracts;
using CairoTransportation.Services.Algorithms.Mst.DTOs;
using CairoTransportation.Services.Algorithms.Common.DTOs;
using CairoTransportation.Services.Graph;

using CairoTransportation.Services.Algorithms.Common.Instrumentation;

namespace CairoTransportation.Algorithms.NetworkExpansion;

public class PrimNetworkExpander(AlgorithmExecutionMetrics metrics) : IPrimNetworkExpander
{
    private sealed record UndirectedEdge(string A, string B, double Cost, GraphEdge Representative);

    public MstResultDto BuildCheapestNetwork(Graph graph)
    {
        // 1. Edge case: Empty network
        if (graph.NodeCount == 0)
        {
            return new MstResultDto { Connected = true };
        }

        // 2. Prepare edges: Prim's requires undirected edges with custom weights
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
            
            // Weight = Construction Cost adjusted by population priority
            double cost = GetMstWeight(edge, from, to);
            if (!double.IsFinite(cost))
            {
                continue;
            }

            (string a, string b) = NormalizePair(edge.FromNodeId, edge.ToNodeId);
            edgeByRoadId[roadId] = new UndirectedEdge(a, b, cost, edge);
        }

        // 3. Build adjacency map for quick neighbor lookup
        var adjacency = graph.Nodes.ToDictionary(n => n.Id, _ => new List<UndirectedEdge>());
        foreach (UndirectedEdge e in edgeByRoadId.Values)
        {
            adjacency[e.A].Add(e);
            adjacency[e.B].Add(e);
        }

        // 4. Main Prim's Logic: connect nodes one by one using the cheapest available edge
        HashSet<string> visited = [];
        PriorityQueue<UndirectedEdge, double> frontier = new();
        List<GraphEdge> selectedEdges = [];
        double totalCost = 0;

        // Start from an arbitrary node (the first one)
        string startNodeId = graph.Nodes[0].Id;
        visited.Add(startNodeId);
        metrics.MarkDiscovered(startNodeId);

        // Add all roads connected to the starting node to the "frontier"
        foreach (UndirectedEdge e in adjacency[startNodeId])
        {
            frontier.Enqueue(e, e.Cost);
        }

        while (frontier.Count > 0 && visited.Count < graph.NodeCount)
        {
            UndirectedEdge candidate = frontier.Dequeue();
            metrics.MarkExpanded();

            bool aVisited = visited.Contains(candidate.A);
            bool bVisited = visited.Contains(candidate.B);

            // Skip if this road connects two nodes already in our network (prevents cycles)
            if (aVisited && bVisited)
            {
                continue;
            }

            // The node we are now connecting to the network
            string next = aVisited ? candidate.B : candidate.A;
            if (!visited.Add(next))
            {
                continue;
            }
            metrics.MarkDiscovered(next);

            selectedEdges.Add(candidate.Representative);
            
            // IMPORTANT: Count actual construction cost (not the weight used for Prim's selection)
            // This way we show the real cost to build selected roads
            double actualCost = candidate.Representative.IsExisting ? 0 : (candidate.Representative.ConstructionCost ?? 0);
            totalCost += actualCost;

            // Add new possible roads from the newly connected node to the frontier
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

    // Weight Calculation: Lower weight means higher priority for construction
    private static double GetMstWeight(GraphEdge edge, GraphNode from, GraphNode to)
    {
        // Strategy: Balance between using existing roads and expanding with potential roads
        // Use a blended approach that considers distance, capacity, condition, and construction cost
        
        double weight;
        
        if (edge.IsExisting)
        {
            // For existing roads: Use distance + capacity efficiency as weight
            // This keeps them low-cost but not zero, allowing comparison with potential roads
            // Weight = (Distance / Capacity) - lower means better efficiency
            weight = edge.Distance / edge.Capacity;
            
            // Apply slight adjustment based on condition
            if (edge.Condition.HasValue && edge.Condition > 0)
            {
                weight = weight / (1 + (edge.Condition.Value / 10.0)); // Better condition = lower weight
            }
        }
        else
        {
            // For potential roads: Use construction cost normalized by distance and capacity
            // This balances cost efficiency with network capacity
            double baseCost = edge.ConstructionCost ?? double.PositiveInfinity;
            
            if (double.IsInfinity(baseCost))
            {
                return baseCost;
            }
            
            // Normalize cost by distance and capacity for fair comparison
            // Weight = (Cost / Distance) / Capacity - lower means better value
            weight = (baseCost / Math.Max(edge.Distance, 0.1)) / edge.Capacity;
            
            // Apply strategic priority multipliers for critical paths
            double priorityMultiplier = 1.0;
            
            // Critical facilities: significantly reduce weight to encourage selection
            if (from.IsCritical || to.IsCritical)
            {
                priorityMultiplier *= 0.5; // 50% reduction for critical connections
            }
            
            // High-population areas: reduce weight to ensure good connectivity
            if ((from.Population ?? 0) > 350000 || (to.Population ?? 0) > 350000)
            {
                priorityMultiplier *= 0.7; // 30% reduction for major population centers
            }
            
            weight = weight * priorityMultiplier;
        }

        return weight;
    }

    private static (string a, string b) NormalizePair(string x, string y)
        => string.CompareOrdinal(x, y) <= 0 ? (x, y) : (y, x);

    private static ShortestPathNodeDto MapNode(GraphNode n) => new() { Id = n.Id, Name = n.Name, Type = n.Type, X = n.X, Y = n.Y, Population = n.Population, IsCritical = n.IsCritical };
    private static ShortestPathRoadDto MapRoad(GraphEdge e) => new() { Id = Math.Abs(e.Id), FromNodeId = e.FromNodeId, ToNodeId = e.ToNodeId, Distance = e.Distance, Capacity = e.Capacity, Condition = e.Condition, IsExisting = e.IsExisting, ConstructionCost = e.ConstructionCost };
}
