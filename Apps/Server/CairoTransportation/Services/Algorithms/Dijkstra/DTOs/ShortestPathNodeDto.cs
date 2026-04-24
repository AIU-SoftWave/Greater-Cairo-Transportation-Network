namespace CairoTransportation.Services.Algorithms.Dijkstra.DTOs;

public class ShortestPathNodeDto
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public double? X { get; set; }
    public double? Y { get; set; }
    public int? Population { get; set; }
    public bool IsCritical { get; set; }
}
