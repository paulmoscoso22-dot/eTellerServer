using System.ComponentModel.DataAnnotations.Schema;

namespace eTeller.Domain.Models
{
    [Table("ST_FUNACCTYP")]
    public class StFunAcctyp
    {
        [Column("FAT_ID")]
        public required int FatId { get; set; }

        [Column("FAT_DES")]
        public required string FatDes { get; set; }
    }
}