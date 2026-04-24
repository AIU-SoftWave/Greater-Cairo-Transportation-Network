using CairoTransportation.Services.Algorithms.AStar.DTOs;

namespace CairoTransportation.Services.Algorithms.AStar.Contracts;

public interface IAStarService
{
    Task<ShortestPathResultDto> FindShortestPathAsync(string fromNodeId, string toNodeId);
}
