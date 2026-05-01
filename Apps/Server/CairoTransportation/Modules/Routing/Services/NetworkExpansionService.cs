using CairoTransportation.Modules.Routing.Services.Contracts;
using CairoTransportation.Modules.Simulation.Services;
using CairoTransportation.Utils.Algorithms.NetworkExpansion.Contracts;
using CairoTransportation.Utils.Helpers.Common.DTOs;
using CairoTransportation.Utils.Helpers.Common.Instrumentation;
using CairoTransportation.Utils.Helpers.Graph;

namespace CairoTransportation.Modules.Routing.Services;

public class NetworkExpansionService(
    IGraphService graphService,
    IPrimNetworkExpander planner,
    ISimulationService simulationService,
    AlgorithmExecutionMetrics metrics) : INetworkExpansionService
{
    public async Task<AlgorithmResponseDto<MstResultDto>> BuildCheapestNetworkAsync()
    {
        // 1. Load full graph (including potential new roads)
        Graph graph = await graphService.GetGraphAsync(includePotentialRoads: true);

        // 2. Execute Prim's algorithm for cost-effective connectivity
        MstResultDto data = planner.BuildCheapestNetwork(graph);

        AlgorithmTraceDto trace = metrics.Complete();
        simulationService.RecordMetrics("Prim's MST", trace.ExecutionTimeMs, trace.VisitedNodes, trace.ExpandedNodes);

        return new AlgorithmResponseDto<MstResultDto>
        {
            AlgorithmName = "Prim's MST",
            Success = data.Connected,
            Message = data.Connected ? "Cheapest network built." : "Disconnected graph.",
            Trace = trace,
            Data = data
        };
    }
}
