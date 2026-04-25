using CairoTransportation.Services.Algorithms.Common.DTOs;

namespace CairoTransportation.Services.Algorithms.TimeVaryingDijkstra.Contracts;

public interface ITimeVaryingDijkstraService
{
    Task<AlgorithmResponseDto<ShortestPathResultDto>> FindShortestPathAsync(string fromNodeId, string toNodeId, string period);
}
