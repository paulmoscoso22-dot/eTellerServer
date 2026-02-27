using System.ComponentModel.DataAnnotations.Schema;

namespace eTeller.Domain.Models
{
    [Table("sys_USER_SESSIONS")]
    public class UserSession
    {
        [Column("SESSION_ID")]
        public int SessionId { get; set; }

        [Column("USR_ID")]
        public required string UsrId { get; set; }

        [Column("CLI_ID")]
        public string? CliId { get; set; }

        [Column("IP_ADDRESS")]
        public required string IpAddress { get; set; }

        [Column("LOGIN_TIME")]
        public DateTime LoginTime { get; set; }

        [Column("LAST_ACTIVITY")]
        public DateTime LastActivity { get; set; }

        [Column("IS_ACTIVE")]
        public bool IsActive { get; set; }

        [Column("FORCED_LOGIN")]
        public bool ForcedLogin { get; set; }
    }
}
