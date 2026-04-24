using CairoTransportation.Services.Algorithms.Common.DTOs;

namespace CairoTransportation.Services.Algorithms.Dijkstra.Contracts;

public interface IDijkstraService
{
    Task<AlgorithmResponseDto<ShortestPathResultDto>> FindShortestPathAsync(string fromNodeId, string toNodeId);
}
