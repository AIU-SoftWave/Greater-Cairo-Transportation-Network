using CairoTransportation.Services.Algorithms.Common.DTOs;

namespace CairoTransportation.Services;

public enum SimulationWeather
{
    Clear = 0,
    Rain = 1,
    Storm = 2
}

public interface ISimulationService
{
    // Road Closures
    Task ToggleRoadClosureAsync(long roadId);
    Task ResetClosuresAsync();
    Task<HashSet<long>> GetClosedRoadIdsAsync();
    int GetStateVersion();
    
    // Weather
    Task SetWeatherAsync(SimulationWeather weather);
    SimulationWeather GetWeather();

    // Emergency Preemption
    Task SetEmergencyPreemptionAsync(long roadId, bool active);
    Task<bool> IsPreemptedAsync(long roadId);

    // Performance Metrics
    void RecordMetrics(string algorithmName, long executionTimeMs, int visitedNodes, int expandedNodes);
    List<AlgorithmPerformanceMetric> GetPerformanceMetrics();
}

public record AlgorithmPerformanceMetric(
    string AlgorithmName,
    long ExecutionTimeMs,
    int VisitedNodes,
    int ExpandedNodes,
    DateTime Timestamp
);
