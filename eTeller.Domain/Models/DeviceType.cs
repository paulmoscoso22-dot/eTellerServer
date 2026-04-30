using System.ComponentModel.DataAnnotations.Schema;

namespace eTeller.Domain.Models
{
    [Table("sys_DEVICETYPE")]
    public class DeviceType
    {
        [Column("DTY_ID")]
        public required string DtyId { get; set; }

        [Column("DTY_DES")]
        public string? DtyDes { get; set; }
    }
}
