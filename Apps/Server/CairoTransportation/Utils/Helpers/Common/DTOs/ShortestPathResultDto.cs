namespace CairoTransportation.Utils.Helpers.Common.DTOs;

public class ShortestPathResultDto
{
    public string FromNodeId { get; set; } = string.Empty;
    public string ToNodeId { get; set; } = string.Empty;
    public bool Found { get; set; }
    public double TotalDistance { get; set; }
    public double EstimatedTravelTimeMinutes { get; set; }
    public List<ShortestPathNodeDto> PathNodes { get; set; } = [];
    public List<ShortestPathRoadDto> PathRoads { get; set; } = [];
}

