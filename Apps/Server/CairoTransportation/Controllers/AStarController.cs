using CairoTransportation.Services.Algorithms.AStar.Contracts;
using CairoTransportation.Services.Algorithms.AStar.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace CairoTransportation.Controllers;

[ApiController]
[Route("api/algorithms/a-star")]
public class AStarController(IAStarService aStarService) : ControllerBase
{
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
