using CairoTransportation.Algorithms.DynamicProgramming.Contracts;
using CairoTransportation.Services.Algorithms.TransitScheduling.DTOs;

namespace CairoTransportation.Algorithms.DynamicProgramming;

public class ResourceAllocationScheduler : IResourceAllocationScheduler
{
    public TransitSchedulingResultDto GenerateSchedule(List<TransitRouteData> routes, int totalVehicles)
    {
        if (routes.Count == 0 || totalVehicles <= 0)
        {
            return new TransitSchedulingResultDto { TotalVehicles = totalVehicles };
        }

        int n = routes.Count; int V = totalVehicles;
        // dp[i, v] = max passengers served by first 'i' routes using 'v' vehicles
        int[,] dp = new int[n + 1, V + 1];
        // choice[i, v] = number of vehicles assigned to route 'i' to get the max value
        int[,] choice = new int[n + 1, V + 1];

        // 1. Build the DP table
        for (int i = 1; i <= n; i++)
        {
            TransitRouteData r = routes[i - 1];
            for (int v = 0; v <= V; v++)
            {
                dp[i, v] = dp[i - 1, v]; // Default: assign 0 vehicles to this route
                
                int maxPossibleVehicles = Math.Min(r.CapacityPerRoute, v);
                for (int k = 1; k <= maxPossibleVehicles; k++)
                {
                    int val = dp[i - 1, v - k] + k * r.ValuePerVehicle;
                    if (val > dp[i, v])
                    {
                        dp[i, v] = val;
                        choice[i, v] = k;
                    }
                }
            }
        }

        // 2. Backtrack to find the specific allocation per route
        var allocation = new Dictionary<string, int>();
        int rem = V;
        for (int i = n; i > 0; i--)
        {
            int assigned = choice[i, rem];
            if (assigned > 0)
            {
                allocation[routes[i - 1].RouteId] = assigned;
                rem -= assigned;
            }
        }

        return BuildResult(routes, allocation, totalVehicles, dp[n, V]);
    }

    private static TransitSchedulingResultDto BuildResult(List<TransitRouteData> routes, Dictionary<string, int> alloc, int total, int val)
    {
        int used = alloc.Values.Sum();
        return new TransitSchedulingResultDto
        {
            TotalVehicles = total,
            AssignedVehicles = used,
            RemainingVehicles = total - used,
            TotalDemand = routes.Sum(r => r.DailyPassengers),
            EstimatedPassengersServed = val,
            CoverageRatio = routes.Sum(r => r.DailyPassengers) > 0 ? (double)val / routes.Sum(r => r.DailyPassengers) : 0,
            TotalRoutes = routes.Count,
            ActiveRoutes = alloc.Count,
            RouteAllocations = routes.Select(r =>
            {
                alloc.TryGetValue(r.RouteId, out int v);
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
                    Reason = v > 0 ? $"Allocated {v} vehicles" : "Not selected"
                };
            }).ToList()
        };
    }
}
