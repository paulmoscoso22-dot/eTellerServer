using System.ComponentModel.DataAnnotations.Schema;

namespace eTeller.Domain.Models
{
    public class TotalicCassa
    {
        [Column("TOC_CLI_ID")]
        public string TocCliId { get; set; }

        [Column("TOC_BRA_ID")]
        public string TocBraId { get; set; }

        [Column("TOC_DATA")]
        public DateTime TocData { get; set; }

        [Column("TOC_CONTO")]
        public string TocConto { get; set; }

        [Column("TOC_CUR_ID")]
        public string TocCurId { get; set; }

        [Column("TOC_TIME")]
        public DateTime? TocTime { get; set; }

        [Column("TOC_CUT_ID")]
        public string? TocCutId { get; set; }

        [Column("TOC_SALDO_INI")]
        public decimal? TocSaldoIni { get; set; }

        [Column("TOC_SALDO_INI_CTV")]
        public decimal TocSaldoIniCtv { get; set; }

        [Column("TOC_TOTDARE")]
        public decimal? TocTotdare { get; set; }

        [Column("TOC_TOTDARE_CTV")]
        public decimal? TocTotdareCtv { get; set; }

        [Column("TOC_TOTAVERE")]
        public decimal? TocTotavere { get; set; }

        [Column("TOC_TOTAVERE_CTV")]
        public decimal? TocTotavereCtv { get; set; }

        [Column("TOC_SALDO_FIN")]
        public decimal? TocSaldoFin { get; set; }

        [Column("TOC_SALDO_FIN_CTV")]
        public decimal? TocSaldoFinCtv { get; set; }
    }
}
