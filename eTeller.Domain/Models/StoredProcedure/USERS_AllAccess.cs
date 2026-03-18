using System.ComponentModel.DataAnnotations.Schema;

namespace eTeller.Domain.Models.StoredProcedure
{
    public class USERS_AllAccess
    {
        [Column("FUN_ID")]
        public int FunId { get; set; }

        [Column("FUN_NAME")]
        public required string FunName { get; set; }

        [Column("FUN_DESCRIPTION")]
        public string? FunDescription { get; set; }

        [Column("FUN_HOSTCODE")]
        public int? FunHostcode { get; set; }

        [Column("Offline")]
        public bool Offline { get; set; }

        [Column("ROLE_NAME")]
        public required string RoleName { get; set; }

        [Column("ACCESS_LEVEL")]
        public int AccessLevel { get; set; }
    }
}
