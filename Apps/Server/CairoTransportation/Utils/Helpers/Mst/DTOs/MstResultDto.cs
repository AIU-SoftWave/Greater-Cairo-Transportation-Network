using CairoTransportation.Services.Algorithms.Common.DTOs;

namespace CairoTransportation.Services.Algorithms.Mst.DTOs;

public class MstResultDto
{
    public bool Connected { get; set; }
    public double TotalConstructionCost { get; set; }
    public int TotalNodes { get; set; }
    public int SelectedRoadCount { get; set; }
    public List<ShortestPathNodeDto> Nodes { get; set; } = [];
    public List<ShortestPathRoadDto> SelectedRoads { get; set; } = [];
}

