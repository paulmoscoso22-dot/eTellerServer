using System.ComponentModel.DataAnnotations.Schema;

namespace eTeller.Domain.Models
{
    public class Currency
    {
        [Column("CUR_ID")]
        public string CurId { get; set; }

        [Column("CUR_CUT_ID")]
        public string CurCutId { get; set; }

        [Column("CUR_LONDES")]
        public string CurLondes { get; set; }

        [Column("CUR_SHODES")]
        public string CurShodes { get; set; }

        [Column("CUR_SCALE")]
        public decimal CurScale { get; set; }

        [Column("CUR_ROUNDING")]
        public decimal CurRounding { get; set; }

        [Column("CUR_MINAMN")]
        public decimal CurMinamn { get; set; }

        [Column("CUR_MODDAT")]
        public DateTime CurModdat { get; set; }

        [Column("CUR_HOSTCOD")]
        public string CurHostcod { get; set; }

        [Column("CUR_DECQTY")]
        public decimal CurDecqty { get; set; }

        [Column("CUR_LIMIT")]
        public decimal CurLimit { get; set; }

        [Column("CUR_EMUCUR")]
        public bool CurEmucur { get; set; }

        [Column("CUR_EMUEXR")]
        public decimal CurEmuexr { get; set; }

        [Column("CUR_FINEZZA")]
        public string CurFinezza { get; set; }

        [Column("CUR_TOLRAT")]
        public decimal CurTolrat { get; set; }
    }
}
