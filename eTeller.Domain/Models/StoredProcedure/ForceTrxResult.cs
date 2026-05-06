using System.ComponentModel.DataAnnotations.Schema;

namespace eTeller.Domain.Models.StoredProcedure
{
    public class ForceTrxResult
    {
        [Column("TRF_ID")]
        public int TrfId { get; set; }

        [Column("TRF_TRX_ID")]
        public int TrfTrxId { get; set; }

        [Column("TRF_FORTYP")]
        public string TrfFortyp { get; set; } = string.Empty;

        [Column("TRF_FORTXT")]
        public string? TrfFortxt { get; set; }

        [Column("TRX_DATOPE")]
        public DateTime? TrxDatope { get; set; }

        [Column("TRX_DATVAL")]
        public DateTime? TrxDatval { get; set; }

        [Column("ERR_DESC")]
        public string? ErrDesc { get; set; }
    }
}
