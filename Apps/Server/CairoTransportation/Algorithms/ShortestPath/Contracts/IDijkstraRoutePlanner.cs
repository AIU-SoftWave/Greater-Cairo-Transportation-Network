using CairoTransportation.Services.Algorithms.Common.DTOs;
using CairoTransportation.Services.Graph;

namespace CairoTransportation.Algorithms.ShortestPath.Contracts;

/// <summary>
/// Pure Dijkstra's algorithm for finding shortest paths in a weighted graph.
/// </summary>
public interface IDijkstraRoutePlanner
{
    /// <summary>
    /// Executes Dijkstra's algorithm on the provided graph.
    /// </summary>
    ShortestPathResultDto FindShortestPath(Graph graph, string fromNodeId, string toNodeId);
}
