using CairoTransportation.Services.Algorithms.Common.DTOs;

namespace CairoTransportation.Services.Algorithms.AStar.Contracts;

public interface IAStarService
{
    Task<AlgorithmResponseDto<ShortestPathResultDto>> FindShortestPathAsync(string fromNodeId, string toNodeId);
}

