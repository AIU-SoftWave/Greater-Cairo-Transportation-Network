using CairoTransportation.Services.Algorithms.Common.DTOs;
using CairoTransportation.Services.Algorithms.Dijkstra.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace CairoTransportation.Controllers;

[ApiController]
[Route("api/algorithms")]
public class AlgorithmsController(IDijkstraService dijkstraService) : ControllerBase
{
    [HttpGet("shortest-path")]
    [HttpGet("dijkstra/shortest-path")]
    public async Task<IActionResult> GetShortestPath([FromQuery] string from, [FromQuery] string to)
    {
        if (string.IsNullOrWhiteSpace(from) || string.IsNullOrWhiteSpace(to))
        {
            return BadRequest(new AlgorithmResponseDto<ShortestPathResultDto>
            {
                AlgorithmName = "Dijkstra",
                Success = false,
                Message = "Both 'from' and 'to' query parameters are required.",
                Data = new ShortestPathResultDto
                {
                    FromNodeId = from ?? string.Empty,
                    ToNodeId = to ?? string.Empty,
                    Found = false
                }
            });
        }

        AlgorithmResponseDto<ShortestPathResultDto> result = await dijkstraService.FindShortestPathAsync(from, to);
        return result.Success ? Ok(result) : NotFound(result);
    }
}
