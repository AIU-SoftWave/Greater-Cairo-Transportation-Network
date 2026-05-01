using CairoTransportation.Utils.Helpers.Common.DTOs;
using CairoTransportation.Utils.Helpers.Graph;

namespace CairoTransportation.Utils.Algorithms.ShortestPath.Contracts;

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
