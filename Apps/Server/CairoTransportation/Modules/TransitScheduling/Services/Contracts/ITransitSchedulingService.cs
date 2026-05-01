using CairoTransportation.Modules.TransitScheduling.Services.TransitScheduling.DTOs;
using CairoTransportation.Utils.Helpers.Common.DTOs;

namespace CairoTransportation.Modules.TransitScheduling.Services.Contracts;

/// <summary>
/// Business service for public transportation scheduling.
/// Optimizes vehicle allocation across different bus and metro lines.
/// </summary>
public interface ITransitSchedulingService
{
    /// <summary>
    /// Distributes a fleet of vehicles across transit routes to maximize passenger coverage.
    /// Uses Resource Allocation Dynamic Programming.
    /// </summary>
    /// <param name="totalVehicles">Total number of vehicles to be assigned.</param>
    Task<AlgorithmResponseDto<TransitSchedulingResultDto>> GenerateScheduleAsync(int totalVehicles);
    Task<List<ShortestPathNodeDto>> GetRouteGeometryAsync(string routeId);
    Task<List<TransferHubDto>> GetTransferHubsAsync();
}
