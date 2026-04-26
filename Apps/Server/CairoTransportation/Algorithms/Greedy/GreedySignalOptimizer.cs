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

        // 2. Prioritize: Emergency routes first, then by congestion ratio
        List<SignalRoadCongestion> prioritized = allIntersections
            ? congested.OrderByDescending(r => r.IsEmergencyRoute).ThenByDescending(r => r.CongestionRatio).ToList()
            : congested.OrderByDescending(r => r.IsEmergencyRoute).ThenByDescending(r => r.CongestionRatio).Take(Math.Clamp(topN, 1, 50)).ToList();

        // 3. Group by intersection and calculate green times
        var plans = prioritized.GroupBy(r => r.IntersectionLocationId).Select(g =>
        {
            var sorted = g.OrderByDescending(r => r.IsEmergencyRoute).ThenByDescending(r => r.CongestionRatio).ToList();
            double total = sorted.Sum(r => r.CongestionRatio);
            
            // Adjust cycle time (60-120s) based on total intersection load
            int cycle = Math.Clamp(60 + (int)(total * 10), 60, 120);

            var phases = sorted.Select((r, i) => new SignalPhaseDto
            {
                From = r.FromLocation ?? "Unknown",
                To = r.ToLocation ?? "Unknown",
                CongestionPercent = r.CongestionRatio * 100,
                Priority = i + 1,
                // Strategy: Emergency routes get 40% minimum; others get time proportional to congestion
                GreenTimeSeconds = r.IsEmergencyRoute ? Math.Max(20, (int)(cycle * 0.4)) : Math.Clamp((int)(cycle * (r.CongestionRatio / Math.Max(total, 0.1))), 10, cycle / 2)
            }).ToList();

            return new IntersectionSignalPlan { Name = g.Key, CycleTimeSeconds = cycle, Roads = phases };
        }).ToList();

        return new TrafficSignalResultDto { Period = period, Summary = new SignalSummary { RoadsAnalyzed = roads.Count, OptimizedIntersections = plans.Count, EstimatedWaitTimeReductionPercent = plans.Count > 0 ? 10 : 0 }, Intersections = plans };
    }
}
