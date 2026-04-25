using CairoTransportation.Data;
using CairoTransportation.Models;
using Microsoft.EntityFrameworkCore;

namespace CairoTransportation.Services;

public interface ILocationService
{
    Task<List<Location>> GetAllAsync();
    Task<Location?> GetByIdAsync(string id);
}

public class LocationService(TransportationDbContext dbContext) : ILocationService
{
    public Task<List<Location>> GetAllAsync() => dbContext.Locations.AsNoTracking().ToListAsync();

    public Task<Location?> GetByIdAsync(string id) => dbContext.Locations.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
}

