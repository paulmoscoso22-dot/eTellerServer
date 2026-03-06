using System.ComponentModel.DataAnnotations.Schema;

namespace eTeller.Domain.Models
{
    public class AntirecRules
    {
        [Column("ARL_ID")]
        public int ArlId { get; set; }

        [Column("ARL_OP_TYPE_ID")]
        public string ArlOpTypeId { get; set; }

        [Column("ARL_CUR_TYPE_ID")]
        public string ArlCurTypeId { get; set; }

        [Column("ARL_ACCT_ID")]
        public string? ArlAcctId { get; set; }

        [Column("ARL_ACCT_TYPE")]
        public string? ArlAcctType { get; set; }

        [Column("ARL_LIMIT")]
        public decimal ArlLimit { get; set; }

        [Column("ARL_EXCLUDE")]
        public bool ArlExclude { get; set; }

        [Column("ARL_REC_DATE")]
        public DateTime ArlRecDate { get; set; }

        [Column("ARL_VAL_START")]
        public DateTime ArlValStart { get; set; }

        [Column("ARL_VAL_END")]
        public DateTime ArlValEnd { get; set; }

        [Column("ARL_ISCANCELED")]
        public bool ArlIscanceled { get; set; }

        [Column("ARL_ISINTERNAL")]
        public bool ArlIsinternal { get; set; }
    }
}
