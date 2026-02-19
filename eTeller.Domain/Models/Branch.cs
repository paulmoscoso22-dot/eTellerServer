using System.ComponentModel.DataAnnotations.Schema;

namespace eTeller.Domain.Models
{
    public class Branch
    {
        [Column("BRA_ID")]
        public string BraId { get; set; }

        [Column("BRA_DES")]
        public string BraDes { get; set; }

        [Column("BRA_HOSCOD")]
        public string BraHoscod { get; set; }
    }
}
