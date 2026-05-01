using CairoTransportation.Modules.TrafficControl.Services.TrafficSignal.DTOs;

namespace CairoTransportation.Utils.Algorithms.Greedy.Contracts;

public interface IGreedySignalOptimizer
{
    /// <summary>
    /// Executes greedy optimization for traffic signal timing.
    /// </summary>
    /// <param name="roads">List of roads at intersections with congestion data.</param>
    /// <param name="period">The traffic period.</param>
    /// <param name="topN">Number of intersections to prioritize.</param>
    /// <param name="allIntersections">Whether to analyze all intersections.</param>
    TrafficSignalResultDto OptimizeSignals(List<SignalRoadCongestion> roads, string period, int topN, bool allIntersections);
}

public record SignalRoadCongestion(long RoadId, string? FromLocation, string? ToLocation, string IntersectionLocationId, int Flow, int Capacity, double CongestionRatio, bool IsEmergencyRoute);
