namespace CairoTransportation.Services.Algorithms.TrafficSignal.DTOs;

/// <summary>
/// Result DTO for traffic signal optimization.
/// </summary>
public class TrafficSignalResultDto
{
    /// <summary>Time period analyzed.</summary>
    public string Period { get; set; } = string.Empty;

    /// <summary>Number of roads analyzed.</summary>
    public int RoadsAnalyzed { get; set; }

    /// <summary>Number of intersections analyzed in the selected period.</summary>
    public int IntersectionsAnalyzed { get; set; }

    /// <summary>Number of intersections that received at least one recommendation.</summary>
    public int IntersectionsWithSignalRecommendations { get; set; }

    /// <summary>Number of signal recommendations generated.</summary>
    public int SignalRecommendations { get; set; }

    /// <summary>Total congestion score across analyzed roads.</summary>
    public double TotalCongestionScore { get; set; }

    /// <summary>Estimated wait time reduction with optimized signals.</summary>
    public double EstimatedWaitTimeReductionPercent { get; set; }

    /// <summary>Individual signal timing recommendations.</summary>
    public List<SignalTimingDto> SignalTimings { get; set; } = [];
}

/// <summary>
/// DTO representing signal timing for a specific road/direction.
/// </summary>
public class SignalTimingDto
{
    /// <summary>Road identifier.</summary>
    public long RoadId { get; set; }

    /// <summary>From location name.</summary>
    public string? FromLocation { get; set; }

    /// <summary>To location name.</summary>
    public string? ToLocation { get; set; }

    /// <summary>Current traffic flow (vehicles per period).</summary>
    public int CurrentFlow { get; set; }

    /// <summary>Road capacity.</summary>
    public int RoadCapacity { get; set; }

    /// <summary>Congestion ratio (flow / capacity).</summary>
    public double CongestionRatio { get; set; }

    /// <summary>Priority rank (1 = highest congestion).</summary>
    public int PriorityRank { get; set; }

    /// <summary>Recommended green light duration in seconds.</summary>
    public int RecommendedGreenDurationSeconds { get; set; }

    /// <summary>Recommended cycle time in seconds.</summary>
    public int RecommendedCycleTimeSeconds { get; set; }

    /// <summary>Reason for this signal timing recommendation.</summary>
    public string Reason { get; set; } = string.Empty;
}
