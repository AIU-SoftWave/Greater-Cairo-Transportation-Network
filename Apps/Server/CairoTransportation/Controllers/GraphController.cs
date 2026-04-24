using CairoTransportation.Services.Graph;
using Microsoft.AspNetCore.Mvc;

namespace CairoTransportation.Controllers;

/// <summary>
/// API endpoints for accessing transportation network graph data.
/// Used by algorithms to retrieve network structure for computation.
/// </summary>
[ApiController]
[Route("api/graph")]
public class GraphController(IGraphService graphService) : ControllerBase
{
    /// <summary>
    /// Gets the complete transportation network graph with all nodes and edges.
    /// </summary>
    /// <remarks>
    /// Returns:
    /// - All locations as nodes
    /// - All existing roads as edges
    /// - Adjacency lists for efficient neighbor lookup
    /// - Node and edge indexes for O(1) access
    /// </remarks>
    /// <returns>Complete graph with all nodes and edges</returns>
    [HttpGet]
    public async Task<IActionResult> GetGraph() => Ok(await graphService.GetGraphAsync());
}
