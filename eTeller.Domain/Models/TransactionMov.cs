using System.ComponentModel.DataAnnotations.Schema;

namespace eTeller.Domain.Models
{
    public class TransactionMov
    {
        [Column("TRM_ID")]
        public int TrmId { get; set; }

        [Column("TRM_TRX_ID")]
        public int TrmTrxId { get; set; }

        [Column("TRM_MOVTYP")]
        public string TrmMovtyp { get; set; }

        [Column("TRM_BRANCH")]
        public string TrmBranch { get; set; }

        [Column("TRM_ACCTYPE")]
        public string TrmAcctype { get; set; }

        [Column("TRM_ACCOUNT")]
        public string TrmAccount { get; set; }

        [Column("TRM_ACCCUR")]
        public string TrmAcccur { get; set; }

        [Column("TRM_AMOUNT")]
        public decimal TrmAmount { get; set; }

        [Column("TRM_AMTCTV")]
        public decimal TrmAmtctv { get; set; }

        [Column("TRM_VALDAT")]
        public DateTime TrmValdat { get; set; }

        [Column("TRM_DATCRE")]
        public DateTime TrmDatcre { get; set; }

        [Column("TRM_REACOD")]
        public string TrmReacod { get; set; }

        [Column("TRM_FREETX1")]
        public string? TrmFreetx1 { get; set; }

        [Column("TRM_FREETX2")]
        public string? TrmFreetx2 { get; set; }

        [Column("TRM_COSCEN")]
        public string? TrmCoscen { get; set; }

        [Column("TRM_HOSTCOD")]
        public string? TrmHostcod { get; set; }

        [Column("TRM_UPDPOS")]
        public bool? TrmUpdpos { get; set; }
    }
}

