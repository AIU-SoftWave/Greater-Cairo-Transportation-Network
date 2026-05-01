using CairoTransportation.Utils.Helpers.Common.DTOs;
using CairoTransportation.Utils.Helpers.Graph;

namespace CairoTransportation.Utils.Algorithms.NetworkExpansion.Contracts;

/// <summary>
/// Pure algorithm implementation for Minimum Spanning Tree using Prim's algorithm.
/// Considers construction costs and prioritizes high-population/critical areas.
/// </summary>
public interface IPrimNetworkExpander
{
    /// <summary>
    /// Executes Prim's algorithm on the provided graph.
    /// </summary>
    /// <param name="graph">The transportation network graph.</param>
    /// <returns>An MST result containing selected roads and total cost.</returns>
    MstResultDto BuildCheapestNetwork(Graph graph);
}
