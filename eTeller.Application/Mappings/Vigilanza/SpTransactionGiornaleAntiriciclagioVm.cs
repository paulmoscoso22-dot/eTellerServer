namespace eTeller.Application.Mappings.Vigilanza
{
    public class SpTransactionGiornaleAntiriciclagioVm
    {
        public int TrxId { get; set; }

        public DateTime? TrxDate { get; set; }

        public string? TrxCutId { get; set; }

        public string? CutDes { get; set; }

        public string? TrxOptId { get; set; }

        public string? OptDes { get; set; }

        public string? TrxReport { get; set; }

        public string? TrxCurId { get; set; }

        public decimal? TrxAmount { get; set; }

        public decimal? TrxRate { get; set; }

        public string? AppearerName { get; set; }

        public string? BeneficiaryName { get; set; }

        public int? TrxStatus { get; set; }

        public string? StaDes { get; set; }
    }
}
