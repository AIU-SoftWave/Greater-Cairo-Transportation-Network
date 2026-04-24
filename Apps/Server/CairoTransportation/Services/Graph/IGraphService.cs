namespace CairoTransportation.Services.Graph;

/// <summary>
/// Shared graph service that provides graph data structures for algorithms.
/// This interface will be extended incrementally as new algorithms are implemented.
/// </summary>
public interface IGraphService
{
    /// <summary>
    /// Gets the complete transportation graph with all nodes and edges.
    /// Only includes existing roads (IsExisting = true).
    /// </summary>
    /// <returns>Complete graph ready for algorithm processing.</returns>
    Task<Graph> GetGraphAsync();
}
