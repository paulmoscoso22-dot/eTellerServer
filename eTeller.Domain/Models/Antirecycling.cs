using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eTeller.Domain.Models;

/// <summary>
/// Entità che rappresenta un record di controllo antiriciclaggio nel sistema eTeller.
/// Mappata sulla tabella ANTIRECYCLING.
/// </summary>
[Table("ANTIRECYCLING")]
public sealed class Antirecycling
{
    /// <summary>Identificativo univoco del record di antiriciclaggio</summary>
    [Key]
    [Column("ARC_ID")]
    public int ArcId { get; set; }

    /// <summary>Identificativo della transazione associata</summary>
    [Column("ARC_TRX_ID")]
    public int ArcTrxId { get; set; }

    /// <summary>Identificativo dell'apparecchio (nullable)</summary>
    [Column("ARC_APPEARER_ID")]
    public int? ArcAppearerId { get; set; }

    /// <summary>Identificativo del beneficiario (nullable)</summary>
    [Column("ARC_BENEFICIARY_ID")]
    public int? ArcBeneficiaryId { get; set; }

    /// <summary>Flag: transazione forzata</summary>
    [Column("ARC_FORCED")]
    public bool ArcForced { get; set; }

    /// <summary>Motivazione della forzatura (nullable)</summary>
    [Column("ARC_MOTIVATION")]
    [MaxLength(500)]
    public string? ArcMotivation { get; set; }
}
