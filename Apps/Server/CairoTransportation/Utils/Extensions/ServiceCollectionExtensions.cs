using CairoTransportation.Data;
using CairoTransportation.Modules.MaintenancePlanning.Services;
using CairoTransportation.Modules.MaintenancePlanning.Services.Contracts;
using CairoTransportation.Modules.NetworkManagement.Services;
using CairoTransportation.Modules.Routing.Services;
using CairoTransportation.Modules.Routing.Services.Contracts;
using CairoTransportation.Modules.Simulation.Services;
using CairoTransportation.Modules.TrafficControl.Services;
using CairoTransportation.Modules.TrafficControl.Services.Contracts;
using CairoTransportation.Modules.TransitScheduling.Services;
using CairoTransportation.Modules.TransitScheduling.Services.Contracts;
using CairoTransportation.Utils.Algorithms.DynamicProgramming;
using CairoTransportation.Utils.Algorithms.DynamicProgramming.Contracts;
using CairoTransportation.Utils.Algorithms.Greedy;
using CairoTransportation.Utils.Algorithms.Greedy.Contracts;
using CairoTransportation.Utils.Algorithms.NetworkExpansion;
using CairoTransportation.Utils.Algorithms.NetworkExpansion.Contracts;
using CairoTransportation.Utils.Algorithms.ShortestPath;
using CairoTransportation.Utils.Algorithms.ShortestPath.Contracts;
using CairoTransportation.Utils.Helpers.Common.Instrumentation;
using CairoTransportation.Utils.Helpers.Graph;
using Microsoft.EntityFrameworkCore;

namespace CairoTransportation.Utils.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddProjectInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        string connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Missing connection string 'DefaultConnection'.");

        services.AddDbContext<TransportationDbContext>(options =>
            options.UseSqlite(connectionString));

        return services;
    }

    public static IServiceCollection AddProjectApplicationServices(this IServiceCollection services)
    {
        services.AddMemoryCache();

        // Infrastructure Services
        services.AddScoped<AlgorithmExecutionMetrics>();
        services.AddScoped<ILocationService, LocationService>();
        services.AddScoped<IRoadService, RoadService>();
        services.AddScoped<ITrafficService, TrafficService>();
        services.AddScoped<IRouteService, RouteService>();
        services.AddScoped<IGraphService, GraphService>();
        services.AddSingleton<ISimulationService, SimulationService>();

        // Pure Algorithm Layer
        services.AddScoped<IDijkstraRoutePlanner, DijkstraRoutePlanner>();
        services.AddScoped<IAStarPathFinder, AStarPathFinder>();
        services.AddScoped<ITimeVaryingRoutePlanner, TimeVaryingRoutePlanner>();
        services.AddScoped<IPrimNetworkExpander, PrimNetworkExpander>();
        services.AddScoped<IKnapsackMaintenanceOptimizer, KnapsackMaintenanceOptimizer>();
        services.AddScoped<IResourceAllocationScheduler, ResourceAllocationScheduler>();
        services.AddScoped<IGreedySignalOptimizer, GreedySignalOptimizer>();

        // Business Service Layer (SRP)
        services.AddScoped<IDijkstraService, DijkstraService>();
        services.AddScoped<IAStarService, AStarService>();
        services.AddScoped<INetworkExpansionService, NetworkExpansionService>();
        services.AddScoped<ITimeVaryingDijkstraService, TimeVaryingDijkstraService>();
        services.AddScoped<IMaintenancePlanningService, MaintenancePlanningService>();
        services.AddScoped<ITransitSchedulingService, TransitSchedulingService>();
        services.AddScoped<ITrafficSignalService, TrafficSignalService>();

        return services;
    }
}
