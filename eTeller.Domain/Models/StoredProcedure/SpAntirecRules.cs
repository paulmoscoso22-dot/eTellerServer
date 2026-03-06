using System.ComponentModel.DataAnnotations.Schema;

namespace eTeller.Domain.Models.StoredProcedure
{
    public class SpAntirecRules : AntirecRules
    {
        [Column("OPT_DES")]
        public required string OptDes { get; set; }

        [Column("CUT_DES")]
        public required string CutDes { get; set; }
    }
}
