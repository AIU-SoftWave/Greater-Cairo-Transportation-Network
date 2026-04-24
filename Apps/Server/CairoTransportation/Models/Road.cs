using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CairoTransportation.Models;

[Table("roads")]
public class Road
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("id")]
    public long Id { get; set; }

    [Required]
    [StringLength(10)]
    [Column("from_location_id", TypeName = "varchar(10)")]
    public string FromLocationId { get; set; } = string.Empty;

    [Required]
    [StringLength(10)]
    [Column("to_location_id", TypeName = "varchar(10)")]
    public string ToLocationId { get; set; } = string.Empty;

    [Required]
    [Column("distance")]
    public double Distance { get; set; }

    [Required]
    [Column("capacity")]
    public int Capacity { get; set; }

    [Column("condition")]
    public int? Condition { get; set; }

    [Required]
    [Column("is_existing")]
    public bool IsExisting { get; set; }

    [Column("construction_cost")]
    public double? ConstructionCost { get; set; }

    [ForeignKey(nameof(FromLocationId))]
    public Location? FromLocation { get; set; }

    [ForeignKey(nameof(ToLocationId))]
    public Location? ToLocation { get; set; }

    public ICollection<TrafficFlow> TrafficFlows { get; set; } = [];
    public RoadMaintenance? Maintenance { get; set; }
}
