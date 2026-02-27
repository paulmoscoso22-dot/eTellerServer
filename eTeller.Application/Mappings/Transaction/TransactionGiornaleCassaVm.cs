namespace eTeller.Application.Mappings.Transaction
{
    public class TransactionGiornaleCassaVm
    {
        public int TrxId { get; set; }
        public string? Genere { get; set; }
        public string? Tipo { get; set; }
        public string? Report { get; set; }
        public string? Nop { get; set; }
        public string? TrxDivope { get; set; }
        public string? TrxDivctp { get; set; }
        public decimal? TrxImpope { get; set; }
        public string? BigliettiBanca { get; set; }
        public string? NonContanti { get; set; }
        public string? Contanti { get; set; }
        public string? ImpCHF { get; set; }
        public string? Stato { get; set; }
    }
}
