using CairoTransportation.Models;
using CairoTransportation.Services;
using Microsoft.AspNetCore.Mvc;

namespace CairoTransportation.Controllers;

[ApiController]
[Route("api/roads")]
public class RoadsController(IRoadService roadService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await roadService.GetAllAsync());

    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetById(long id)
    {
        Road? road = await roadService.GetByIdAsync(id);
        return road is null ? NotFound() : Ok(road);
    }

    [HttpGet("from/{locationId}")]
    public async Task<IActionResult> GetByFromLocation(string locationId) => Ok(await roadService.GetByFromLocationIdAsync(locationId));

    [HttpGet("{roadId:long}/maintenance")]
    public async Task<IActionResult> GetMaintenance(long roadId)
    {
        RoadMaintenance? maintenance = await roadService.GetMaintenanceByRoadIdAsync(roadId);
        return maintenance is null ? NotFound() : Ok(maintenance);
    }
}
