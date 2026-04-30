using System.ComponentModel.DataAnnotations.Schema;

namespace eTeller.Domain.Models
{
    [Table("sys_CLIENT_DEVICE")]
    public class ClientDevice
    {
        [Column("CLI_ID")]
        public required string CliId { get; set; }

        [Column("DEV_ID")]
        public int DevId { get; set; }
    }
}
