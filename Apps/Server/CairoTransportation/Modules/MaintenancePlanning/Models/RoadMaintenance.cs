using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using CairoTransportation.Modules.NetworkManagement.Models;

namespace CairoTransportation.Modules.MaintenancePlanning.Models;

[Table("road_maintenance")]
public class RoadMaintenance
{
    [Key]
    [Column("road_id")]
    public long RoadId { get; set; }

    [Column("priority")]
    public int? Priority { get; set; }

    [Column("estimated_cost")]
    public double? EstimatedCost { get; set; }

    [JsonIgnore]
    [ForeignKey(nameof(RoadId))]
    public Road? Road { get; set; }
}

