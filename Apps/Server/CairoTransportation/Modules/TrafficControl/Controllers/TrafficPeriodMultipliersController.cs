using CairoTransportation.Modules.TrafficControl.Models;
using CairoTransportation.Modules.TrafficControl.Services;
using Microsoft.AspNetCore.Mvc;

namespace CairoTransportation.Modules.TrafficControl.Controllers;

/// <summary>
/// Provides read access to configured traffic period multipliers used by time-varying routing.
/// </summary>
[ApiController]
[Route("api/traffic-policy")]
public class TrafficPeriodMultipliersController(ITrafficService trafficService) : ControllerBase
{
    /// <summary>
    /// Returns all configured traffic period multipliers.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await trafficService.GetPeriodMultipliersAsync());

    /// <summary>
    /// Returns one traffic period multiplier by period key.
    /// </summary>
    [HttpGet("{period}")]
    public async Task<IActionResult> GetByPeriod(string period)
    {
        if (string.IsNullOrWhiteSpace(period))
        {
            return BadRequest("'period' route parameter is required.");
        }

        string normalizedPeriod = period.Trim().ToUpperInvariant();
        TrafficPeriodMultiplier? result = await trafficService.GetPeriodMultiplierAsync(normalizedPeriod);
        return result is null ? NotFound($"Period '{period}' was not found.") : Ok(result);
    }
}

