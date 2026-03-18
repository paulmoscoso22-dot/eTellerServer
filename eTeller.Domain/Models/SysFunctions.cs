using System.ComponentModel.DataAnnotations.Schema;

namespace eTeller.Domain.Models
{
    [Table("sys_FUNCTIONS")]
    public class SysFunctions
    {
        [Column("FUN_ID")]
        public int FunId { get; set; }

        [Column("FUN_NAME")]
        public string FunName { get; set; }

        [Column("FUN_DESCRIPTION")]
        public string? FunDescription { get; set; }

        [Column("FUN_HOSTCODE")]
        public int? FunHostcode { get; set; }

        [Column("Offline")]
        public bool Offline { get; set; }
    }
}
