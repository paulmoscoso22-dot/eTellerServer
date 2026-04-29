using System.ComponentModel.DataAnnotations.Schema;

namespace eTeller.Domain.Models
{
    [Table("sys_DEVICE")]
    public class sys_DEVICE
    {
        [Column("DEV_ID")]
        public int DevId { get; set; }

        [Column("DEV_TYPE")]
        public required string DevType { get; set; }

        [Column("DEV_NAME")]
        public required string DevName { get; set; }

        [Column("DEV_IOADDRESS")]
        public string? DevIoaddress { get; set; }

        [Column("DEV_DRIVER_ADDRESS")]
        public string? DevDriverAddress { get; set; }

        [Column("DEV_BRA_ID")]
        public required string DevBraId { get; set; }
    }
}
