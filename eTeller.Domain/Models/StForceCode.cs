using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eTeller.Domain.Models;

[Table("ST_FORCECODE")]
public sealed class StForceCode
{
    [Key]
    [Column("FOC_ID")]
    [MaxLength(5)]
    public string FocId { get; set; } = string.Empty;

    [Column("FOC_DES")]
    [MaxLength(50)]
    public string? FocDes { get; set; }
}
