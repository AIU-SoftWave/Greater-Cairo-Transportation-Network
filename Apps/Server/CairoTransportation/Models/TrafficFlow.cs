using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace CairoTransportation.Models;

[Table("traffic_flow")]
public class TrafficFlow
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("id")]
    public long Id { get; set; }

    [Required]
    [Column("road_id")]
    public long RoadId { get; set; }

    [Required]
    [StringLength(20)]
    [Column("period", TypeName = "varchar(20)")]
    public string Period { get; set; } = string.Empty;

    [Required]
    [Column("flow")]
    public int Flow { get; set; }

    [JsonIgnore]
    [ForeignKey(nameof(RoadId))]
    public Road? Road { get; set; }
}
