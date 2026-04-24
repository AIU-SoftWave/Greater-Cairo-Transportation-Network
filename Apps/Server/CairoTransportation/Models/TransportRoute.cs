using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace CairoTransportation.Models;

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

    [JsonIgnore]
    public ICollection<RouteStop> RouteStops { get; set; } = [];
}
