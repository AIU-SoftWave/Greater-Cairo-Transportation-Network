using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CairoTransportation.Models;

[Table("transport_demand")]
public class TransportDemand
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
    [Column("daily_passengers")]
    public int DailyPassengers { get; set; }

    [ForeignKey(nameof(FromLocationId))]
    public Location? FromLocation { get; set; }

    [ForeignKey(nameof(ToLocationId))]
    public Location? ToLocation { get; set; }
}
