namespace CairoTransportation.Services.Algorithms.TransitScheduling.DTOs;

public class TransferHubDto
{
    public string LocationId { get; set; } = string.Empty;
    public string LocationName { get; set; } = string.Empty;
    public int RouteCount { get; set; }
    public List<string> RouteIds { get; set; } = [];
    public double X { get; set; }
    public double Y { get; set; }
}
