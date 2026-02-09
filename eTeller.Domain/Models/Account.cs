using System.ComponentModel.DataAnnotations.Schema;

namespace eTeller.Domain.Models
{
    public class Account
    {
        [Column("IAC_ID")]
        public int IacId { get; set; }

        [Column("IAC_ACC_ID")]
        public string IacAccId { get; set; }

        [Column("IAC_CUTID")]
        public string IacCutId { get; set; }

        [Column("IAC_CUR_ID")]
        public string IacCurId { get; set; }

        [Column("IAC_ACT_ID")]
        public string IacActId { get; set; }

        [Column("IAC_BRA_ID")]
        public string IacBraId { get; set; }

        [Column("IAC_DES")]
        public string IacDes { get; set; }

        [Column("IAC_CLI_CASSA")]
        public string IacCliCassa { get; set; }

        [Column("IAC_HOSTPREFIX")]
        public string IacHostprefix { get; set; }
    }
}
