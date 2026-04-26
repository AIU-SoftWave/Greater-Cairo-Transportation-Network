using CairoTransportation.Services.Algorithms.Common.DTOs;
using CairoTransportation.Services.Algorithms.MaintenancePlanning.DTOs;

namespace CairoTransportation.Services.MaintenancePlanning.Contracts;

/// <summary>
/// Business service for road maintenance strategy.
/// Uses optimization algorithms to decide which roads to fix first given a limited budget.
/// </summary>
public interface IMaintenancePlanningService
{
    /// <summary>
    /// Generates an optimal maintenance schedule using the 0/1 Knapsack algorithm.
    /// Maximizes the "Priority Score" of repaired roads within the budget.
    /// </summary>
    /// <param name="budget">The total financial budget available for repairs.</param>
    Task<AlgorithmResponseDto<MaintenancePlanningResultDto>> GenerateMaintenancePlanAsync(double budget);
}
