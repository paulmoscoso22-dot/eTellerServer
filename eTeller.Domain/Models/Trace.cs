using System.ComponentModel.DataAnnotations.Schema;

namespace eTeller.Domain.Models
{
    [Table("TRACE")]
    public class Trace
    {
        [Column("TRA_ID")]
        public int TraId { get; set; }

        [Column("TRA_Time")]
        public DateTime TraTime { get; set; }

        [Column("TRA_User")]
        public required string TraUser { get; set; }

        [Column("TRA_FunCode")]
        public required string TraFunCode { get; set; }

        [Column("TRA_SubFun")]
        public string? TraSubFun { get; set; }

        [Column("TRA_Station")]
        public required string TraStation { get; set; }

        [Column("TRA_TabNam")]
        public required string TraTabNam { get; set; }

        [Column("TRA_EntCode")]
        public required string TraEntCode { get; set; }

        [Column("TRA_RevTrxTrace")]
        public string? TraRevTrxTrace { get; set; }

        [Column("TRA_Des")]
        public string? TraDes { get; set; }

        [Column("TRA_ExtRef")]
        public string? TraExtRef { get; set; }

        [Column("TRA_Error")]
        public bool TraError { get; set; }
    }
}
