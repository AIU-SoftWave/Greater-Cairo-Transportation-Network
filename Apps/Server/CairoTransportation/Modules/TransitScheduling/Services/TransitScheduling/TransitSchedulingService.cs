using CairoTransportation.Data;
using CairoTransportation.Services.Algorithms.Common.DTOs;
using CairoTransportation.Services.Algorithms.Common.Instrumentation;
using CairoTransportation.Services.Algorithms.TransitScheduling.Contracts;
using CairoTransportation.Services.Algorithms.TransitScheduling.DTOs;
using Microsoft.EntityFrameworkCore;

namespace CairoTransportation.Services.Algorithms.TransitScheduling;

public class TransitSchedulingService(TransportationDbContext dbContext)
    : ITransitSchedulingService
{
    private record RouteData(
        string RouteId,
        string RouteType,
        int DailyPassengers,
        int CapacityPerRoute,
        int StopCount,
        int ValuePerVehicle);

    public async Task<AlgorithmResponseDto<TransitSchedulingResultDto>> GenerateScheduleAsync(int totalVehicles)
    {
        var metrics = new AlgorithmExecutionMetrics();

        if (totalVehicles <= 0)
        {
            return CreateFailureResponse("Total vehicles must be greater than zero.", metrics, totalVehicles);
        }

        List<RouteData> routes = await LoadRoutesAsync();
        metrics.MarkExpanded();

        if (routes.Count == 0)
        {
            return CreateEmptyResponse(totalVehicles, metrics);
        }

        int n = routes.Count;
        int V = totalVehicles;

        int[,] dp = new int[n + 1, V + 1];
        int[,] choice = new int[n + 1, V + 1];

        // ================= DP =================
        for (int i = 1; i <= n; i++)
        {
            RouteData route = routes[i - 1];

            for (int v = 0; v <= V; v++)
            {
                dp[i, v] = dp[i - 1, v];
                choice[i, v] = 0;

                int max = Math.Min(route.CapacityPerRoute, v);

                for (int k = 1; k <= max; k++)
                {
                    int value = k * route.ValuePerVehicle;
                    int candidate = dp[i - 1, v - k] + value;

                    if (candidate > dp[i, v])
                    {
                        dp[i, v] = candidate;
                        choice[i, v] = k;
                    }
                }
            }
        }

        // ================= BACKTRACK =================
        var allocation = new Dictionary<string, int>();
        int remaining = V;

        for (int i = n; i > 0; i--)
        {
            int assigned = choice[i, remaining];
            if (assigned > 0)
            {
                allocation[routes[i - 1].RouteId] = assigned;
                remaining -= assigned;
            }
        }

        TransitSchedulingResultDto result = BuildResult(routes, allocation, totalVehicles, dp[n, V]);

        return CreateSuccessResponse(
            result,
            $"Optimized {result.ActiveRoutes} routes using {result.AssignedVehicles} vehicles.",
            metrics);
    }

    // ================= DATA =================
    private async Task<List<RouteData>> LoadRoutesAsync()
        => await dbContext.TransportRoutes
            .AsNoTracking()
            .Select(r => new RouteData(
                r.Id,
                r.Type,
                r.DailyPassengers ?? 0,

                // IMPORTANT:
                // VehiclesAssigned = MAX CAPACITY (your decision)
                r.VehiclesAssigned ?? 20,

                r.RouteStops.Count,

                // SIMPLE VALUE MODEL (no logs, no tricks)
                (r.DailyPassengers ?? 0) / Math.Max(r.VehiclesAssigned ?? 20, 1)
            ))
            .ToListAsync();

    // ================= RESULT =================
    private static TransitSchedulingResultDto BuildResult(
        List<RouteData> routes,
        Dictionary<string, int> allocation,
        int totalVehicles,
        int totalValue)
    {
        int used = allocation.Values.Sum();

        return new TransitSchedulingResultDto
        {
            TotalVehicles = totalVehicles,
            AssignedVehicles = used,
            RemainingVehicles = totalVehicles - used,
            TotalDemand = routes.Sum(r => r.DailyPassengers),

            EstimatedPassengersServed = totalValue,
            CoverageRatio = routes.Sum(r => r.DailyPassengers) > 0
                ? (double)totalValue / routes.Sum(r => r.DailyPassengers)
                : 0,

            TotalRoutes = routes.Count,
            ActiveRoutes = allocation.Count,

            RouteAllocations = routes.Select(r =>
            {
                allocation.TryGetValue(r.RouteId, out int v);

                int served = Math.Min(r.DailyPassengers, v * r.ValuePerVehicle);

                return new RouteAllocationDto
                {
                    RouteId = r.RouteId,
                    RouteType = r.RouteType,
                    AssignedVehicles = v,
                    DailyPassengers = r.DailyPassengers,
                    StopCount = r.StopCount,
                    EstimatedServed = served,
                    EfficiencyScore = v > 0 ? served / (double)v : 0,
                    Reason = v > 0
                        ? $"Allocated {v} vehicles within capacity {r.CapacityPerRoute}"
                        : "Not selected"
                };
            }).ToList()
        };
    }

    // ================= RESPONSES =================
    private static AlgorithmResponseDto<TransitSchedulingResultDto> CreateSuccessResponse(
        TransitSchedulingResultDto result,
        string message,
        AlgorithmExecutionMetrics metrics)
        => new()
        {
            AlgorithmName = "Transit Scheduling (Simple DP)",
            Success = true,
            Message = message,
            Trace = metrics.Complete(),
            Data = result
        };

    private static AlgorithmResponseDto<TransitSchedulingResultDto> CreateEmptyResponse(
        int totalVehicles,
        AlgorithmExecutionMetrics metrics)
        => CreateSuccessResponse(
            new TransitSchedulingResultDto
            {
                TotalVehicles = totalVehicles,
                AssignedVehicles = 0,
                RemainingVehicles = totalVehicles
            },
            "No routes found.",
            metrics);

    private static AlgorithmResponseDto<TransitSchedulingResultDto> CreateFailureResponse(
        string message,
        AlgorithmExecutionMetrics metrics,
        int totalVehicles)
        => new()
        {
            AlgorithmName = "Transit Scheduling (Simple DP)",
            Success = false,
            Message = message,
            Trace = metrics.Complete(),
            Data = new TransitSchedulingResultDto
            {
                TotalVehicles = totalVehicles
            }
        };
}
