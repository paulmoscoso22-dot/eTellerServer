namespace eTeller.Application.Mappings.Transaction
{
    public class TransactionOperationAnnullateVm
    {
        public int TrxId { get; set; }
        public string? Genere { get; set; }
        public string? Tipo { get; set; }
        public string? Report { get; set; }
        public string? TrxCassa { get; set; }
        public string? TrxUsrId { get; set; }
        public string? HostTrace { get; set; }
        public string? TrxText1 { get; set; }
    }
}
