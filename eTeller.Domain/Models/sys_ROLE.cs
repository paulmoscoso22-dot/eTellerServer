using System.ComponentModel.DataAnnotations.Schema;

namespace eTeller.Domain.Models
{
    [Table("sys_ROLE")]
    public class sys_ROLE
    {
        [Column("ROLE_ID")]
        public int RoleId { get; set; }

        [Column("ROLE_NAME")]
        public string RoleName { get; set; }

        [Column("ROLE_DES")]
        public string RoleDes { get; set; }
    }
}
