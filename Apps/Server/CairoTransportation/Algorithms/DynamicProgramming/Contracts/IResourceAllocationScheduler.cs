using CairoTransportation.Services.Algorithms.TransitScheduling.DTOs;

namespace CairoTransportation.Algorithms.DynamicProgramming.Contracts;

public interface IResourceAllocationScheduler
{
    /// <summary>
    /// Executes Dynamic Programming to allocate vehicles across transit routes.
    /// </summary>
    TransitSchedulingResultDto GenerateSchedule(List<TransitRouteData> routes, int totalVehicles);
}

public record TransitRouteData(string RouteId, string RouteType, int DailyPassengers, int CapacityPerRoute, int StopCount, int ValuePerVehicle);
