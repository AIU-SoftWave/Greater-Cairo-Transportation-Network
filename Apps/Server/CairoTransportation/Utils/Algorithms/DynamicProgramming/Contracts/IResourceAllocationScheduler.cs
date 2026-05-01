using CairoTransportation.Modules.TransitScheduling.Services.TransitScheduling.DTOs;

namespace CairoTransportation.Utils.Algorithms.DynamicProgramming.Contracts;

public interface IResourceAllocationScheduler
{
    /// <summary>
    /// Executes Dynamic Programming to allocate vehicles across transit routes.
    /// </summary>
    TransitSchedulingResultDto GenerateSchedule(List<TransitRouteData> routes, int totalVehicles);
}

public record TransitRouteData(string RouteId, string RouteType, int DailyPassengers, int CapacityPerRoute, int StopCount, int ValuePerVehicle);
