using CairoTransportation.Services.Algorithms.Common.DTOs;
using CairoTransportation.Services.Algorithms.Common.Instrumentation;
using CairoTransportation.Services.Algorithms.Mst.Contracts;
using CairoTransportation.Services.Algorithms.Mst.DTOs;
using CairoTransportation.Services.Graph;

namespace CairoTransportation.Services.Algorithms.Mst;

public class MstService(IGraphService graphService) : IMstService
{
    private sealed record UndirectedEdge(string A, string B, double Cost, GraphEdge Representative);

    public async Task<AlgorithmResponseDto<MstResultDto>> BuildCheapestNetworkAsync()
    {
        AlgorithmExecutionMetrics metrics = new();
        CairoTransportation.Services.Graph.Graph graph = await graphService.GetGraphAsync(includePotentialRoads: true);

        if (graph.NodeCount == 0)
        {
            return CreateFailureResponse("Graph contains no nodes.", metrics, new MstResultDto
            {
                Connected = true,
                TotalConstructionCost = 0,
                TotalNodes = 0,
                SelectedRoadCount = 0
            });
        }

        // Build undirected edge set (avoid duplicates from two-way expansion).
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

            double cost = GetConstructionCost(edge);
            if (!double.IsFinite(cost))
            {
                continue;
            }

            (string a, string b) = NormalizePair(edge.FromNodeId, edge.ToNodeId);
            edgeByRoadId[roadId] = new UndirectedEdge(a, b, cost, edge);
        }

        Dictionary<string, List<UndirectedEdge>> adjacency = [];
        foreach (GraphNode node in graph.Nodes)
        {
            adjacency[node.Id] = [];
        }

        foreach (UndirectedEdge e in edgeByRoadId.Values)
        {
            adjacency[e.A].Add(e);
            adjacency[e.B].Add(e);
        }

        HashSet<string> visited = [];
        PriorityQueue<UndirectedEdge, double> frontier = new();
        List<GraphEdge> selectedEdges = [];
        double totalCost = 0;

        string startNodeId = graph.Nodes[0].Id;
        visited.Add(startNodeId);
        metrics.MarkDiscovered(startNodeId);

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

            if (aVisited && bVisited)
            {
                continue;
            }

            string next = aVisited ? candidate.B : candidate.A;
            if (!visited.Add(next))
            {
                continue;
            }

            metrics.MarkDiscovered(next);
            selectedEdges.Add(candidate.Representative);
            totalCost += candidate.Cost;

            foreach (UndirectedEdge e in adjacency[next])
            {
                bool eAVisited = visited.Contains(e.A);
                bool eBVisited = visited.Contains(e.B);
                if (eAVisited && eBVisited)
                {
                    continue;
                }

                frontier.Enqueue(e, e.Cost);
            }
        }

        bool connected = visited.Count == graph.NodeCount;

        MstResultDto result = new()
        {
            Connected = connected,
            TotalConstructionCost = totalCost,
            TotalNodes = graph.NodeCount,
            SelectedRoadCount = selectedEdges.Count,
            Nodes = graph.Nodes.Select(MapNode).ToList(),
            SelectedRoads = selectedEdges.Select(MapRoad).ToList()
        };

        if (!connected)
        {
            return CreateFailureResponse(
                "Graph is disconnected; a spanning tree covering all cities could not be built.",
                metrics,
                result);
        }

        return CreateSuccessResponse(result, "Cheapest network built using MST (Prim's algorithm).", metrics);
    }

    private static (string a, string b) NormalizePair(string x, string y)
        => string.CompareOrdinal(x, y) <= 0 ? (x, y) : (y, x);

    private static double GetConstructionCost(GraphEdge edge)
    {
        if (edge.IsExisting)
        {
            return 0;
        }

        return edge.ConstructionCost ?? double.PositiveInfinity;
    }

    private static AlgorithmResponseDto<MstResultDto> CreateSuccessResponse(
        MstResultDto result,
        string message,
        AlgorithmExecutionMetrics metrics) =>
        new()
        {
            AlgorithmName = "MST",
            Success = true,
            Message = message,
            Trace = metrics.Complete(),
            Data = result
        };

    private static AlgorithmResponseDto<MstResultDto> CreateFailureResponse(
        string message,
        AlgorithmExecutionMetrics metrics,
        MstResultDto result) =>
        new()
        {
            AlgorithmName = "MST",
            Success = false,
            Message = message,
            Trace = metrics.Complete(),
            Data = result
        };

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

