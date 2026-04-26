using CairoTransportation.Algorithms.Greedy.Contracts;
using CairoTransportation.Services.Algorithms.TrafficSignal.DTOs;

namespace CairoTransportation.Algorithms.Greedy;

public class GreedySignalOptimizer : IGreedySignalOptimizer
{
    public TrafficSignalResultDto OptimizeSignals(List<SignalRoadCongestion> roads, string period, int topN, bool allIntersections)
    {
        // 1. Filter for roads that actually need attention (congested or emergency)
        var congested = roads.Where(r => r.CongestionRatio > 0.5 || r.IsEmergencyRoute).ToList();
        if (congested.Count == 0)
        {
            return new TrafficSignalResultDto { Period = period, Summary = new SignalSummary { RoadsAnalyzed = roads.Count } };
        }

        // 2. Greedy Priority: Sort by Emergency status first, then by Congestion level
        List<SignalRoadCongestion> prioritized = allIntersections
            ? congested.OrderByDescending(r => r.IsEmergencyRoute).ThenByDescending(r => r.CongestionRatio).ToList()
            : congested.OrderByDescending(r => r.IsEmergencyRoute).ThenByDescending(r => r.CongestionRatio).Take(Math.Clamp(topN, 1, 50)).ToList();

        // 3. Calculation: Group roads by their intersection and assign green times
        var plans = prioritized.GroupBy(r => r.IntersectionLocationId).Select(g =>
        {
            var sortedInIntersection = g.OrderByDescending(r => r.IsEmergencyRoute).ThenByDescending(r => r.CongestionRatio).ToList();
            double totalLoad = sortedInIntersection.Sum(r => r.CongestionRatio);
            
            // Adjust intersection cycle time (60-120s) based on total traffic load
            int cycle = Math.Clamp(60 + (int)(totalLoad * 10), 60, 120);

            var phases = sortedInIntersection.Select((r, i) => new SignalPhaseDto
            {
                From = r.FromLocation ?? "Unknown",
                To = r.ToLocation ?? "Unknown",
                CongestionPercent = r.CongestionRatio * 100,
                Priority = i + 1,
                // Strategy: 
                // Emergency routes get a guaranteed 40% of the cycle.
                // Others get time proportional to their traffic flow.
                GreenTimeSeconds = r.IsEmergencyRoute 
                    ? Math.Max(20, (int)(cycle * 0.4)) 
                    : Math.Clamp((int)(cycle * (r.CongestionRatio / Math.Max(totalLoad, 0.1))), 10, cycle / 2)
            }).ToList();

            return new IntersectionSignalPlan { Name = g.Key, CycleTimeSeconds = cycle, Roads = phases };
        }).ToList();

        return new TrafficSignalResultDto 
        { 
            Period = period, 
            Summary = new SignalSummary 
            { 
                RoadsAnalyzed = roads.Count, 
                OptimizedIntersections = plans.Count, 
                EstimatedWaitTimeReductionPercent = plans.Count > 0 ? 10 : 0 
            }, 
            Intersections = plans 
        };
    }
}
