namespace eTeller.Application.Mappings.ForceTrx
{
    public class ForceTrxVm
    {
        public int TrfId { get; set; }
        public int TrfTrxId { get; set; }
        public string TrfFortyp { get; set; } = string.Empty;
        public string? TrfFortxt { get; set; }
        public DateTime? TrxDatope { get; set; }
        public DateTime? TrxDatval { get; set; }
        public string? ErrDesc { get; set; }
    }
}
