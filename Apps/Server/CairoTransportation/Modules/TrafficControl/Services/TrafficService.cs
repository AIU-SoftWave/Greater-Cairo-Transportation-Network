using CairoTransportation.Data;
using CairoTransportation.Models;
using Microsoft.EntityFrameworkCore;

namespace CairoTransportation.Services;

public interface ITrafficService
{
    Task<List<TrafficFlow>> GetByRoadIdAsync(long roadId);
    Task<List<TrafficFlow>> GetByPeriodAsync(string period);
    Task<TrafficPeriodMultiplier?> GetPeriodMultiplierAsync(string period);
    Task<List<TrafficPeriodMultiplier>> GetPeriodMultipliersAsync();
}

public class TrafficService(TransportationDbContext dbContext) : ITrafficService
{
    public Task<List<TrafficFlow>> GetByRoadIdAsync(long roadId) =>
        dbContext.TrafficFlows.AsNoTracking().Where(x => x.RoadId == roadId).ToListAsync();

    public Task<List<TrafficFlow>> GetByPeriodAsync(string period)
    {
        string normalizedPeriod = period.Trim().ToUpperInvariant();
        return dbContext.TrafficFlows.AsNoTracking().Where(x => x.Period == normalizedPeriod).ToListAsync();
    }

    public Task<TrafficPeriodMultiplier?> GetPeriodMultiplierAsync(string period)
    {
        string normalizedPeriod = period.Trim().ToUpperInvariant();
        return dbContext.TrafficPeriodMultipliers.AsNoTracking().FirstOrDefaultAsync(x => x.Period == normalizedPeriod);
    }

    public Task<List<TrafficPeriodMultiplier>> GetPeriodMultipliersAsync() =>
        dbContext.TrafficPeriodMultipliers.AsNoTracking().OrderBy(x => x.Period).ToListAsync();
}

