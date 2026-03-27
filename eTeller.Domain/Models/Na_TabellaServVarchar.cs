using System.ComponentModel.DataAnnotations.Schema;

namespace eTeller.Domain.Models
{
    [Table("NA_TabellaServVarchar")]
    public class Na_TabellaServVarchar
    {
        [Column("ID")]
        public string? Id { get; set; }

        [Column("DES")]
        public string? Des { get; set; }
    }
}
