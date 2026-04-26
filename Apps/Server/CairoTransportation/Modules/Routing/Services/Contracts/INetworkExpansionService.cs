using CairoTransportation.Services.Algorithms.Common.DTOs;
using CairoTransportation.Services.Algorithms.Mst.DTOs;

namespace CairoTransportation.Services.Routing.Contracts;

/// <summary>
/// Business service for network expansion planning.
/// Designs the most cost-effective way to connect all areas of the city.
/// </summary>
public interface INetworkExpansionService
{
    /// <summary>
    /// Builds a Minimum Spanning Tree (MST) of the network to find the cheapest 
    /// set of roads that connects all nodes, prioritizing high-population areas.
    /// </summary>
    Task<AlgorithmResponseDto<MstResultDto>> BuildCheapestNetworkAsync();
}
