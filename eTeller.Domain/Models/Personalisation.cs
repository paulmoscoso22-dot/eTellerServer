using System.ComponentModel.DataAnnotations.Schema;

namespace eTeller.Domain.Models
{
    [Table("PERSONALISATION")]
    public class Personalisation
    {
        [Column("PAR_ID")]
        public required string ParId { get; set; }

        [Column("PAR_DES")]
        public string? ParDes { get; set; }

        [Column("PAR_VALUE")]
        public string? ParValue { get; set; }
    }
}
