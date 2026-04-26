using CairoTransportation.Services.Algorithms.Common.DTOs;
using CairoTransportation.Services.Graph;

namespace CairoTransportation.Algorithms.ShortestPath.Contracts;

/// <summary>
/// Pure algorithm for time-varying shortest path calculations.
/// </summary>
public interface ITimeVaryingRoutePlanner
{
    /// <summary>
    /// Finds a traffic-aware shortest path using Dijkstra.
    /// </summary>
    ShortestPathResultDto FindShortestPath(
        Graph graph, 
        string fromNodeId, 
        string toNodeId, 
        Dictionary<long, int> trafficByRoadId, 
        double periodMultiplier);
}
