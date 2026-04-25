using CairoTransportation.Models;
using CairoTransportation.Services;
using Microsoft.AspNetCore.Mvc;

namespace CairoTransportation.Controllers;

/// <summary>
/// Provides access to location data such as neighborhoods and facilities.
/// Use this controller when you need to browse the transportation network nodes.
/// </summary>
[ApiController]
[Route("api/city-locations")]
[Route("api/locations")]
public class LocationsController(ILocationService locationService) : ControllerBase
{
    /// <summary>
    /// Gets all locations in the transportation network.
    /// </summary>
    /// <remarks>
    /// Use this endpoint when you want to list every node in the network,
    /// such as for map views, dropdowns, or algorithm inputs.
    /// </remarks>
    /// <returns>A list of all locations.</returns>
    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await locationService.GetAllAsync());

    /// <summary>
    /// Gets a single location by its identifier.
    /// </summary>
    /// <remarks>
    /// Use this endpoint when you need one specific node, for example before
    /// creating a route or inspecting a place in the network.
    /// </remarks>
    /// <param name="id">The location identifier.</param>
    /// <returns>The matching location if found; otherwise 404 Not Found.</returns>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        Location? location = await locationService.GetByIdAsync(id);
        return location is null ? NotFound() : Ok(location);
    }
}

