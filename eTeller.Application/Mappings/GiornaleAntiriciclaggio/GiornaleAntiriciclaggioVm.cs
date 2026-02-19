namespace eTeller.Application.Mappings
{
    public class GiornaleAntiriciclaggioVm
    {
        // Transaction properties
        public int TrxId { get; set; }

        public string TrxCassa { get; set; }

        public DateTime? TrxDatope { get; set; }

        public int? TrxStatus { get; set; }

        public string TrxBraId { get; set; }

        public string TrxDailySequence { get; set; }

        public string TrxCutId { get; set; }

        public string TrxOptId { get; set; }

        public string TrxDivope { get; set; }

        public decimal? TrxImpope { get; set; }

        public bool? TrxReverse { get; set; }

        // Antirrecycling properties
        public int? ArcTrxId { get; set; }

        public bool? ArcForced { get; set; }

        public string? ArcMotivation { get; set; }

        // AntiricSubject properties
        public int? AnsId { get; set; }

        public int? AnsAppearerId { get; set; }

        public bool? AnsIsAde { get; set; }

        // AntirecAppearer properties
        public string? AraName { get; set; }

        public DateTime? AraRecdate { get; set; }
    }
}
