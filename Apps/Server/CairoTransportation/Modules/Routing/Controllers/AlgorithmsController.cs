using CairoTransportation.Services;
using CairoTransportation.Services.Algorithms.Common.DTOs;
using CairoTransportation.Services.Routing.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace CairoTransportation.Controllers;

/// <summary>
/// Provides algorithm endpoints for network routing and optimization.
/// Use this controller when you need shortest-path calculations.
/// </summary>
[ApiController]
[Route("api/route-planning")]
public class AlgorithmsController(
    IDijkstraService dijkstraService,
    ITimeVaryingDijkstraService timeVaryingDijkstraService,
    ITrafficService trafficService) : ControllerBase
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

    /// <summary>
    /// Finds a shortest path using Dijkstra with period-based traffic multipliers.
    /// </summary>
    /// <param name="from">The starting node identifier.</param>
    /// <param name="to">The destination node identifier.</param>
    /// <param name="period">Traffic period configured in database multipliers table.</param>
    /// <returns>A traffic-aware shortest path result with standardized trace metrics.</returns>
    [HttpGet("time-route")]
    public async Task<IActionResult> GetTimeVaryingShortestPath([FromQuery] string from, [FromQuery] string to, [FromQuery] string period)
    {
        if (string.IsNullOrWhiteSpace(from) || string.IsNullOrWhiteSpace(to) || string.IsNullOrWhiteSpace(period))
        {
            return BadRequest(new AlgorithmResponseDto<ShortestPathResultDto>
            {
                AlgorithmName = "Time-Varying Dijkstra",
                Success = false,
                Message = "'from', 'to', and 'period' query parameters are required.",
                Data = new ShortestPathResultDto
                {
                    FromNodeId = from ?? string.Empty,
                    ToNodeId = to ?? string.Empty,
                    Found = false
                }
            });
        }

        string normalizedPeriod = period.Trim().ToUpperInvariant();
        if (await trafficService.GetPeriodMultiplierAsync(normalizedPeriod) is null)
        {
            return BadRequest(new AlgorithmResponseDto<ShortestPathResultDto>
            {
                AlgorithmName = "Time-Varying Dijkstra",
                Success = false,
                Message = $"Unsupported period '{period}'. No multiplier is configured in database.",
                Data = new ShortestPathResultDto
                {
                    FromNodeId = from,
                    ToNodeId = to,
                    Found = false
                }
            });
        }

        AlgorithmResponseDto<ShortestPathResultDto> result = await timeVaryingDijkstraService.FindShortestPathAsync(from, to, normalizedPeriod);
        return result.Success ? Ok(result) : NotFound(result);
    }
}

