using System.ComponentModel.DataAnnotations.Schema;

namespace eTeller.Domain.Models
{
    [Table("ST_STATOENTITA")]
    public class ST_STATOENTITA
    {
        [Column("STE_ID")]
        public string SteId { get; set; }

        [Column("STE_DES")]
        public string SteDes { get; set; }
    }
}
