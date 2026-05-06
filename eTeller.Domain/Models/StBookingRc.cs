using System.ComponentModel.DataAnnotations.Schema;

namespace eTeller.Domain.Models
{
    public class StBookingRc
    {
        [Column("BRC_CUT_ID")] public string BrcCutId { get; set; }
        [Column("BRC_OPT_ID")] public string BrcOptId { get; set; }
        [Column("BRC_ACT_ID")] public string BrcActId { get; set; }
        [Column("BRC_CODCAU")] public string BrcCodcau { get; set; }
        [Column("BRC_CODCAUSTO")] public string BrcCodcausto { get; set; }
        [Column("BRC_TEXT1")] public string? BrcText1 { get; set; }
        [Column("BRC_TEXT2")] public string? BrcText2 { get; set; }
    }
}
