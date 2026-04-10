using System.ComponentModel.DataAnnotations.Schema;

namespace eTeller.Domain.Models.StoredProcedure
{
    [Table("USERS_SELECT_ROLE")]
    public class UserSelectRole
    {
        [Column("USR_EXTREF")]
        public string? UsrExtref { get; set; }

        [Column("USR_STATUS")]
        public required string UsrStatus { get; set; }

        [Column("USR_BRA_ID")]
        public required string UsrBraId { get; set; }

        [Column("USR_HOST_ID")]
        public string? UsrHostId { get; set; }

        [Column("USR_ID")]
        public required string UsrId { get; set; }
    }
}
