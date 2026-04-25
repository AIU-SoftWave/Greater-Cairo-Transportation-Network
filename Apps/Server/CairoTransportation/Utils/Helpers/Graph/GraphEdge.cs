namespace CairoTransportation.Services.Graph;

/// <summary>
/// Represents an edge in the transportation graph (road).
/// Used by algorithms to traverse and analyze the network.
/// </summary>
public class GraphEdge
{
    /// <summary>
    /// Unique identifier for the road/edge.
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// Source node identifier.
    /// </summary>
    public string FromNodeId { get; set; } = string.Empty;

    /// <summary>
    /// Target node identifier.
    /// </summary>
    public string ToNodeId { get; set; } = string.Empty;

    /// <summary>
    /// Distance in km (primary weight for most algorithms).
    /// </summary>
    public double Distance { get; set; }

    /// <summary>
    /// Vehicle capacity constraint.
    /// </summary>
    public int Capacity { get; set; }

    /// <summary>
    /// Road condition (1-5 scale, affects travel time in time-dependent routing).
    /// 1 = excellent, 5 = poor.
    /// </summary>
    public int? Condition { get; set; }

    /// <summary>
    /// Whether the road currently exists (0) or is under construction (1).
    /// Used to filter available edges in pathfinding.
    /// </summary>
    public bool IsExisting { get; set; }

    /// <summary>
    /// Construction cost if road is planned/under construction.
    /// Used by MST and network expansion algorithms.
    /// </summary>
    public double? ConstructionCost { get; set; }

    /// <summary>
    /// Current traffic flow on this edge (if queried for a specific period).
    /// Used by time-dependent and congestion-aware algorithms.
    /// </summary>
    public int? CurrentTraffic { get; set; }

    /// <summary>
    /// Maintenance priority for this edge.
    /// Used by greedy and dynamic programming algorithms.
    /// Lower value = higher priority.
    /// </summary>
    public int? MaintenancePriority { get; set; }

    /// <summary>
    /// Estimated maintenance cost.
    /// Used by budget-constrained optimization.
    /// </summary>
    public double? MaintenanceCost { get; set; }
}
