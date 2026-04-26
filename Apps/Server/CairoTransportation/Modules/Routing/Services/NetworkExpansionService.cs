using CairoTransportation.Algorithms.NetworkExpansion.Contracts;
using CairoTransportation.Services.Algorithms.Common.DTOs;
using CairoTransportation.Services.Algorithms.Common.Instrumentation;
using CairoTransportation.Services.Algorithms.Mst.DTOs;
using CairoTransportation.Services.Graph;
using CairoTransportation.Services.Routing.Contracts;

namespace CairoTransportation.Services.Routing;

public class NetworkExpansionService(
    IGraphService graphService, 
    IPrimNetworkExpander planner) : INetworkExpansionService
{
    public async Task<AlgorithmResponseDto<MstResultDto>> BuildCheapestNetworkAsync()
    {
        var metrics = new AlgorithmExecutionMetrics();
        
        // 1. Load full graph (including potential new roads)
        Graph.Graph graph = await graphService.GetGraphAsync(includePotentialRoads: true);

        // 2. Execute Prim's algorithm for cost-effective connectivity
        MstResultDto data = planner.BuildCheapestNetwork(graph);

        return new AlgorithmResponseDto<MstResultDto>
        {
            AlgorithmName = "Prim's MST",
            Success = data.Connected,
            Message = data.Connected ? "Cheapest network built." : "Disconnected graph.",
            Trace = metrics.Complete(),
            Data = data
        };
    }
}
