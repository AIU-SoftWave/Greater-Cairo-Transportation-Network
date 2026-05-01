using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace CairoTransportation.Modules.Routing.Models;

[Table("transport_routes")]
public class TransportRoute
{
    [Key]
    [StringLength(10)]
    [Column("id", TypeName = "varchar(10)")]
    public string Id { get; set; } = string.Empty;

    [Required]
    [StringLength(20)]
    [Column("type", TypeName = "varchar(20)")]
    public string Type { get; set; } = string.Empty;

    [Column("daily_passengers")]
    public int? DailyPassengers { get; set; }

    [Column("vehicles_assigned")]
    public int? VehiclesAssigned { get; set; }

    [Column("capacity_per_unit", TypeName = "int")]
    public int CapacityPerUnit { get; set; } = 50; // Default capacity per vehicle/unit
    [JsonIgnore]
    public ICollection<RouteStop> RouteStops { get; set; } = [];
}

