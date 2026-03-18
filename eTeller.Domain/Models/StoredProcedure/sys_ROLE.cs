using System.ComponentModel.DataAnnotations.Schema;

namespace eTeller.Domain.Models
{
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
