using CairoTransportation.Modules.Simulation.Services;
using Microsoft.AspNetCore.Mvc;

namespace CairoTransportation.Modules.Simulation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SimulationController(ISimulationService simulationService) : ControllerBase
{
    [HttpPost("toggle-road-closure/{id}")]
    public async Task<IActionResult> ToggleRoadClosure(long id)
    {
        await simulationService.ToggleRoadClosureAsync(id);
        return Ok(new { Success = true, Message = $"Road {id} status toggled." });
    }

    [HttpPost("reset")]
    public async Task<IActionResult> Reset()
    {
        await simulationService.ResetClosuresAsync();
        return Ok(new { Success = true, Message = "Simulation reset." });
    }

    [HttpGet("closed-roads")]
    public async Task<IActionResult> GetClosedRoads()
    {
        HashSet<long> ids = await simulationService.GetClosedRoadIdsAsync();
        return Ok(ids);
    }

    [HttpPost("preemption/{id}")]
    public async Task<IActionResult> SetPreemption(long id, [FromQuery] bool active)
    {
        await simulationService.SetEmergencyPreemptionAsync(id, active);
        return Ok(new { Success = true, Message = $"Preemption for road {id} set to {active}." });
    }

    [HttpGet("metrics")]
    public IActionResult GetMetrics() => Ok(simulationService.GetPerformanceMetrics());

    [HttpPost("weather")]
    public async Task<IActionResult> SetWeather([FromQuery] SimulationWeather weather)
    {
        await simulationService.SetWeatherAsync(weather);
        return Ok(new { Success = true, Message = $"Weather set to {weather}." });
    }
}
