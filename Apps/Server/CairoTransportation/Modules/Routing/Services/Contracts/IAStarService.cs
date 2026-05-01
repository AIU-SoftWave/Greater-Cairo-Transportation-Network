using CairoTransportation.Utils.Helpers.Common.DTOs;

namespace CairoTransportation.Modules.Routing.Services.Contracts;

/// <summary>
/// Business service for emergency routing using A* search.
/// Coordinates data fetching, caching, and execution metrics for emergency vehicle dispatch.
/// </summary>
public interface IAStarService
{
    /// <summary>
    /// Finds the optimal path between two nodes using coordinate-guided A* search.
    /// </summary>
    Task<AlgorithmResponseDto<ShortestPathResultDto>> FindShortestPathAsync(string fromNodeId, string toNodeId);

    /// <summary>
    /// Finds the nearest medical facility or critical infrastructure from an origin node.
    /// Especially useful for emergency responders.
    /// </summary>
    Task<AlgorithmResponseDto<ShortestPathResultDto>> FindNearestMedicalFacilityAsync(string fromNodeId);
}
