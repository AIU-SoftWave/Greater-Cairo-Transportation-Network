using CairoTransportation.Algorithms.DynamicProgramming;
using CairoTransportation.Algorithms.DynamicProgramming.Contracts;
using CairoTransportation.Algorithms.Greedy;
using CairoTransportation.Algorithms.Greedy.Contracts;
using CairoTransportation.Algorithms.NetworkExpansion;
using CairoTransportation.Algorithms.NetworkExpansion.Contracts;
using CairoTransportation.Algorithms.ShortestPath;
using CairoTransportation.Algorithms.ShortestPath.Contracts;
using CairoTransportation.Data;
using CairoTransportation.Services;
using CairoTransportation.Services.Graph;
using CairoTransportation.Services.MaintenancePlanning;
using CairoTransportation.Services.MaintenancePlanning.Contracts;
using CairoTransportation.Services.Routing;
using CairoTransportation.Services.Routing.Contracts;
using CairoTransportation.Services.TrafficControl;
using CairoTransportation.Services.TrafficControl.Contracts;
using CairoTransportation.Services.TransitScheduling;
using CairoTransportation.Services.TransitScheduling.Contracts;
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
        services.AddScoped<ILocationService, LocationService>();
        services.AddScoped<IRoadService, RoadService>();
        services.AddScoped<ITrafficService, TrafficService>();
        services.AddScoped<IRouteService, RouteService>();
        services.AddScoped<IGraphService, GraphService>();

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
