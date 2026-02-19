using System.ComponentModel.DataAnnotations.Schema;

namespace eTeller.Domain.Models
{
    public class ST_OperationType
    {
        [Column("OPT_ID")]
        public string OptId { get; set; }

        [Column("OPT_DES")]
        public string OptDes { get; set; }

        [Column("OPT_HOSCOD")]
        public string OptHoscod { get; set; }

        [Column("OPT_ISCREDIT")]
        public string OptIscredit { get; set; }

        [Column("OPT_APT_ID")]
        public string OptAptId { get; set; }

        [Column("OPT_PRTDV")]
        public bool OptPrtdv { get; set; }

        [Column("OPT_ADV_ID")]
        public string? OptAdvId { get; set; }
    }
}
