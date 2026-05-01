using CairoTransportation.Data;
using CairoTransportation.Modules.MaintenancePlanning.Services.Contracts;
using CairoTransportation.Utils.Algorithms.DynamicProgramming.Contracts;
using CairoTransportation.Utils.Helpers.Common.DTOs;
using CairoTransportation.Utils.Helpers.Common.Instrumentation;
using Microsoft.EntityFrameworkCore;

namespace CairoTransportation.Modules.MaintenancePlanning.Services;

public class MaintenancePlanningService(
    TransportationDbContext dbContext,
    IKnapsackMaintenanceOptimizer optimizer) : IMaintenancePlanningService
{
    public async Task<AlgorithmResponseDto<MaintenancePlanningResultDto>> GenerateMaintenancePlanAsync(double budget)
    {
        var metrics = new AlgorithmExecutionMetrics();
        if (budget <= 0)
        {
            return new AlgorithmResponseDto<MaintenancePlanningResultDto> { Success = false, Message = "Budget must be > 0." };
        }

        // 1. Fetch roads in need of maintenance from the database
        List<MaintenanceCandidate> candidates = await (from m in dbContext.RoadMaintenances.AsNoTracking()
                                                       join r in dbContext.Roads.AsNoTracking() on m.RoadId equals r.Id
                                                       join f in dbContext.Locations.AsNoTracking() on r.FromLocationId equals f.Id
                                                       join t in dbContext.Locations.AsNoTracking() on r.ToLocationId equals t.Id
                                                       where m.EstimatedCost > 0 && m.Priority > 0
                                                       // Value = Priority * Condition Modifier
                                                       let value = (int)((m.Priority ?? 1) * (1 + (100 - (r.Condition ?? 50)) / 100.0))
                                                       select new MaintenanceCandidate(r.Id, f.Name, t.Name, r.Condition, (int)m.EstimatedCost!, m.Priority ?? 1, value))
                               .ToListAsync();

        // 2. Run Knapsack optimization to maximize priority score within budget
        MaintenancePlanningResultDto data = optimizer.GenerateMaintenancePlan(candidates, budget);

        return new AlgorithmResponseDto<MaintenancePlanningResultDto>
        {
            AlgorithmName = "Knapsack Maintenance Optimizer",
            Success = true,
            Message = data.SelectedRoadCount > 0 ? "Maintenance plan generated." : "No roads selected.",
            Trace = metrics.Complete(),
            Data = data
        };
    }
}
