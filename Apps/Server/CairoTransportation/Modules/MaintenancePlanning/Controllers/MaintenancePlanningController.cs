using CairoTransportation.Modules.MaintenancePlanning.Services.Contracts;
using CairoTransportation.Utils.Helpers.Common.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace CairoTransportation.Modules.MaintenancePlanning.Controllers;

/// <summary>
/// Provides maintenance planning optimization using Dynamic Programming (0/1 Knapsack).
/// </summary>
[ApiController]
[Route("api/maintenance-strategy")]
public class MaintenancePlanningController(IMaintenancePlanningService maintenanceService) : ControllerBase
{
    /// <summary>
    /// Generates an optimal maintenance plan within the given budget.
    /// </summary>
    /// <remarks>
    /// Uses 0/1 Knapsack Dynamic Programming to maximize total priority score
    /// while staying within budget. Lower condition roads get higher urgency multipliers.
    /// </remarks>
    /// <param name="budget">Total maintenance budget (e.g., 10000000 for 10 million).</param>
    /// <returns>Optimized maintenance plan with selected roads and cost breakdown.</returns>
    [HttpGet]
    public async Task<IActionResult> GetMaintenancePlan([FromQuery] double budget)
    {
        // Validate budget is a valid number
        if (double.IsNaN(budget) || double.IsInfinity(budget))
        {
            return BadRequest(new AlgorithmResponseDto<MaintenancePlanningResultDto>
            {
                AlgorithmName = "Maintenance Planning",
                Success = false,
                Message = "Budget must be a valid finite number.",
                Data = new MaintenancePlanningResultDto
                {
                    Budget = 0,
                    TotalCost = 0,
                    RemainingBudget = 0,
                    TotalPriorityScore = 0,
                    SelectedRoadCount = 0,
                    TotalCandidateRoads = 0,
                    ExpectedConditionImprovement = 0,
                    SelectedRoads = [],
                    NotSelectedRoads = []
                }
            });
        }

        // Validate budget is positive
        if (budget <= 0)
        {
            return BadRequest(new AlgorithmResponseDto<MaintenancePlanningResultDto>
            {
                AlgorithmName = "Maintenance Planning",
                Success = false,
                Message = "Budget must be greater than zero.",
                Data = new MaintenancePlanningResultDto
                {
                    Budget = budget,
                    TotalCost = 0,
                    RemainingBudget = 0,
                    TotalPriorityScore = 0,
                    SelectedRoadCount = 0,
                    TotalCandidateRoads = 0,
                    ExpectedConditionImprovement = 0,
                    SelectedRoads = [],
                    NotSelectedRoads = []
                }
            });
        }

        // Validate budget doesn't exceed maximum (prevent excessive memory usage)
        const double maxBudget = 1_000_000_000_000;  // 1 trillion
        if (budget > maxBudget)
        {
            return BadRequest(new AlgorithmResponseDto<MaintenancePlanningResultDto>
            {
                AlgorithmName = "Maintenance Planning",
                Success = false,
                Message = $"Budget exceeds maximum allowed value of {maxBudget:C}. Please use a smaller budget.",
                Data = new MaintenancePlanningResultDto
                {
                    Budget = budget,
                    TotalCost = 0,
                    RemainingBudget = 0,
                    TotalPriorityScore = 0,
                    SelectedRoadCount = 0,
                    TotalCandidateRoads = 0,
                    ExpectedConditionImprovement = 0,
                    SelectedRoads = [],
                    NotSelectedRoads = []
                }
            });
        }

        AlgorithmResponseDto<MaintenancePlanningResultDto> result = await maintenanceService.GenerateMaintenancePlanAsync(budget);
        return result.Success ? Ok(result) : BadRequest(result);
    }
}

