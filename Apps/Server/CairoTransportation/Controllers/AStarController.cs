using CairoTransportation.Services.Algorithms.AStar.Contracts;
using CairoTransportation.Services.Algorithms.AStar.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace CairoTransportation.Controllers;

/// <summary>
/// Provides A* route search endpoints for targeted and emergency-friendly routing.
/// Use this controller when you want a route search guided by coordinates.
/// </summary>
[ApiController]
[Route("api/algorithms/a-star")]
public class AStarController(IAStarService aStarService) : ControllerBase
{
    /// <summary>
    /// Finds the shortest path using A* search.
    /// </summary>
    /// <remarks>
    /// Use this endpoint when you want a route search that is more target-directed than Dijkstra.
    /// It is especially useful for emergency routing, fast target-focused search, and map-based navigation,
    /// because it uses node coordinates to guide the search toward the destination.
    /// </remarks>
    /// <param name="from">The starting node identifier.</param>
    /// <param name="to">The destination node identifier.</param>
    /// <returns>A rich route result containing the path, road list, total distance, and status.</returns>
    [HttpGet]
    public async Task<IActionResult> GetShortestPath([FromQuery] string from, [FromQuery] string to)
    {
        if (string.IsNullOrWhiteSpace(from) || string.IsNullOrWhiteSpace(to))
        {
            return BadRequest("Both 'from' and 'to' query parameters are required.");
        }

        ShortestPathResultDto result = await aStarService.FindShortestPathAsync(from, to);
        return result.Found ? Ok(result) : NotFound(result);
    }
}
