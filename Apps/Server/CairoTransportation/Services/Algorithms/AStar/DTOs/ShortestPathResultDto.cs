namespace CairoTransportation.Services.Algorithms.AStar.DTOs;

public class ShortestPathResultDto
{
    public string FromNodeId { get; set; } = string.Empty;
    public string ToNodeId { get; set; } = string.Empty;
    public bool Found { get; set; }
    public double TotalDistance { get; set; }

    /// <summary>
    /// Number of unique nodes discovered by the algorithm (including the start node).
    /// A node is counted when its best-known cost is first assigned or improved.
    /// </summary>
    public int VisitedNodes { get; set; }

    /// <summary>
    /// Number of unique nodes expanded by the algorithm.
    /// A node is expanded when dequeued from the priority queue and processed.
    /// </summary>
    public int ExpandedNodes { get; set; }

    /// <summary>
    /// End-to-end execution time for one shortest-path request in milliseconds.
    /// Includes graph fetch, search, and result mapping.
    /// </summary>
    public long ExecutionTimeMs { get; set; }

    public string? Message { get; set; }
    public List<ShortestPathNodeDto> PathNodes { get; set; } = [];
    public List<ShortestPathRoadDto> PathRoads { get; set; } = [];
}
