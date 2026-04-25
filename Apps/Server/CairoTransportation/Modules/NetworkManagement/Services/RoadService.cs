using CairoTransportation.Data;
using CairoTransportation.Models;
using Microsoft.EntityFrameworkCore;

namespace CairoTransportation.Services;

public interface IRoadService
{
    Task<List<Road>> GetAllAsync();
    Task<Road?> GetByIdAsync(long id);
    Task<List<Road>> GetByFromLocationIdAsync(string locationId);
    Task<RoadMaintenance?> GetMaintenanceByRoadIdAsync(long roadId);
}

public class RoadService(TransportationDbContext dbContext) : IRoadService
{
    public Task<List<Road>> GetAllAsync() => dbContext.Roads.AsNoTracking().ToListAsync();

    public Task<Road?> GetByIdAsync(long id) => dbContext.Roads.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);

    public Task<List<Road>> GetByFromLocationIdAsync(string locationId) =>
        dbContext.Roads.AsNoTracking().Where(x => x.FromLocationId == locationId).ToListAsync();

    public Task<RoadMaintenance?> GetMaintenanceByRoadIdAsync(long roadId) =>
        dbContext.RoadMaintenances.AsNoTracking().FirstOrDefaultAsync(x => x.RoadId == roadId);
}

