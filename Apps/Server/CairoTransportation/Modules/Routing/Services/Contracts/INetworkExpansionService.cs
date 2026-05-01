using CairoTransportation.Utils.Helpers.Common.DTOs;

namespace CairoTransportation.Modules.Routing.Services.Contracts;

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
