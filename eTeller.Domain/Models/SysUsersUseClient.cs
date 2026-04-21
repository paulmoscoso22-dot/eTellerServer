using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eTeller.Domain.Models
{
    [Table("sys_USERSuseCLIENT")]
    public class SysUsersUseClient
    {
        // USR_CLI_ID int NOT NULL (Primary Key)
        [Key]
        [Column("USR_CLI_ID")]
        public int UsrCliId { get; set; }

        // CLI_ID varchar NOT NULL
        [Column("CLI_ID")]
        [Required]
        public string CliId { get; set; } = null!;

        // USR_ID varchar NOT NULL
        [Column("USR_ID")]
        [Required]
        public string UsrId { get; set; } = null!;

        // DATAIN datetime NOT NULL
        [Column("DATAIN")]
        [Required]
        public DateTime DataIn { get; set; }

        // DATAOUT datetime NULL
        [Column("DATAOUT")]
        public DateTime? DataOut { get; set; }

        // LOGOUT bit NOT NULL
        [Column("LOGOUT")]
        [Required]
        public bool Logout { get; set; }

        // FORCED bit NOT NULL
        [Column("FORCED")]
        [Required]
        public bool Forced { get; set; }
    }
}
