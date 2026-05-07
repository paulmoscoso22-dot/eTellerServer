using System.ComponentModel.DataAnnotations.Schema;

namespace eTeller.Domain.Models
{
    [Table("NA_TabellaServInt")]
    public class Na_TabellaServInt
    {
        [Column("ID")]
        public int Id { get; set; }

        [Column("DES")]
        public string? Des { get; set; }
    }
}
