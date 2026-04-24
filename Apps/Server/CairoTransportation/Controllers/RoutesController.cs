using CairoTransportation.Models;
using CairoTransportation.Services;
using Microsoft.AspNetCore.Mvc;

namespace CairoTransportation.Controllers;

[ApiController]
[Route("api/routes")]
public class RoutesController(IRouteService routeService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await routeService.GetAllAsync());

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        TransportRoute? route = await routeService.GetByIdAsync(id);
        return route is null ? NotFound() : Ok(route);
    }

    [HttpGet("{id}/stops")]
    public async Task<IActionResult> GetStops(string id) => Ok(await routeService.GetStopsByRouteIdAsync(id));
}
