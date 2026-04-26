using CairoTransportation.Data;
using CairoTransportation.Services;
using CairoTransportation.Services.Algorithms.AStar;
using CairoTransportation.Services.Algorithms.AStar.Contracts;
using CairoTransportation.Services.Algorithms.Dijkstra;
using CairoTransportation.Services.Algorithms.Dijkstra.Contracts;
using CairoTransportation.Services.Algorithms.MaintenancePlanning;
using CairoTransportation.Services.Algorithms.MaintenancePlanning.Contracts;
using CairoTransportation.Services.Algorithms.Mst;
using CairoTransportation.Services.Algorithms.Mst.Contracts;
using CairoTransportation.Services.Algorithms.TimeVaryingDijkstra;
using CairoTransportation.Services.Algorithms.TimeVaryingDijkstra.Contracts;
using CairoTransportation.Services.Algorithms.TrafficSignal;
using CairoTransportation.Services.Algorithms.TrafficSignal.Contracts;
using CairoTransportation.Services.Algorithms.TransitScheduling;
using CairoTransportation.Services.Algorithms.TransitScheduling.Contracts;
using CairoTransportation.Services.Graph;
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
        services.AddScoped<ILocationService, LocationService>();
        services.AddScoped<IRoadService, RoadService>();
        services.AddScoped<ITrafficService, TrafficService>();
        services.AddScoped<IRouteService, RouteService>();
        services.AddScoped<IGraphService, GraphService>();
        services.AddScoped<IDijkstraService, DijkstraService>();
        services.AddScoped<IAStarService, AStarService>();
        services.AddScoped<IMstService, MstService>();
        services.AddScoped<ITimeVaryingDijkstraService, TimeVaryingDijkstraService>();
        services.AddScoped<IMaintenancePlanningService, MaintenancePlanningService>();
        services.AddScoped<ITransitSchedulingService, TransitSchedulingService>();
        services.AddScoped<ITrafficSignalService, TrafficSignalService>();

        return services;
    }
}
