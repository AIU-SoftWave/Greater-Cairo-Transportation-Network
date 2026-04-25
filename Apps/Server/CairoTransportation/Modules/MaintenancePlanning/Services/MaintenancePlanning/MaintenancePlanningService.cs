using CairoTransportation.Data;
using CairoTransportation.Models;
using CairoTransportation.Services.Algorithms.Common.DTOs;
using CairoTransportation.Services.Algorithms.Common.Instrumentation;
using CairoTransportation.Services.Algorithms.MaintenancePlanning.Contracts;
using CairoTransportation.Services.Algorithms.MaintenancePlanning.DTOs;
using Microsoft.EntityFrameworkCore;

namespace CairoTransportation.Services.Algorithms.MaintenancePlanning;

/// <summary>
/// Service for generating optimal maintenance plans using 0/1 Knapsack Dynamic Programming.
/// Selects roads to maximize total priority score within a given budget.
/// </summary>
public class MaintenancePlanningService(TransportationDbContext dbContext) : IMaintenancePlanningService
{
    public async Task<AlgorithmResponseDto<MaintenancePlanningResultDto>> GenerateMaintenancePlanAsync(double budget)
    {
        AlgorithmExecutionMetrics metrics = new();

        if (budget <= 0)
        {
            return CreateFailureResponse("Budget must be greater than zero.", metrics, budget);
        }

        // Load candidates with their computed value scores
        List<Candidate> candidates = await LoadCandidatesAsync();
        metrics.MarkExpanded();

        if (candidates.Count == 0)
        {
            return CreateEmptyResponse(budget, metrics);
        }

        // Cap budget at total cost of all candidates (no need for larger DP table)
        int totalCost = candidates.Sum(c => c.Cost);
        int effectiveBudget = (int)Math.Min(budget, totalCost * 1.1);

        int n = candidates.Count;
        int B = effectiveBudget;

        // 0/1 Knapsack DP: dp[i, b] = max value using first i items with budget b
        int[,] dp = new int[n + 1, B + 1];

        // Build DP table
        for (int i = 1; i <= n; i++)
        {
            Candidate item = candidates[i - 1];
            for (int b = 0; b <= B; b++)
            {
                // Don't take item i
                dp[i, b] = dp[i - 1, b];

                // Take item i if it fits and improves value
                if (item.Cost <= b)
                {
                    int valueWithItem = dp[i - 1, b - item.Cost] + item.Value;
                    if (valueWithItem > dp[i, b])
                    {
                        dp[i, b] = valueWithItem;
                    }
                }
            }
            metrics.MarkDiscovered(item.RoadId.ToString());
        }

        metrics.MarkExpanded();

        // Backtrack to find selected items
        HashSet<long> selectedIds = [];
        int remaining = B;

        for (int i = n; i > 0 && remaining > 0; i--)
        {
            // If value changed, item i was selected
            if (dp[i, remaining] != dp[i - 1, remaining])
            {
                Candidate item = candidates[i - 1];
                selectedIds.Add(item.RoadId);
                remaining -= item.Cost;
            }
        }

        metrics.MarkExpanded();

        // Build result
        MaintenancePlanningResultDto result = BuildResult(candidates, selectedIds, budget);
        string message = result.SelectedRoadCount > 0
            ? $"Maintenance plan generated: {result.SelectedRoadCount} roads selected with total priority score {result.TotalPriorityScore}."
            : "No roads could be selected within the given budget.";

        return CreateSuccessResponse(result, message, metrics);
    }

    /// <summary>
    /// Internal model for DP computation.
    /// </summary>
    private record Candidate(long RoadId, string? From, string? To, double? Condition, int Cost, int Priority, int Value);

    private async Task<List<Candidate>> LoadCandidatesAsync()
    {
        IQueryable<Candidate> query =
            from m in dbContext.RoadMaintenances.AsNoTracking()
            join r in dbContext.Roads.AsNoTracking() on m.RoadId equals r.Id
            join f in dbContext.Locations.AsNoTracking() on r.FromLocationId equals f.Id
            join t in dbContext.Locations.AsNoTracking() on r.ToLocationId equals t.Id
            where m.EstimatedCost > 0 && m.Priority > 0
            let value = (int)((m.Priority ?? 1) * (1 + (100 - (r.Condition ?? 50)) / 100.0))
            select new Candidate(
                r.Id,
                f.Name,
                t.Name,
                r.Condition,
                (int)m.EstimatedCost!,
                m.Priority ?? 1,
                value
            );

        return await query.ToListAsync();
    }

    private static MaintenancePlanningResultDto BuildResult(
        List<Candidate> candidates,
        HashSet<long> selectedIds,
        double budget)
    {
        double totalCost = 0;
        int totalPriority = 0;
        double totalImprovement = 0;
        List<MaintenanceRoadDto> selected = [];
        List<MaintenanceRoadDto> notSelected = [];

        foreach (Candidate c in candidates)
        {
            bool isSelected = selectedIds.Contains(c.RoadId);
            double currentCond = c.Condition ?? 50;
            double newCond = Math.Min(100, currentCond + (100 - currentCond) * 0.5);

            MaintenanceRoadDto dto = new()
            {
                RoadId = c.RoadId,
                FromLocation = c.From,
                ToLocation = c.To,
                CurrentCondition = currentCond,
                EstimatedCost = c.Cost,
                Priority = c.Priority,
                ExpectedNewCondition = newCond,
                Reason = isSelected
                    ? "Selected by 0/1 Knapsack optimization"
                    : c.Cost > budget ? "Cost exceeds total budget" : "Lower priority/cost ratio"
            };

            if (isSelected)
            {
                totalCost += c.Cost;
                totalPriority += c.Priority;
                totalImprovement += newCond - currentCond;
                selected.Add(dto);
            }
            else
            {
                notSelected.Add(dto);
            }
        }

        return new MaintenancePlanningResultDto
        {
            Budget = budget,
            TotalCost = totalCost,
            RemainingBudget = budget - totalCost,
            TotalPriorityScore = totalPriority,
            SelectedRoadCount = selected.Count,
            TotalCandidateRoads = candidates.Count,
            ExpectedConditionImprovement = totalImprovement,
            SelectedRoads = selected.OrderByDescending(x => x.Priority).ToList(),
            NotSelectedRoads = notSelected.OrderByDescending(x => x.Priority).ToList()
        };
    }

    private static AlgorithmResponseDto<MaintenancePlanningResultDto> CreateEmptyResponse(
        double budget, AlgorithmExecutionMetrics metrics) =>
        CreateSuccessResponse(
            new MaintenancePlanningResultDto
            {
                Budget = budget,
                TotalCost = 0,
                RemainingBudget = budget,
                TotalPriorityScore = 0,
                SelectedRoadCount = 0,
                TotalCandidateRoads = 0,
                ExpectedConditionImprovement = 0,
                SelectedRoads = new List<MaintenanceRoadDto>(),
                NotSelectedRoads = new List<MaintenanceRoadDto>()
            },
            "No maintenance candidate roads found in database.",
            metrics);

    private static AlgorithmResponseDto<MaintenancePlanningResultDto> CreateSuccessResponse(
        MaintenancePlanningResultDto result, string message, AlgorithmExecutionMetrics metrics) =>
        new()
        {
            AlgorithmName = "Maintenance Planning (0/1 Knapsack DP)",
            Success = true,
            Message = message,
            Trace = metrics.Complete(),
            Data = result
        };

    private static AlgorithmResponseDto<MaintenancePlanningResultDto> CreateFailureResponse(
        string message, AlgorithmExecutionMetrics metrics, double budget) =>
        new()
        {
            AlgorithmName = "Maintenance Planning (0/1 Knapsack DP)",
            Success = false,
            Message = message,
            Trace = metrics.Complete(),
            Data = new MaintenancePlanningResultDto { Budget = budget }
        };
}
