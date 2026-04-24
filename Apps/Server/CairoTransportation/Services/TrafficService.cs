using CairoTransportation.Data;
using CairoTransportation.Models;
using Microsoft.EntityFrameworkCore;

namespace CairoTransportation.Services;

public interface ITrafficService
{
    Task<List<TrafficFlow>> GetByRoadIdAsync(long roadId);
    Task<List<TrafficFlow>> GetByPeriodAsync(string period);
}

public class TrafficService(TransportationDbContext dbContext) : ITrafficService
{
    public Task<List<TrafficFlow>> GetByRoadIdAsync(long roadId) =>
        dbContext.TrafficFlows.AsNoTracking().Where(x => x.RoadId == roadId).ToListAsync();

    public Task<List<TrafficFlow>> GetByPeriodAsync(string period) =>
        dbContext.TrafficFlows.AsNoTracking().Where(x => x.Period == period).ToListAsync();
}
