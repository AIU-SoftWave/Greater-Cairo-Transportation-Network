namespace CairoTransportation.Modules.TransitScheduling.Services.TransitScheduling.DTOs;

/// <summary>
/// Result DTO for transit scheduling optimization.
/// </summary>
public class TransitSchedulingResultDto
{
    /// <summary>Total vehicles available in fleet.</summary>
    public int TotalVehicles { get; set; }

    /// <summary>Total vehicles assigned to routes.</summary>
    public int AssignedVehicles { get; set; }

    /// <summary>Remaining unassigned vehicles.</summary>
    public int RemainingVehicles { get; set; }

    /// <summary>Total passenger demand across all routes.</summary>
    public int TotalDemand { get; set; }

    /// <summary>Estimated passengers served with current allocation.</summary>
    public int EstimatedPassengersServed { get; set; }

    /// <summary>Coverage ratio (served / total demand).</summary>
    public double CoverageRatio { get; set; }

    /// <summary>Number of routes in the network.</summary>
    public int TotalRoutes { get; set; }

    /// <summary>Number of routes that received vehicle allocation.</summary>
    public int ActiveRoutes { get; set; }

    /// <summary>Route allocation details.</summary>
    public List<RouteAllocationDto> RouteAllocations { get; set; } = [];
}

/// <summary>
/// DTO representing vehicle allocation for a single route.
/// </summary>
public class RouteAllocationDto
{
    /// <summary>Route identifier.</summary>
    public string RouteId { get; set; } = string.Empty;

    /// <summary>Route type (bus, metro, etc.).</summary>
    public string RouteType { get; set; } = string.Empty;

    /// <summary>Number of vehicles assigned to this route.</summary>
    public int AssignedVehicles { get; set; }

    /// <summary>Current vehicles assigned (from database).</summary>
    public int? CurrentVehicles { get; set; }

    /// <summary>Estimated daily passengers for this route.</summary>
    public int? DailyPassengers { get; set; }

    /// <summary>Number of stops on this route.</summary>
    public int StopCount { get; set; }

    /// <summary>Estimated frequency in minutes (based on vehicle count).</summary>
    public int? EstimatedFrequencyMinutes { get; set; }

    /// <summary>Estimated passengers served by this allocation.</summary>
    public int EstimatedServed { get; set; }

    /// <summary>Efficiency score (passengers per vehicle).</summary>
    public double EfficiencyScore { get; set; }

    /// <summary>Explanation for the allocation decision.</summary>
    public string Reason { get; set; } = string.Empty;
}

