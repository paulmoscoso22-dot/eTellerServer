using System.ComponentModel.DataAnnotations.Schema;

namespace eTeller.Domain.Models
{
    public class StAccountType
    {
        [Column("ACT_ID")] public string ActId { get; set; }
        [Column("ACT_DES")] public string? ActDes { get; set; }
    }
}
