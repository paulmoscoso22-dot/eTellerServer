using System.ComponentModel.DataAnnotations.Schema;

namespace eTeller.Domain.Models
{
    [Table("ST_TRACE_FUNCTION")]
    public class ST_TRACE_FUNCTION
    {
        [Column("TFC_ID", TypeName = "varchar(50)")]
        public required string TfcId { get; set; }

        [Column("TFC_DES", TypeName = "nvarchar(255)")]
        public required string TfcDes { get; set; }
    }
}
