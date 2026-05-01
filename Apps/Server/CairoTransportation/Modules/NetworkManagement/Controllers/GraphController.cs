using CairoTransportation.Utils.Helpers.Graph;
using Microsoft.AspNetCore.Mvc;

namespace CairoTransportation.Modules.NetworkManagement.Controllers;

/// <summary>
/// Provides the graph view of the transportation network for algorithm services.
/// Use this controller when you need the full node-edge structure.
/// </summary>
[ApiController]
[Route("api/network-topology")]
public class GraphController(IGraphService graphService) : ControllerBase
{
    /// <summary>
    /// Gets the complete transportation graph.
    /// </summary>
    /// <remarks>
    /// Use this endpoint when you are building or testing algorithms such as
    /// Dijkstra, A*, MST, or any custom graph traversal logic.
    /// It returns the nodes, edges, adjacency list, and lookup indexes used by algorithms.
    /// </remarks>
    /// <returns>The full graph structure for the transportation network.</returns>
    [HttpGet]
    public async Task<IActionResult> GetGraph() => Ok(await graphService.GetGraphAsync());
}

