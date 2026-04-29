using System.Diagnostics;
using CairoTransportation.Services.Algorithms.Common.DTOs;

namespace CairoTransportation.Services.Algorithms.Common.Instrumentation;

public sealed class AlgorithmExecutionMetrics
{
    private readonly Stopwatch _stopwatch = Stopwatch.StartNew();
    private readonly HashSet<string> _discoveredNodes = [];

    public int ExpandedNodes { get; private set; }

    public void MarkDiscovered(string nodeId) => _discoveredNodes.Add(nodeId);

    public void MarkExpanded() => ExpandedNodes++;

    public AlgorithmTraceDto Complete()
    {
        _stopwatch.Stop();
        return new AlgorithmTraceDto
        {
            VisitedNodes = _discoveredNodes.Count,
            ExpandedNodes = ExpandedNodes,
            ExecutionTimeMs = _stopwatch.Elapsed.TotalMilliseconds
        };
    }
}

