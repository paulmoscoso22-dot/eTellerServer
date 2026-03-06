namespace eTeller.Application.Mappings.Vigilanza
{
    public class SpAntirecRulesVm
    {
        public int ArlId { get; set; }
        public string ArlOpTypeId { get; set; }
        public string ArlCurTypeId { get; set; }
        public string? ArlAcctId { get; set; }
        public string? ArlAcctType { get; set; }
        public decimal ArlLimit { get; set; }
        public bool ArlExclude { get; set; }
        public DateTime ArlRecDate { get; set; }
        public DateTime ArlValStart { get; set; }
        public DateTime ArlValEnd { get; set; }
        public bool ArlIscanceled { get; set; }
        public bool ArlIsinternal { get; set; }
        public required string OptDes { get; set; }
        public required string CutDes { get; set; }
    }
}
