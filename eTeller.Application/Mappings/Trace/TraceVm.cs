namespace eTeller.Application.Mappings.Trace
{
    public class TraceVm
    {
        public int TraId { get; set; }
        public DateTime TraTime { get; set; }
        public required string TraUser { get; set; }
        public required string TraFunCode { get; set; }
        public string? TraSubFun { get; set; }
        public required string TraStation { get; set; }
        public required string TraTabNam { get; set; }
        public required string TraEntCode { get; set; }
        public string? TraRevTrxTrace { get; set; }
        public string? TraDes { get; set; }
        public string? TraExtRef { get; set; }
        public bool TraError { get; set; }
    }
}
