using System.ComponentModel.DataAnnotations.Schema;

namespace eTeller.Domain.Models.StoredProcedure
{
    [Table("FUNCTION_ROLE")]
    public class FunctionRole
    {
        [Column("FUN_ID")]
        public required int FunId { get; set; }

        [Column("FUN_NAME")]
        public required string FunName { get; set; }

        [Column("FUN_DESCRIPTION")]
        public string? FunDescription { get; set; }

        [Column("ACCESS_LEVEL")]
        public required int AccessLevel { get; set; }
    }
}
