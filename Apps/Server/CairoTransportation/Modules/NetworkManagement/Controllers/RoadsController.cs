using CairoTransportation.Models;
using CairoTransportation.Services;
using Microsoft.AspNetCore.Mvc;

namespace CairoTransportation.Controllers;

/// <summary>
/// Provides access to road data and road maintenance information.
/// Use this controller when you need to inspect or query the network edges.
/// </summary>
[ApiController]
[Route("api/road-network")]
[Route("api/roads")]
public class RoadsController(IRoadService roadService) : ControllerBase
{
    /// <summary>
    /// Gets all roads in the network.
    /// </summary>
    /// <remarks>
    /// Use this endpoint when you need the full edge list for analysis,
    /// reporting, or graph construction.
    /// </remarks>
    /// <returns>A list of all roads.</returns>
    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await roadService.GetAllAsync());

    /// <summary>
    /// Gets a single road by its numeric identifier.
    /// </summary>
    /// <remarks>
    /// Use this endpoint to inspect one road, including its distance,
    /// capacity, condition, and whether it is two-way.
    /// </remarks>
    /// <param name="id">The road identifier.</param>
    /// <returns>The matching road if found; otherwise 404 Not Found.</returns>
    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetById(long id)
    {
        Road? road = await roadService.GetByIdAsync(id);
        return road is null ? NotFound() : Ok(road);
    }

    /// <summary>
    /// Gets all roads that start from a specific location.
    /// </summary>
    /// <remarks>
    /// Use this endpoint when you want to see the outgoing connections
    /// from one place in the network.
    /// </remarks>
    /// <param name="locationId">The source location identifier.</param>
    /// <returns>A list of roads starting from the specified location.</returns>
    [HttpGet("from/{locationId}")]
    public async Task<IActionResult> GetByFromLocation(string locationId) => Ok(await roadService.GetByFromLocationIdAsync(locationId));

    /// <summary>
    /// Gets maintenance information for a specific road.
    /// </summary>
    /// <remarks>
    /// Use this endpoint when you need to review repair priority or estimated cost,
    /// such as for maintenance planning or optimization.
    /// </remarks>
    /// <param name="roadId">The road identifier.</param>
    /// <returns>The maintenance record if found; otherwise 404 Not Found.</returns>
    [HttpGet("{roadId:long}/maintenance")]
    public async Task<IActionResult> GetMaintenance(long roadId)
    {
        RoadMaintenance? maintenance = await roadService.GetMaintenanceByRoadIdAsync(roadId);
        return maintenance is null ? NotFound() : Ok(maintenance);
    }
}

