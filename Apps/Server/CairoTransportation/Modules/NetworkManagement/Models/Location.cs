using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using CairoTransportation.Modules.Routing.Models;
using CairoTransportation.Modules.TransitScheduling.Models;

namespace CairoTransportation.Modules.NetworkManagement.Models;

[Table("locations")]
public class Location
{
    [Key]
    [StringLength(10)]
    [Column("id", TypeName = "varchar(10)")]
    public string Id { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    [Column("name", TypeName = "varchar(100)")]
    public string Name { get; set; } = string.Empty;

    [Required]
    [StringLength(20)]
    [Column("type", TypeName = "varchar(20)")]
    public string Type { get; set; } = string.Empty;

    [StringLength(50)]
    [Column("category", TypeName = "varchar(50)")]
    public string? Category { get; set; }

    [Column("population")]
    public int? Population { get; set; }

    [Required]
    [Column("x")]
    public double X { get; set; }

    [Required]
    [Column("y")]
    public double Y { get; set; }

    [Column("is_critical")]
    public bool IsCritical { get; set; }

    [JsonIgnore] public ICollection<Road> OutgoingRoads { get; set; } = [];
    [JsonIgnore] public ICollection<Road> IncomingRoads { get; set; } = [];
    [JsonIgnore] public ICollection<RouteStop> RouteStops { get; set; } = [];
    [JsonIgnore] public ICollection<TransportDemand> OriginDemands { get; set; } = [];
    [JsonIgnore] public ICollection<TransportDemand> DestinationDemands { get; set; } = [];
}

