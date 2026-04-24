using CairoTransportation.Data;
using CairoTransportation.Models;
using CairoTransportation.Services.Algorithms.Common.DTOs;
using CairoTransportation.Services.Algorithms.Common.Instrumentation;
using CairoTransportation.Services.Algorithms.MaintenancePlanning.Contracts;
using CairoTransportation.Services.Algorithms.MaintenancePlanning.DTOs;
using Microsoft.EntityFrameworkCore;

namespace CairoTransportation.Services.Algorithms.MaintenancePlanning;

public class MaintenancePlanningService(TransportationDbContext dbContext) : IMaintenancePlanningService
{
    private sealed record MaintenanceCandidate(
        long RoadId,
        string? FromLocation,
        string? ToLocation,
        double? CurrentCondition,
        double? EstimatedCost,
        int? Priority,
        double ValueScore);  // Computed priority score for DP

    public async Task<AlgorithmResponseDto<MaintenancePlanningResultDto>> GenerateMaintenancePlanAsync(double budget)
    {
        AlgorithmExecutionMetrics metrics = new();

        if (budget <= 0)
        {
            return CreateFailureResponse("Budget must be greater than zero.", metrics, budget);
        }

        // Load all candidate roads for maintenance (have maintenance records with cost and priority)
        List<MaintenanceCandidate> candidates = await LoadMaintenanceCandidatesAsync();
        metrics.MarkExpanded();

        if (candidates.Count == 0)
        {
            return CreateSuccessResponse(
                new MaintenancePlanningResultDto
                {
                    Budget = budget,
                    TotalCost = 0,
                    RemainingBudget = budget,
                    TotalPriorityScore = 0,
                    SelectedRoadCount = 0,
                    TotalCandidateRoads = 0,
                    ExpectedConditionImprovement = 0,
                    SelectedRoads = [],
                    NotSelectedRoads = []
                },
                "No maintenance candidate roads found in database.",
                metrics);
        }

        // Convert budget to integer cents to avoid floating point issues in DP
        // Max budget handling: cap at reasonable amount to prevent excessive memory
        int maxBudgetCents = (int)Math.Min(budget * 100, 10_000_000);  // Max 100k budget in cents

        // 0/1 Knapsack DP: dp[b] = max value achievable with budget b
        // Using 1D array optimization
        int[] dp = new int[maxBudgetCents + 1];
        int[] selected = new int[maxBudgetCents + 1];  // Tracks which item was last selected

        for (int i = 0; i < candidates.Count; i++)
        {
            MaintenanceCandidate candidate = candidates[i];
            if (!candidate.EstimatedCost.HasValue || candidate.EstimatedCost.Value <= 0)
            {
                continue;
            }

            int costCents = (int)(candidate.EstimatedCost.Value * 100);
            int value = candidate.ValueScore > 0 ? (int)candidate.ValueScore : 1;

            // Standard 0/1 knapsack: iterate backwards to avoid reusing same item
            for (int b = maxBudgetCents; b >= costCents; b--)
            {
                if (dp[b - costCents] + value > dp[b])
                {
                    dp[b] = dp[b - costCents] + value;
                    selected[b] = i + 1;  // Store 1-based index
                }
            }

            metrics.MarkDiscovered(candidate.RoadId.ToString());
        }

        metrics.MarkExpanded();

        // Backtrack to find selected items
        HashSet<int> selectedIndices = [];
        int remainingBudget = maxBudgetCents;
        while (remainingBudget > 0 && selected[remainingBudget] > 0)
        {
            int itemIndex = selected[remainingBudget] - 1;  // Convert back to 0-based
            if (selectedIndices.Contains(itemIndex))
            {
                break;  // Safety: prevent infinite loop
            }

            selectedIndices.Add(itemIndex);
            MaintenanceCandidate candidate = candidates[itemIndex];
            int costCents = (int)(candidate.EstimatedCost!.Value * 100);
            remainingBudget -= costCents;
        }

        metrics.MarkExpanded();

        // Build result
        double totalCost = 0;
        int totalPriority = 0;
        double totalConditionImprovement = 0;
        List<MaintenanceRoadDto> selectedRoads = [];
        List<MaintenanceRoadDto> notSelectedRoads = [];

        for (int i = 0; i < candidates.Count; i++)
        {
            MaintenanceCandidate c = candidates[i];
            bool isSelected = selectedIndices.Contains(i);

            // Estimate new condition: assume maintenance brings condition to 100
            // or improves by 30% of gap, whichever is reasonable
            double currentCond = c.CurrentCondition ?? 50;
            double newCondition = Math.Min(100, currentCond + (100 - currentCond) * 0.5);

            if (isSelected)
            {
                totalCost += c.EstimatedCost!.Value;
                totalPriority += c.Priority ?? 0;
                totalConditionImprovement += newCondition - currentCond;

                selectedRoads.Add(new MaintenanceRoadDto
                {
                    RoadId = c.RoadId,
                    FromLocation = c.FromLocation,
                    ToLocation = c.ToLocation,
                    CurrentCondition = currentCond,
                    EstimatedCost = c.EstimatedCost,
                    Priority = c.Priority,
                    ExpectedNewCondition = newCondition,
                    Reason = "Selected by 0/1 Knapsack optimization for max priority within budget"
                });
            }
            else
            {
                notSelectedRoads.Add(new MaintenanceRoadDto
                {
                    RoadId = c.RoadId,
                    FromLocation = c.FromLocation,
                    ToLocation = c.ToLocation,
                    CurrentCondition = currentCond,
                    EstimatedCost = c.EstimatedCost,
                    Priority = c.Priority,
                    ExpectedNewCondition = newCondition,
                    Reason = c.EstimatedCost.HasValue && c.EstimatedCost.Value > budget
                        ? "Cost exceeds total budget"
                        : "Not selected - lower priority/cost ratio than alternatives"
                });
            }
        }

        MaintenancePlanningResultDto result = new()
        {
            Budget = budget,
            TotalCost = totalCost,
            RemainingBudget = budget - totalCost,
            TotalPriorityScore = totalPriority,
            SelectedRoadCount = selectedRoads.Count,
            TotalCandidateRoads = candidates.Count,
            ExpectedConditionImprovement = totalConditionImprovement,
            SelectedRoads = selectedRoads.OrderByDescending(x => x.Priority).ToList(),
            NotSelectedRoads = notSelectedRoads.OrderByDescending(x => x.Priority).ToList()
        };

        string message = selectedRoads.Count > 0
            ? $"Maintenance plan generated: {selectedRoads.Count} roads selected with total priority score {totalPriority} within budget {budget:C}."
            : "No roads could be selected within the given budget.";

        return CreateSuccessResponse(result, message, metrics);
    }

    private async Task<List<MaintenanceCandidate>> LoadMaintenanceCandidatesAsync()
    {
        IQueryable<MaintenanceCandidate> query = from maintenance in dbContext.RoadMaintenances.AsNoTracking()
                                                 join road in dbContext.Roads.AsNoTracking() on maintenance.RoadId equals road.Id
                                                 join fromLoc in dbContext.Locations.AsNoTracking() on road.FromLocationId equals fromLoc.Id
                                                 join toLoc in dbContext.Locations.AsNoTracking() on road.ToLocationId equals toLoc.Id
                                                 where maintenance.EstimatedCost.HasValue && maintenance.EstimatedCost.Value > 0
                                                       && maintenance.Priority.HasValue && maintenance.Priority.Value > 0
                                                 select new MaintenanceCandidate(
                                                     road.Id,
                                                     fromLoc.Name,
                                                     toLoc.Name,
                                                     road.Condition,
                                                     maintenance.EstimatedCost,
                                                     maintenance.Priority,
                                                     // Value score: priority * (1 + urgency from condition)
                                                     // Lower condition = higher urgency multiplier
                                                     (maintenance.Priority ?? 1) * (1 + (100 - (road.Condition ?? 50)) / 100.0)
                                                 );

        return await query.ToListAsync();
    }

    private static AlgorithmResponseDto<MaintenancePlanningResultDto> CreateSuccessResponse(
        MaintenancePlanningResultDto result,
        string message,
        AlgorithmExecutionMetrics metrics) =>
        new()
        {
            AlgorithmName = "Maintenance Planning (0/1 Knapsack DP)",
            Success = true,
            Message = message,
            Trace = metrics.Complete(),
            Data = result
        };

    private static AlgorithmResponseDto<MaintenancePlanningResultDto> CreateFailureResponse(
        string message,
        AlgorithmExecutionMetrics metrics,
        double budget) =>
        new()
        {
            AlgorithmName = "Maintenance Planning (0/1 Knapsack DP)",
            Success = false,
            Message = message,
            Trace = metrics.Complete(),
            Data = new MaintenancePlanningResultDto
            {
                Budget = budget,
                TotalCost = 0,
                RemainingBudget = 0,
                TotalPriorityScore = 0,
                SelectedRoadCount = 0,
                TotalCandidateRoads = 0,
                ExpectedConditionImprovement = 0
            }
        };
}
