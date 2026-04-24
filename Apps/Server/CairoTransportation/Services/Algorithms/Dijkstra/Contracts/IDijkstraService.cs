using CairoTransportation.Services.Algorithms.Dijkstra.DTOs;

namespace CairoTransportation.Services.Algorithms.Dijkstra.Contracts;

public interface IDijkstraService
{
    Task<ShortestPathResultDto> FindShortestPathAsync(string fromNodeId, string toNodeId);
}
