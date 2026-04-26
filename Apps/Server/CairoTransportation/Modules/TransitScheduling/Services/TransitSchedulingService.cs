using CairoTransportation.Algorithms.DynamicProgramming.Contracts;
using CairoTransportation.Data;
using CairoTransportation.Services.Algorithms.Common.DTOs;
using CairoTransportation.Services.Algorithms.Common.Instrumentation;
using CairoTransportation.Services.Algorithms.TransitScheduling.DTOs;
using CairoTransportation.Services.TransitScheduling.Contracts;
using Microsoft.EntityFrameworkCore;

namespace CairoTransportation.Services.TransitScheduling;

public class TransitSchedulingService(
    TransportationDbContext dbContext, 
    IResourceAllocationScheduler scheduler) : ITransitSchedulingService
{
    public async Task<AlgorithmResponseDto<TransitSchedulingResultDto>> GenerateScheduleAsync(int totalVehicles)
    {
        var metrics = new AlgorithmExecutionMetrics();
        if (totalVehicles <= 0)
        {
            return new AlgorithmResponseDto<TransitSchedulingResultDto> { Success = false, Message = "Vehicles must be > 0." };
        }

        // 1. Fetch current transit route performance data
        List<TransitRouteData> routes = await dbContext.TransportRoutes.AsNoTracking().Select(r => new TransitRouteData(
            r.Id, r.Type, r.DailyPassengers ?? 0, r.VehiclesAssigned ?? 20, r.RouteStops.Count, 
            (r.DailyPassengers ?? 0) / Math.Max(r.VehiclesAssigned ?? 20, 1))).ToListAsync();

        // 2. Run Resource Allocation algorithm to optimize vehicle assignment
        TransitSchedulingResultDto data = scheduler.GenerateSchedule(routes, totalVehicles);

        return new AlgorithmResponseDto<TransitSchedulingResultDto>
        {
            AlgorithmName = "Resource Allocation Scheduler (DP)",
            Success = true,
            Message = $"Optimized {data.ActiveRoutes} routes.",
            Trace = metrics.Complete(),
            Data = data
        };
    }
}
