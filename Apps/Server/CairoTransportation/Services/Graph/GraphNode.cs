namespace CairoTransportation.Services.Graph;

/// <summary>
/// Represents a node in the transportation graph (location).
/// Used by algorithms to build graph data structures.
/// </summary>
public class GraphNode
{
    /// <summary>
    /// Unique identifier for the location (node).
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Location name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Location type (e.g., "Station", "Hub", "Terminal").
    /// </summary>
    public string Type { get; set; } = string.Empty;

    /// <summary>
    /// X coordinate for spatial queries and visualization.
    /// </summary>
    public double? X { get; set; }

    /// <summary>
    /// Y coordinate for spatial queries and visualization.
    /// </summary>
    public double? Y { get; set; }

    /// <summary>
    /// Demand/importance indicator for algorithms.
    /// </summary>
    public int? Population { get; set; }

    /// <summary>
    /// Whether this location is critical for network operations.
    /// Used by optimization algorithms to prioritize maintenance/routing.
    /// </summary>
    public bool IsCritical { get; set; }
}
