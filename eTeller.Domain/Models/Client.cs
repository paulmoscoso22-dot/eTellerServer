using System.ComponentModel.DataAnnotations.Schema;

namespace eTeller.Domain.Models
{
    [Table("sys_CLIENT")]
    public class Client
    {
        [Column("CLI_ID")]
        public required string CliId { get; set; }

        [Column("CLI_IP")]
        public required string CliIp { get; set; }

        [Column("CLI_MAC")]
        public required string CliMac { get; set; }

        [Column("CLI_AUTHCODE")]
        public required string CliAuthcode { get; set; }

        [Column("CLI_BRA_ID")]
        public required string CliBraId { get; set; }

        [Column("CLI_DES")]
        public string? CliDes { get; set; }

        [Column("CLI_OFF")]
        public string? CliOff { get; set; }

        [Column("CLI_STATUS")]
        public required string CliStatus { get; set; }

        [Column("CLI_LINGUA")]
        public string? CliLingua { get; set; }

        [Column("CLI_CNT")]
        public int CliCnt { get; set; }

        [Column("CLI_DATCOUNTER")]
        public DateTime? CliDatcounter { get; set; }
    }
}
