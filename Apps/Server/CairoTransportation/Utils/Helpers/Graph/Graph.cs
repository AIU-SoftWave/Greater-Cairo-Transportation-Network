namespace CairoTransportation.Services.Graph;

/// <summary>
/// Represents a complete transportation network graph.
/// Provided by IGraphService; consumed by algorithm implementations.
/// </summary>
public class Graph
{
    /// <summary>
    /// All nodes (locations) in the graph.
    /// </summary>
    public List<GraphNode> Nodes { get; set; } = [];

    /// <summary>
    /// All edges (roads) in the graph.
    /// </summary>
    public List<GraphEdge> Edges { get; set; } = [];

    /// <summary>
    /// Adjacency information for each node.
    /// Key: node ID, Value: list of edge IDs connected to that node.
    /// Used for efficient traversal by algorithms.
    /// </summary>
    public Dictionary<string, List<long>> AdjacencyList { get; set; } = [];

    /// <summary>
    /// Node lookup by ID for O(1) access during algorithm execution.
    /// </summary>
    public Dictionary<string, GraphNode> NodeIndex { get; set; } = [];

    /// <summary>
    /// Edge lookup by ID for O(1) access during algorithm execution.
    /// </summary>
    public Dictionary<long, GraphEdge> EdgeIndex { get; set; } = [];

    /// <summary>
    /// Time period for which traffic data was included (e.g., "morning", "evening").
    /// Null if no traffic filtering was applied.
    /// </summary>
    public string? TrafficPeriod { get; set; }

    /// <summary>
    /// Total number of nodes in the graph.
    /// </summary>
    public int NodeCount => Nodes.Count;

    /// <summary>
    /// Total number of edges in the graph.
    /// </summary>
    public int EdgeCount => Edges.Count;
}
