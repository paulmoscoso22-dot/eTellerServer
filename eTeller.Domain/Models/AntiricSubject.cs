using System.ComponentModel.DataAnnotations.Schema;

namespace eTeller.Domain.Models
{
    public class AntiricSubject
    {
        [Column("ANS_ID")]
        public int AnsId { get; set; }

        [Column("ANS_IS_ADE")]
        public bool AnsIsAde { get; set; }

        [Column("ANS_APPEARER_ID")]
        public int AnsAppearerId { get; set; }
    }
}
