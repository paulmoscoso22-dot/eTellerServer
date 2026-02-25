using System.ComponentModel.DataAnnotations.Schema;

namespace eTeller.Domain.Models.StoredProcedure
{
    public class SpTransactionGiornaleAntiriciclagio : Transaction
    {
        //ANTIRECYCLING
        [Column("ARC_ID")]
        public int ArcId { get; set; }

        [Column("ARC_TRX_ID")]
        public int ArcTrxId { get; set; }

        [Column("ARC_APPEARER_ID")]
        public int? ArcAppearerId { get; set; }

        [Column("ARC_BENEFICIARY_ID")]
        public int? ArcBeneficiaryId { get; set; }

        [Column("ARC_FORCED")]
        public bool ArcForced { get; set; }

        [Column("ARC_MOTIVATION")]
        public string? ArcMotivation { get; set; }

        //ANTIRECYCLING SUBJECT
        [Column("ANS_ID")]
        public int AnsId { get; set; }

        [Column("ANS_IS_ADE")]
        public bool AnsIsAde { get; set; }

        [Column("ANS_APPEARER_ID")]
        public int AnsAppearerId { get; set; }

        //ANTIRECYCLING APPEARER
        [Column("ARA_ID")]
        public int AraId { get; set; }

        [Column("ARA_RECDATE")]
        public DateTime AraRecdate { get; set; }

        [Column("ARA_NAME")]
        public string? AraName { get; set; }

        [Column("ARA_BIRTHDATE")]
        public DateTime? AraBirthdate { get; set; }

        [Column("ARA_BIRTHPLACE")]
        public string? AraBirthplace { get; set; }

        [Column("ARA_NATIONALITY")]
        public string? AraNationality { get; set; }

        [Column("ARA_IDDOCNUM")]
        public string? AraIddocnum { get; set; }

        [Column("ARA_DOCEXPDATE")]
        public DateTime? AraDocexpdate { get; set; }

        [Column("ARA_REC_COMPLETE")]
        public bool AraRecComplete { get; set; }

        [Column("ARA_ISUPDATED")]
        public bool AraIsupdated { get; set; }

        [Column("ARA_ISCANCELED")]
        public bool AraIscanceled { get; set; }

        [Column("ARA_CUS_ID")]
        public string? AraCusId { get; set; }

        [Column("ARA_ADDRESS")]
        public string? AraAddress { get; set; }

        [Column("ARA_REPRESENTS")]
        public string? AraRepresents { get; set; }

        
    }
}
