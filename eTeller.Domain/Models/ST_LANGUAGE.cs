using System.ComponentModel.DataAnnotations.Schema;

namespace eTeller.Domain.Models
{
    public class ST_LANGUAGE
    {
        [Column("LAN_ID")]
        public string LanId { get; set; }

        [Column("LAN_DES")]
        public string LanDes { get; set; }
    }
}
