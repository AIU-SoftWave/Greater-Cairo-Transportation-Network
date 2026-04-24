using CairoTransportation.Models;
using CairoTransportation.Services;
using Microsoft.AspNetCore.Mvc;

namespace CairoTransportation.Controllers;

/// <summary>
/// Provides access to public transport route data and route stops.
/// Use this controller when you need transit network details.
/// </summary>
[ApiController]
[Route("api/routes")]
public class RoutesController(IRouteService routeService) : ControllerBase
{
    /// <summary>
    /// Gets all transit routes.
    /// </summary>
    /// <remarks>
    /// Use this endpoint when you want to browse all metro and bus routes
    /// in the current transport network.
    /// </remarks>
    /// <returns>A list of all transport routes.</returns>
    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await routeService.GetAllAsync());

    /// <summary>
    /// Gets a single route by its identifier.
    /// </summary>
    /// <remarks>
    /// Use this endpoint when you want to inspect one route before showing
    /// its stops or using it in transit analysis.
    /// </remarks>
    /// <param name="id">The route identifier.</param>
    /// <returns>The matching route if found; otherwise 404 Not Found.</returns>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        TransportRoute? route = await routeService.GetByIdAsync(id);
        return route is null ? NotFound() : Ok(route);
    }

    /// <summary>
    /// Gets the ordered stops for a specific route.
    /// </summary>
    /// <remarks>
    /// Use this endpoint when you need the stop sequence for scheduling,
    /// transfer analysis, or route visualization.
    /// </remarks>
    /// <param name="id">The route identifier.</param>
    /// <returns>The list of stops for the route.</returns>
    [HttpGet("{id}/stops")]
    public async Task<IActionResult> GetStops(string id) => Ok(await routeService.GetStopsByRouteIdAsync(id));
}
