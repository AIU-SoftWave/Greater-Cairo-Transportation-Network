using CairoTransportation.Services.Algorithms.Dijkstra.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace CairoTransportation.Controllers;

[ApiController]
[Route("api/algorithms")]
public class AlgorithmsController(IDijkstraService dijkstraService) : ControllerBase
{
    [HttpGet("shortest-path")]
    public async Task<IActionResult> GetShortestPath([FromQuery] string from, [FromQuery] string to)
    {
        if (string.IsNullOrWhiteSpace(from) || string.IsNullOrWhiteSpace(to))
            return BadRequest("Both 'from' and 'to' query parameters are required.");

        var result = await dijkstraService.FindShortestPathAsync(from, to);
        return result.Found ? Ok(result) : NotFound(result);
    }
}
