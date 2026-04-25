using CairoTransportation.Data;
using CairoTransportation.Models;
using Microsoft.EntityFrameworkCore;

namespace CairoTransportation.Services;

public interface IRouteService
{
    Task<List<TransportRoute>> GetAllAsync();
    Task<TransportRoute?> GetByIdAsync(string id);
    Task<List<RouteStop>> GetStopsByRouteIdAsync(string routeId);
}

public class RouteService(TransportationDbContext dbContext) : IRouteService
{
    public Task<List<TransportRoute>> GetAllAsync() => dbContext.TransportRoutes.AsNoTracking().ToListAsync();

    public Task<TransportRoute?> GetByIdAsync(string id) =>
        dbContext.TransportRoutes.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);

    public Task<List<RouteStop>> GetStopsByRouteIdAsync(string routeId) =>
        dbContext.RouteStops.AsNoTracking()
            .Include(x => x.Location)
            .Where(x => x.RouteId == routeId)
            .OrderBy(x => x.StopOrder)
            .ToListAsync();
}
