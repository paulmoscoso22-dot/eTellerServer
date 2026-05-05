using System.ComponentModel.DataAnnotations.Schema;

namespace eTeller.Domain.Models.StoredProcedure
{
    public class CorsiResult
    {
        [Column("CPR_CUR_ID1")]
        public string CprCurId1 { get; set; } = string.Empty;

        [Column("CPR_CUR_ID2")]
        public string CprCurId2 { get; set; } = string.Empty;

        [Column("CPR_CUT_ID")]
        public string CprCutId { get; set; } = string.Empty;

        [Column("CPR_VALDAT")]
        public DateTime CprValdat { get; set; }

        [Column("CPR_RATE_BUY")]
        public decimal CprRateBuy { get; set; }

        [Column("CPR_RATE_SELL")]
        public decimal CprRateSell { get; set; }

        [Column("CPR_DATREG")]
        public DateTime? CprDatreg { get; set; }

        [Column("CUR_LONDES")]
        public string? CurLondes { get; set; }

        [Column("CUR_SHODES")]
        public string? CurShodes { get; set; }

        [Column("CUR_HOSTCOD")]
        public string? CurHostcod { get; set; }

        [Column("CUR_MODDAT")]
        public DateTime? CurModdat { get; set; }
    }
}
