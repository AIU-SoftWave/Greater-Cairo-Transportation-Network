namespace CairoTransportation.Services.Algorithms.Common.DTOs;

public class ShortestPathRoadDto
{
    public long Id { get; set; }
    public string FromNodeId { get; set; } = string.Empty;
    public string ToNodeId { get; set; } = string.Empty;
    public double Distance { get; set; }
    public int Capacity { get; set; }
    public int? Condition { get; set; }
    public bool IsExisting { get; set; }
    public double? ConstructionCost { get; set; }
}
