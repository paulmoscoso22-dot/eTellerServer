namespace eTeller.Application.Mappings.Manager
{
    public class CorsiVm
    {
        public string CprCurId1 { get; set; } = string.Empty;
        public string CprCurId2 { get; set; } = string.Empty;
        public string CprCutId { get; set; } = string.Empty;
        public DateTime CprValdat { get; set; }
        public decimal CprRateBuy { get; set; }
        public decimal CprRateSell { get; set; }
        public DateTime? CprDatreg { get; set; }
        public string? CurLondes { get; set; }
        public string? CurShodes { get; set; }
        public string? CurHostcod { get; set; }
        public DateTime? CurModdat { get; set; }
    }
}
