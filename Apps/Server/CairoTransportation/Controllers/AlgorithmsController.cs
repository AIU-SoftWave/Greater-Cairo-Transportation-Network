using CairoTransportation.Services.Algorithms.Dijkstra.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace CairoTransportation.Controllers;

/// <summary>
/// Provides algorithm endpoints for network routing and optimization.
/// Use this controller when you need shortest-path calculations.
/// </summary>
[ApiController]
[Route("api/algorithms")]
public class AlgorithmsController(IDijkstraService dijkstraService) : ControllerBase
{
    /// <summary>
    /// Finds the shortest path between two nodes using Dijkstra's algorithm.
    /// </summary>
    /// <remarks>
    /// Use this endpoint when you want the best general-purpose route between two places.
    /// It is appropriate for normal route planning when you want the cheapest path by distance.
    /// </remarks>
    /// <param name="from">The starting node identifier.</param>
    /// <param name="to">The destination node identifier.</param>
    /// <returns>A rich route result containing the path, road list, total distance, and status.</returns>
    [HttpGet("shortest-path")]
    public async Task<IActionResult> GetShortestPath([FromQuery] string from, [FromQuery] string to)
    {
        if (string.IsNullOrWhiteSpace(from) || string.IsNullOrWhiteSpace(to))
            return BadRequest("Both 'from' and 'to' query parameters are required.");

        var result = await dijkstraService.FindShortestPathAsync(from, to);
        return result.Found ? Ok(result) : NotFound(result);
    }
}
