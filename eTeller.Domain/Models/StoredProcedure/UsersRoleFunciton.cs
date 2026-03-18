using System.ComponentModel.DataAnnotations.Schema;

namespace eTeller.Domain.Models.StoredProcedure
{
    public class UsersRoleFunction
    {
        [Column("FUN_NAME")]
        public required string FunName { get; set; }

        [Column("USR_EXTREF")]
        public string? UsrExtref { get; set; }

        [Column("FAT_DES")]
        public required string FatDes { get; set; }

        [Column("FUN_ID")]
        public int FunId { get; set; }

        [Column("USR_ID")]
        public required string UsrId { get; set; }

        [Column("BRA_DES")]
        public string? BraDes { get; set; }

        [Column("STE_DES")]
        public required string SteDes { get; set; }

        [Column("BRA_ID")]
        public required string BraId { get; set; }

        [Column("STE_ID")]
        public required string SteId { get; set; }

        [Column("FUN_DESCRIPTION")]
        public string? FunDescription { get; set; }

        [Column("FAT_ID")]
        public int? FatId { get; set; }
    }
}
