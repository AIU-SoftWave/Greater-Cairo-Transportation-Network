using System.Collections.Concurrent;

namespace CairoTransportation.Services;

public class SimulationService : ISimulationService
{
    private readonly HashSet<long> _closedRoadIds = new();
    private readonly ConcurrentDictionary<long, bool> _preemptedRoads = new();
    private readonly ConcurrentQueue<AlgorithmPerformanceMetric> _metrics = new();
    private readonly object _lock = new();
    private int _version = 0;
    private SimulationWeather _weather = SimulationWeather.Clear;

    public Task ToggleRoadClosureAsync(long roadId)
    {
        lock (_lock)
        {
            if (_closedRoadIds.Contains(roadId))
                _closedRoadIds.Remove(roadId);
            else
                _closedRoadIds.Add(roadId);
            
            _version++;
        }
        return Task.CompletedTask;
    }

    public Task ResetClosuresAsync()
    {
        lock (_lock)
        {
            _closedRoadIds.Clear();
            _preemptedRoads.Clear();
            _version++;
        }
        return Task.CompletedTask;
    }

    public int GetStateVersion() => _version;

    public Task SetWeatherAsync(SimulationWeather weather)
    {
        lock (_lock)
        {
            _weather = weather;
            _version++;
        }
        return Task.CompletedTask;
    }

    public SimulationWeather GetWeather() => _weather;

    public Task<HashSet<long>> GetClosedRoadIdsAsync()
    {
        lock (_lock)
        {
            return Task.FromResult(new HashSet<long>(_closedRoadIds));
        }
    }

    public Task SetEmergencyPreemptionAsync(long roadId, bool active)
    {
        _preemptedRoads[roadId] = active;
        return Task.CompletedTask;
    }

    public Task<bool> IsPreemptedAsync(long roadId) => Task.FromResult(_preemptedRoads.TryGetValue(roadId, out bool active) && active);

    public void RecordMetrics(string algorithmName, long executionTimeMs, int visitedNodes, int expandedNodes)
    {
        _metrics.Enqueue(new AlgorithmPerformanceMetric(
            algorithmName,
            executionTimeMs,
            visitedNodes,
            expandedNodes,
            DateTime.UtcNow
        ));

        // Keep only last 100 metrics
        while (_metrics.Count > 100)
        {
            _metrics.TryDequeue(out _);
        }
    }

    public List<AlgorithmPerformanceMetric> GetPerformanceMetrics() => _metrics.ToList();
}
