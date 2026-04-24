using CairoTransportation.Services;
using Microsoft.AspNetCore.Mvc;

namespace CairoTransportation.Controllers;

[ApiController]
[Route("api/traffic")]
public class TrafficController(ITrafficService trafficService) : ControllerBase
{
    [HttpGet("road/{roadId:long}")]
    public async Task<IActionResult> GetByRoadId(long roadId) => Ok(await trafficService.GetByRoadIdAsync(roadId));

    [HttpGet("period/{period}")]
    public async Task<IActionResult> GetByPeriod(string period) => Ok(await trafficService.GetByPeriodAsync(period));
}
