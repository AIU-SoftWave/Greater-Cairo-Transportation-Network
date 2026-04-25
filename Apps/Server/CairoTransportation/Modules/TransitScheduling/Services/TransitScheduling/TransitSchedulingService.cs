using CairoTransportation.Data;
using CairoTransportation.Models;
using CairoTransportation.Services.Algorithms.Common.DTOs;
using CairoTransportation.Services.Algorithms.Common.Instrumentation;
using CairoTransportation.Services.Algorithms.TransitScheduling.Contracts;
using CairoTransportation.Services.Algorithms.TransitScheduling.DTOs;
using Microsoft.EntityFrameworkCore;

namespace CairoTransportation.Services.Algorithms.TransitScheduling;

/// <summary>
/// Service for optimizing transit vehicle allocation using Dynamic Programming.
/// 
/// This is a Resource Allocation problem: distribute limited vehicles across routes
/// to maximize total passenger demand coverage.
/// </summary>
public class TransitSchedulingService(TransportationDbContext dbContext) : ITransitSchedulingService
{
    /// <summary>
    /// Internal model for DP computation.
    /// </summary>
    private record RouteData(
        string RouteId,
        string RouteType,
        int? DailyPassengers,
        int? CurrentVehicles,
        int StopCount,
        int ValuePerVehicle);

    public async Task<AlgorithmResponseDto<TransitSchedulingResultDto>> GenerateScheduleAsync(int totalVehicles)
    {
        AlgorithmExecutionMetrics metrics = new();

        if (totalVehicles <= 0)
        {
            return CreateFailureResponse("Total vehicles must be greater than zero.", metrics, totalVehicles);
        }

        // Load route data with stop counts
        List<RouteData> routes = await LoadRoutesAsync();
        metrics.MarkExpanded();

        if (routes.Count == 0)
        {
            return CreateEmptyResponse(totalVehicles, metrics);
        }

        // Cap vehicles at reasonable max (sum of max vehicles per route)
        int maxVehiclesPerRoute = 20; // Assume max 20 vehicles per route
        int effectiveFleet = Math.Min(totalVehicles, routes.Count * maxVehiclesPerRoute);

        int n = routes.Count;
        int V = effectiveFleet;

        // DP: dp[i, v] = max demand covered using first i routes with v vehicles
        int[,] dp = new int[n + 1, V + 1];
        // choice[i, v] = vehicles assigned to route i in optimal solution
        int[,] choice = new int[n + 1, V + 1];

        // Build DP table
        for (int i = 1; i <= n; i++)
        {
            RouteData route = routes[i - 1];
            int maxForThisRoute = Math.Min(maxVehiclesPerRoute, V);

            for (int v = 0; v <= V; v++)
            {
                // Default: don't assign any vehicles to this route
                dp[i, v] = dp[i - 1, v];
                choice[i, v] = 0;

                // Try assigning k vehicles to this route
                for (int k = 1; k <= maxForThisRoute && k <= v; k++)
                {
                    int valueWithK = dp[i - 1, v - k] + k * route.ValuePerVehicle;
                    if (valueWithK > dp[i, v])
                    {
                        dp[i, v] = valueWithK;
                        choice[i, v] = k;
                    }
                }
            }

            metrics.MarkDiscovered(route.RouteId);
        }

        metrics.MarkExpanded();

        // Backtrack to find allocation
        Dictionary<string, int> allocation = [];
        int remainingVehicles = V;

        for (int i = n; i > 0 && remainingVehicles > 0; i--)
        {
            int assigned = choice[i, remainingVehicles];
            if (assigned > 0)
            {
                RouteData route = routes[i - 1];
                allocation[route.RouteId] = assigned;
                remainingVehicles -= assigned;
            }
        }

        metrics.MarkExpanded();

        // Build result
        TransitSchedulingResultDto result = BuildResult(routes, allocation, totalVehicles, dp[n, V]);
        string message = result.ActiveRoutes > 0
            ? $"Transit schedule optimized: {result.ActiveRoutes} routes active with {result.AssignedVehicles} vehicles, serving ~{result.EstimatedPassengersServed} passengers."
            : "No vehicles could be allocated to routes.";

        return CreateSuccessResponse(result, message, metrics);
    }

    private async Task<List<RouteData>> LoadRoutesAsync()
    {
        // Load routes with their stop counts
        List<RouteData> routes = await dbContext.TransportRoutes
            .AsNoTracking()
            .Select(r => new RouteData(
                r.Id,
                r.Type,
                r.DailyPassengers,
                r.VehiclesAssigned,
                r.RouteStops.Count,
                // Value per vehicle: passengers per vehicle (estimated)
                r.DailyPassengers.HasValue && r.VehiclesAssigned.HasValue && r.VehiclesAssigned.Value > 0
                    ? r.DailyPassengers.Value / r.VehiclesAssigned.Value
                    : r.DailyPassengers ?? 100 // Default if no data
            ))
            .ToListAsync();

        return routes;
    }

    private static TransitSchedulingResultDto BuildResult(
        List<RouteData> routes,
        Dictionary<string, int> allocation,
        int totalVehicles,
        int totalValue)
    {
        int assignedVehicles = 0;
        int totalDemand = routes.Sum(r => r.DailyPassengers ?? 0);
        List<RouteAllocationDto> allocations = [];

        foreach (RouteData route in routes)
        {
            bool isAllocated = allocation.TryGetValue(route.RouteId, out int vehicles);
            assignedVehicles += vehicles;

            int estimatedServed = isAllocated
                ? Math.Min(route.DailyPassengers ?? 0, vehicles * route.ValuePerVehicle)
                : 0;

            double efficiency = vehicles > 0 ? (double)estimatedServed / vehicles : 0;

            // Estimate frequency: assume 16 hours operation, round-trip time ~2 hours
            int? frequency = vehicles > 0 ? 120 / vehicles : null; // minutes between buses

            allocations.Add(new RouteAllocationDto
            {
                RouteId = route.RouteId,
                RouteType = route.RouteType,
                AssignedVehicles = vehicles,
                CurrentVehicles = route.CurrentVehicles,
                DailyPassengers = route.DailyPassengers,
                StopCount = route.StopCount,
                EstimatedFrequencyMinutes = frequency,
                EstimatedServed = estimatedServed,
                EfficiencyScore = efficiency,
                Reason = isAllocated
                    ? $"Allocated {vehicles} vehicles based on demand {route.DailyPassengers} passengers"
                    : "No vehicles allocated - lower priority than other routes"
            });
        }

        return new TransitSchedulingResultDto
        {
            TotalVehicles = totalVehicles,
            AssignedVehicles = assignedVehicles,
            RemainingVehicles = totalVehicles - assignedVehicles,
            TotalDemand = totalDemand,
            EstimatedPassengersServed = totalValue,
            CoverageRatio = totalDemand > 0 ? (double)totalValue / totalDemand : 0,
            TotalRoutes = routes.Count,
            ActiveRoutes = allocation.Count,
            RouteAllocations = allocations.OrderByDescending(x => x.EstimatedServed).ToList()
        };
    }

    private static AlgorithmResponseDto<TransitSchedulingResultDto> CreateEmptyResponse(
        int totalVehicles, AlgorithmExecutionMetrics metrics) =>
        CreateSuccessResponse(
            new TransitSchedulingResultDto
            {
                TotalVehicles = totalVehicles,
                AssignedVehicles = 0,
                RemainingVehicles = totalVehicles,
                TotalDemand = 0,
                EstimatedPassengersServed = 0,
                CoverageRatio = 0,
                TotalRoutes = 0,
                ActiveRoutes = 0,
                RouteAllocations = []
            },
            "No transit routes found in database.",
            metrics);

    private static AlgorithmResponseDto<TransitSchedulingResultDto> CreateSuccessResponse(
        TransitSchedulingResultDto result, string message, AlgorithmExecutionMetrics metrics) =>
        new()
        {
            AlgorithmName = "Transit Scheduling (Resource Allocation DP)",
            Success = true,
            Message = message,
            Trace = metrics.Complete(),
            Data = result
        };

    private static AlgorithmResponseDto<TransitSchedulingResultDto> CreateFailureResponse(
        string message, AlgorithmExecutionMetrics metrics, int totalVehicles) =>
        new()
        {
            AlgorithmName = "Transit Scheduling (Resource Allocation DP)",
            Success = false,
            Message = message,
            Trace = metrics.Complete(),
            Data = new TransitSchedulingResultDto { TotalVehicles = totalVehicles }
        };
}

