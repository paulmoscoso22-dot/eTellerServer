using System.ComponentModel.DataAnnotations.Schema;

namespace eTeller.Domain.Models
{
    public class ST_CurrencyType
    {
        [Column("CUT_ID")]
        public string CutId { get; set; }

        [Column("CUT_DES")]
        public string CutDes { get; set; }

        [Column("CUT_HOST_COD")]
        public string? CutHostCod { get; set; }

        [Column("CUT_SPR_BUY")]
        public decimal CutSprBuy { get; set; }

        [Column("CUT_SPR_SELL")]
        public decimal CutSprSell { get; set; }

        [Column("CUT_SPR_DIP_BUY")]
        public decimal CutSprDipBuy { get; set; }

        [Column("CUT_SPR_DIP_SELL")]
        public decimal CutSprDipSell { get; set; }
    }
}
