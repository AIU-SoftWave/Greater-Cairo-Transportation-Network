using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace CairoTransportation.Models;

[Table("route_stops")]
public class RouteStop
{
    [Required]
    [StringLength(10)]
    [Column("route_id", TypeName = "varchar(10)")]
    public string RouteId { get; set; } = string.Empty;

    [Required]
    [StringLength(10)]
    [Column("location_id", TypeName = "varchar(10)")]
    public string LocationId { get; set; } = string.Empty;

    [Required]
    [Column("stop_order")]
    public int StopOrder { get; set; }

    [JsonIgnore]
    [ForeignKey(nameof(RouteId))]
    public TransportRoute? Route { get; set; }

    [JsonIgnore]
    [ForeignKey(nameof(LocationId))]
    public Location? Location { get; set; }
}
