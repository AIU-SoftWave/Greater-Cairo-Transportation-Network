using CairoTransportation.Modules.TrafficControl.Services;
using Microsoft.AspNetCore.Mvc;

namespace CairoTransportation.Modules.TrafficControl.Controllers;

/// <summary>
/// Provides access to traffic flow data for roads and time periods.
/// Use this controller when you need congestion information for routing.
/// </summary>
[ApiController]
[Route("api/traffic-monitoring")]
public class TrafficController(ITrafficService trafficService) : ControllerBase
{
    /// <summary>
    /// Gets all traffic flow records for a specific road.
    /// </summary>
    /// <remarks>
    /// Use this endpoint when you want to see traffic samples for one road,
    /// for example while building traffic-aware route calculations.
    /// </remarks>
    /// <param name="roadId">The road identifier.</param>
    /// <returns>A list of traffic flow entries for the road.</returns>
    [HttpGet("road/{roadId:long}")]
    public async Task<IActionResult> GetByRoadId(long roadId) => Ok(await trafficService.GetByRoadIdAsync(roadId));

    /// <summary>
    /// Gets all traffic flow records for a specific time period.
    /// </summary>
    /// <remarks>
    /// Use this endpoint when you need traffic information for a time slot
    /// such as morning or evening for congestion-aware routing.
    /// </remarks>
    /// <param name="period">The traffic period name.</param>
    /// <returns>A list of traffic flow entries for the period.</returns>
    [HttpGet("period/{period}")]
    public async Task<IActionResult> GetByPeriod(string period)
    {
        if (string.IsNullOrWhiteSpace(period))
        {
            return BadRequest("'period' route parameter is required.");
        }

        string normalizedPeriod = period.Trim().ToUpperInvariant();
        if (await trafficService.GetPeriodMultiplierAsync(normalizedPeriod) is null)
        {
            return BadRequest($"Unsupported period '{period}'. No multiplier is configured in database.");
        }

        return Ok(await trafficService.GetByPeriodAsync(normalizedPeriod));
    }
}

