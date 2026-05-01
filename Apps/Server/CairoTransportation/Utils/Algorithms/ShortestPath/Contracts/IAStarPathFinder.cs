using CairoTransportation.Utils.Helpers.Common.DTOs;
using CairoTransportation.Utils.Helpers.Graph;

namespace CairoTransportation.Utils.Algorithms.ShortestPath.Contracts;

/// <summary>
/// Pure A* search algorithm for emergency routing.
/// </summary>
public interface IAStarPathFinder
{
    /// <summary>
    /// Finds the shortest path using A* search (Euclidean heuristic).
    /// </summary>
    ShortestPathResultDto FindShortestPath(Graph graph, string fromNodeId, string toNodeId);

    /// <summary>
    /// Finds the nearest medical facility from the origin.
    /// </summary>
    ShortestPathResultDto FindNearestMedicalFacility(Graph graph, string fromNodeId);
}
