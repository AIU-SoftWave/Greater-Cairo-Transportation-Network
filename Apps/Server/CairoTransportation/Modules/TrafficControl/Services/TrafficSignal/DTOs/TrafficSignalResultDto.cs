namespace CairoTransportation.Services.Algorithms.TrafficSignal.DTOs;

/// <summary>
/// Result DTO for traffic signal optimization.
/// </summary>
public class TrafficSignalResultDto
{
    /// <summary>Time period analyzed.</summary>
    public string Period { get; set; } = string.Empty;

    /// <summary>Summary statistics.</summary>
    public SignalSummary Summary { get; set; } = new();

    /// <summary>Intersection-level signal plans.</summary>
    public List<IntersectionSignalPlan> Intersections { get; set; } = [];
}

/// <summary>
/// Summary statistics for signal optimization.
/// </summary>
public class SignalSummary
{
    /// <summary>Number of roads analyzed.</summary>
    public int RoadsAnalyzed { get; set; }

    /// <summary>Number of intersections analyzed in the selected period.</summary>
    public int IntersectionsAnalyzed { get; set; }

    /// <summary>Number of intersections that received signal optimizations.</summary>
    public int OptimizedIntersections { get; set; }

    /// <summary>Estimated wait time reduction with optimized signals.</summary>
    public double EstimatedWaitTimeReductionPercent { get; set; }
}

/// <summary>
/// Signal plan for a single intersection.
/// </summary>
public class IntersectionSignalPlan
{
    /// <summary>Intersection name.</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>Cycle time for this intersection (seconds).</summary>
    public int CycleTimeSeconds { get; set; }

    /// <summary>Road phases at this intersection.</summary>
    public List<SignalPhaseDto> Roads { get; set; } = [];
}

/// <summary>
/// Signal phase for a road at an intersection.
/// </summary>
public class SignalPhaseDto
{
    /// <summary>From location name.</summary>
    public string From { get; set; } = string.Empty;

    /// <summary>To location name (intersection).</summary>
    public string To { get; set; } = string.Empty;

    /// <summary>Congestion percentage (0-100+).</summary>
    public double CongestionPercent { get; set; }

    /// <summary>Priority rank (1 = highest congestion).</summary>
    public int Priority { get; set; }

    /// <summary>Green light duration for this phase (seconds).</summary>
    public int GreenTimeSeconds { get; set; }
}

