namespace eTeller.Domain.Models.StoredProcedure
{
    public class GiornaleAntiriciclaggio : Transaction
    {
        // Antirrecycling properties
        public int? ArcId { get; set; }

        public int? ArcTrxId { get; set; }

        public int? ArcAppearerId { get; set; }

        public int? ArcBeneficiaryId { get; set; }

        public bool? ArcForced { get; set; }

        public string? ArcMotivation { get; set; }

        // AntiricSubject properties
        public int? AnsId { get; set; }

        public bool? AnsIsAde { get; set; }

        public int? AnsAppearerId { get; set; }

        // AntirecAppearer properties
        public int? AraId { get; set; }

        public DateTime? AraRecdate { get; set; }

        public string? AraName { get; set; }

        public DateTime? AraBirthdate { get; set; }

        public string? AraBirthplace { get; set; }

        public string? AraNationality { get; set; }

        public string? AraIddocnum { get; set; }

        public DateTime? AraDocexpdate { get; set; }

        public bool? AraRecComplete { get; set; }

        public bool? AraIsupdated { get; set; }

        public bool? AraIscanceled { get; set; }

        public string? AraCusId { get; set; }

        public string? AraAddress { get; set; }

        public string? AraRepresents { get; set; }
    }
}


