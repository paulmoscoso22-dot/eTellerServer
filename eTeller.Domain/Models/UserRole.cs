using System.ComponentModel.DataAnnotations.Schema;

namespace eTeller.Domain.Models
{
    [Table("sys_USERS_ROLE")]
    public class UserRole
    {
        [Column("ROLE_ID")]
        public int RoleId { get; set; }

        [Column("USER_ID")]
        public string UserId { get; set; }
    }
}