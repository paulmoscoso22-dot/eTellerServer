using System.ComponentModel.DataAnnotations.Schema;

namespace eTeller.Domain.Models
{
    [Table("sys_USERS")]
    public class User
    {
        [Column("USR_ID")]
        public required string UsrId { get; set; }

        [Column("USR_HOST_ID")]
        public string? UsrHostId { get; set; }

        [Column("USR_BRA_ID")]
        public required string UsrBraId { get; set; }

        [Column("USR_STATUS")]
        public required string UsrStatus { get; set; }

        [Column("USR_EXTREF")]
        public string? UsrExtref { get; set; }

        [Column("USR_PASS")]
        public required string UsrPass { get; set; }

        [Column("USR_CHG_PAS")]
        public bool UsrChgPas { get; set; }

        [Column("USR_LINGUA")]
        public required string UsrLingua { get; set; }

        [Column("USR_TENTATIVI")]
        public int UsrTentativi { get; set; }

        [Column("USR_MAIL")]
        public string? UsrMail { get; set; }
    }
}
