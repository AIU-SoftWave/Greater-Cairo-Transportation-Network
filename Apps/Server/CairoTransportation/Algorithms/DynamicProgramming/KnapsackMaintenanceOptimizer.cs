using CairoTransportation.Algorithms.DynamicProgramming.Contracts;
using CairoTransportation.Services.Algorithms.MaintenancePlanning.DTOs;

namespace CairoTransportation.Algorithms.DynamicProgramming;

public class KnapsackMaintenanceOptimizer : IKnapsackMaintenanceOptimizer
{
    public MaintenancePlanningResultDto GenerateMaintenancePlan(List<MaintenanceCandidate> candidates, double budget)
    {
        if (candidates.Count == 0 || budget <= 0)
        {
            return new MaintenancePlanningResultDto { Budget = budget };
        }

        // Use a capped budget to prevent OOM on massive virtual budgets
        int totalCost = candidates.Sum(c => c.Cost);
        int B = (int)Math.Min(budget, totalCost * 1.1);
        int n = candidates.Count;

        // dp[i, b] = max priority value using first 'i' roads with budget 'b'
        int[,] dp = new int[n + 1, B + 1];

        // 1. Fill DP Table
        for (int i = 1; i <= n; i++)
        {
            MaintenanceCandidate item = candidates[i - 1];
            for (int b = 0; b <= B; b++)
            {
                dp[i, b] = dp[i - 1, b]; // Skip road
                if (item.Cost <= b)
                {
                    int val = dp[i - 1, b - item.Cost] + item.Value;
                    if (val > dp[i, b])
                    {
                        dp[i, b] = val;
                    }

                }
            }
        }

        // 2. Backtrack to find selected road IDs
        HashSet<long> selectedIds = [];
        int remaining = B;
        for (int i = n; i > 0 && remaining > 0; i--)
        {
            if (dp[i, remaining] != dp[i - 1, remaining])
            {
                selectedIds.Add(candidates[i - 1].RoadId);
                remaining -= candidates[i - 1].Cost;
            }
        }

        return BuildResult(candidates, selectedIds, budget);
    }

    private static MaintenancePlanningResultDto BuildResult(List<MaintenanceCandidate> candidates, HashSet<long> selectedIds, double budget)
    {
        double cost = 0; int priority = 0; double improvement = 0;
        List<MaintenanceRoadDto> selected = []; List<MaintenanceRoadDto> notSelected = [];

        foreach (MaintenanceCandidate c in candidates)
        {
            bool isSel = selectedIds.Contains(c.RoadId);
            double curr = c.Condition ?? 50;
            double next = Math.Min(100, curr + (100 - curr) * 0.5); // Heuristic improvement
            
            var dto = new MaintenanceRoadDto 
            { 
                RoadId = c.RoadId, 
                FromLocation = c.From, 
                ToLocation = c.To, 
                CurrentCondition = curr, 
                EstimatedCost = c.Cost, 
                Priority = c.Priority, 
                ExpectedNewCondition = next, 
                Reason = isSel ? "Optimized via 0/1 Knapsack" : "Not optimal for budget" 
            };

            if (isSel) 
            { 
                cost += c.Cost; 
                priority += c.Priority; 
                improvement += next - curr; 
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
            TotalCost = cost, 
            RemainingBudget = budget - cost, 
            TotalPriorityScore = priority, 
            SelectedRoadCount = selected.Count, 
            TotalCandidateRoads = candidates.Count, 
            ExpectedConditionImprovement = improvement, 
            SelectedRoads = selected.OrderByDescending(x => x.Priority).ToList(), 
            NotSelectedRoads = notSelected.OrderByDescending(x => x.Priority).ToList() 
        };
    }
}
