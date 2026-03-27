namespace eTeller.Application.Mappings.Client
{
    public class ClientVm
    {
        public string CliId { get; set; }
        public string CliIp { get; set; }
        public string CliMac { get; set; }
        public string CliAuthcode { get; set; }
        public string CliBraId { get; set; }
        public string? CliDes { get; set; }
        public string? CliOff { get; set; }
        public string CliStatus { get; set; }
        public string? CliLingua { get; set; }
        public int CliCnt { get; set; }
        public DateTime? CliDatcounter { get; set; }
    }
}
