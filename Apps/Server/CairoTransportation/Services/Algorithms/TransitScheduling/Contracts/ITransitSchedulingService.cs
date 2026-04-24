using CairoTransportation.Services.Algorithms.Common.DTOs;
using CairoTransportation.Services.Algorithms.TransitScheduling.DTOs;

namespace CairoTransportation.Services.Algorithms.TransitScheduling.Contracts;

/// <summary>
/// Service for optimizing transit vehicle allocation across routes using Dynamic Programming.
/// </summary>
public interface ITransitSchedulingService
{
    /// <summary>
    /// Generates an optimal vehicle allocation plan across transit routes.
    /// </summary>
    /// <param name="totalVehicles">Total number of vehicles available in the fleet</param>
    /// <returns>Optimized allocation plan with route frequencies and demand coverage</returns>
    Task<AlgorithmResponseDto<TransitSchedulingResultDto>> GenerateScheduleAsync(int totalVehicles);
}
