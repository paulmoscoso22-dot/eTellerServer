using System.ComponentModel.DataAnnotations.Schema;

namespace eTeller.Domain.Models
{
    public class Antirrecycling
    {
        [Column("ARC_ID")]
        public int ArcId { get; set; }

        [Column("ARC_TRX_ID")]
        public int ArcTrxId { get; set; }

        [Column("ARC_APPEARER_ID")]
        public int? ArcAppearerId { get; set; }

        [Column("ARC_BENEFICIARY_ID")]
        public int? ArcBeneficiaryId { get; set; }

        [Column("ARC_FORCED")]
        public bool ArcForced { get; set; }

        [Column("ARC_MOTIVATION")]
        public string? ArcMotivation { get; set; }
    }
}
