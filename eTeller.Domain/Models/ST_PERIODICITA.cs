using System.ComponentModel.DataAnnotations.Schema;

namespace eTeller.Domain.Models
{
    [Table("ST_PERIODICITA")]
    public class ST_PERIODICITA
    {
        [Column("FUT_PER_ID")]
        public string PeriodicId { get; set; }

        [Column("FUT_PER_DES")]
        public string PeriodicDes { get; set; }
    }
}
