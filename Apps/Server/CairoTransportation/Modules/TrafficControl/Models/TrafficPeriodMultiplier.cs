using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace CairoTransportation.Modules.TrafficControl.Models;

[Table("traffic_period_multipliers")]
public class TrafficPeriodMultiplier
{
    [Key]
    [StringLength(20)]
    [Column("period", TypeName = "varchar(20)")]
    public string Period { get; set; } = string.Empty;

    [Required]
    [Column("multiplier")]
    public double Multiplier { get; set; }

    [JsonIgnore]
    public ICollection<TrafficFlow> TrafficFlows { get; set; } = [];
}

