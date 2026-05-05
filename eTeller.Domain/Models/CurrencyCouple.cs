using System.ComponentModel.DataAnnotations.Schema;

namespace eTeller.Domain.Models
{
    [Table("CURRENCY_COUPLE")]
    public class CurrencyCouple
    {
        [Column("CUC_CUR_1")]
        public string CucCur1 { get; set; } = string.Empty;

        [Column("CUC_CUR_2")]
        public string CucCur2 { get; set; } = string.Empty;

        [Column("CUC_LONDES")]
        public string? CucLondes { get; set; }

        [Column("CUC_SHODES")]
        public string? CucShodes { get; set; }

        [Column("CUC_SIZE")]
        public decimal? CucSize { get; set; }

        [Column("CUC_EXCDIR")]
        public string? CucExcdir { get; set; }
    }
}
