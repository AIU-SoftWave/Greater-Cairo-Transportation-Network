using CairoTransportation.Services.Algorithms.MaintenancePlanning.DTOs;

namespace CairoTransportation.Algorithms.DynamicProgramming.Contracts;

public interface IKnapsackMaintenanceOptimizer
{
    /// <summary>
    /// Executes 0/1 Knapsack optimization to select road maintenance tasks.
    /// </summary>
    /// <param name="candidates">List of maintenance candidates with cost and priority value.</param>
    /// <param name="budget">Total available budget.</param>
    MaintenancePlanningResultDto GenerateMaintenancePlan(List<MaintenanceCandidate> candidates, double budget);
}

public record MaintenanceCandidate(long RoadId, string? From, string? To, double? Condition, int Cost, int Priority, int Value);
