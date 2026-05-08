using System.ComponentModel.DataAnnotations.Schema;

namespace eTeller.Domain.Models
{
    [Table("SERVIZI")]
    public class SERVIZI
    {
        [Column("SER_ID")]
        public string SerId { get; set; } = null!;

        [Column("SER_DES")]
        public string SerDes { get; set; } = null!;

        [Column("SER_RUNNING")]
        public bool? SerRunning { get; set; }

        [Column("SER_DESERR")]
        public string SerDeserr { get; set; } = null!;

        [Column("SER_TRACE")]
        public bool? SerTrace { get; set; }

        [Column("SER_EMAIL")]
        public bool SerEmail { get; set; }

        [Column("SER_SYSERRMAIL")]
        public string? SerSyserrmail { get; set; }

        [Column("SER_APPERRMAIL")]
        public string? SerApperrmail { get; set; }

        [Column("SER_ENABLE")]
        public bool? SerEnable { get; set; }

        [Column("SER_LASTRUN")]
        public DateTime? SerLastrun { get; set; }
    }
}
