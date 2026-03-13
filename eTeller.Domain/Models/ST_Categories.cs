using System.ComponentModel.DataAnnotations.Schema;

namespace eTeller.Domain.Models
{
    [Table("ST_CATEGORIES")]
    public class ST_Categories
    {
        [Column("ACA_ID")]
        public string AcaId { get; set; }

        [Column("ACA_DES")]
        public string AcaDes { get; set; }

        [Column("ACA_SCONFINA")]
        public bool AcaSconfina { get; set; }

        [Column("ACA_LIMMEN")]
        public decimal AcaLimmen { get; set; }

        [Column("ACA_DEBIT")]
        public bool AcaDebit { get; set; }

        [Column("ACA_CREDIT")]
        public bool AcaCredit { get; set; }

        [Column("ACA_CRELIM")]
        public decimal AcaCrelim { get; set; }

        [Column("ACA_SAVETXT")]
        public bool AcaSavetxt { get; set; }

        [Column("ACA_STE_ID")]
        public string AcaSteId { get; set; }

        [Column("ACA_CURTYP")]
        public string AcaCurtyp { get; set; }

        [Column("ACA_USESPREAD")]
        public bool AcaUsespread { get; set; }
    }
}
