using CairoTransportation.Services.Algorithms.Common.DTOs;
using CairoTransportation.Services.Algorithms.TransitScheduling.Contracts;
using CairoTransportation.Services.Algorithms.TransitScheduling.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace CairoTransportation.Controllers;

/// <summary>
/// Provides transit vehicle scheduling optimization using Dynamic Programming (Resource Allocation).
/// </summary>
[ApiController]
[Route("api/transit-operations")]
public class TransitSchedulingController(ITransitSchedulingService schedulingService) : ControllerBase
{
    /// <summary>
    /// Generates an optimal vehicle allocation schedule across transit routes.
    /// </summary>
    /// <remarks>
    /// Uses Resource Allocation DP to distribute vehicles across routes
    /// to maximize total passenger demand coverage.
    /// </remarks>
    /// <param name="vehicles">Total number of vehicles in the fleet (e.g., 50).</param>
    /// <returns>Optimized schedule with route allocations and frequency estimates.</returns>
    [HttpGet]
    public async Task<IActionResult> GetSchedule([FromQuery] int vehicles)
    {
        // Validate input
        if (vehicles <= 0)
        {
            return BadRequest(new AlgorithmResponseDto<TransitSchedulingResultDto>
            {
                AlgorithmName = "Transit Scheduling",
                Success = false,
                Message = "Vehicle count must be greater than zero.",
                Data = new TransitSchedulingResultDto
                {
                    TotalVehicles = vehicles,
                    AssignedVehicles = 0,
                    RemainingVehicles = 0,
                    TotalDemand = 0,
                    EstimatedPassengersServed = 0,
                    CoverageRatio = 0,
                    TotalRoutes = 0,
                    ActiveRoutes = 0,
                    RouteAllocations = []
                }
            });
        }

        // Cap at reasonable maximum
        const int maxVehicles = 1000;
        if (vehicles > maxVehicles)
        {
            return BadRequest(new AlgorithmResponseDto<TransitSchedulingResultDto>
            {
                AlgorithmName = "Transit Scheduling",
                Success = false,
                Message = $"Vehicle count exceeds maximum allowed value of {maxVehicles}.",
                Data = new TransitSchedulingResultDto
                {
                    TotalVehicles = vehicles,
                    AssignedVehicles = 0,
                    RemainingVehicles = 0,
                    TotalDemand = 0,
                    EstimatedPassengersServed = 0,
                    CoverageRatio = 0,
                    TotalRoutes = 0,
                    ActiveRoutes = 0,
                    RouteAllocations = []
                }
            });
        }

        AlgorithmResponseDto<TransitSchedulingResultDto> result = await schedulingService.GenerateScheduleAsync(vehicles);
        return result.Success ? Ok(result) : BadRequest(result);
    }
}

