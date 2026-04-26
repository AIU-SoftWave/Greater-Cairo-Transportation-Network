namespace CairoTransportation.Services.Algorithms.MaintenancePlanning.DTOs;

/// <summary>
/// Result DTO for maintenance planning optimization.
/// </summary>
public class MaintenancePlanningResultDto
{
    public double Budget { get; set; }
    public double TotalCost { get; set; }
    public double RemainingBudget { get; set; }
    public int TotalPriorityScore { get; set; }
    public int SelectedRoadCount { get; set; }
    public int TotalCandidateRoads { get; set; }
    public double ExpectedConditionImprovement { get; set; }
    public List<MaintenanceRoadDto> SelectedRoads { get; set; } = [];
    public List<MaintenanceRoadDto> NotSelectedRoads { get; set; } = [];
}

/// <summary>
/// DTO representing a road in the maintenance plan.
/// </summary>
public class MaintenanceRoadDto
{
    public long RoadId { get; set; }
    public string? FromLocation { get; set; }
    public string? ToLocation { get; set; }
    public double CurrentCondition { get; set; }
    public double ExpectedNewCondition { get; set; }
    public double EstimatedCost { get; set; }
    public int Priority { get; set; }
    public string Reason { get; set; } = string.Empty;
}
