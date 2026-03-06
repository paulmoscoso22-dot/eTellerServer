using System.ComponentModel.DataAnnotations.Schema;

namespace eTeller.Domain.Models
{
    public class HisAntirecAppearer
    {
        [Column("HIS_ID")]
        public int HisId { get; set; }

        [Column("HIS_DATE")]
        public DateTime HisDate { get; set; }

        [Column("ARA_ID")]
        public int AraId { get; set; }

        [Column("ARA_RECDATE")]
        public DateTime AraRecdate { get; set; }

        [Column("ARA_NAME")]
        public required string AraName { get; set; }

        [Column("ARA_BIRTHDATE")]
        public DateTime? AraBirthdate { get; set; }

        [Column("ARA_BIRTHPLACE")]
        public string? AraBirthplace { get; set; }

        [Column("ARA_NATIONALITY")]
        public string? AraNationality { get; set; }

        [Column("ARA_IDDOCNUM")]
        public string? AraIddocnum { get; set; }

        [Column("ARA_DOCEXPDATE")]
        public DateTime? AraDocexpdate { get; set; }

        [Column("ARA_REC_COMPLETE")]
        public bool AraRecComplete { get; set; }

        [Column("ARA_ADDRESS")]
        public string? AraAddress { get; set; }

        [Column("ARA_REPRESENTS")]
        public string? AraRepresents { get; set; }
    }
}
