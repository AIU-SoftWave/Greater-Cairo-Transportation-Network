namespace CairoTransportation.Services.Algorithms.Common.DTOs;

public class AlgorithmTraceDto
{
    public int VisitedNodes { get; set; }
    public int ExpandedNodes { get; set; }
    public long ExecutionTimeMs { get; set; }
}

