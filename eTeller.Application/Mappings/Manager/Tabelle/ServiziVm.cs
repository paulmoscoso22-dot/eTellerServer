namespace eTeller.Application.Mappings.Manager.Tabelle
{
    public class ServiziVm
    {
        public string SerId { get; set; } = null!;
        public string SerDes { get; set; } = null!;
        public bool? SerRunning { get; set; }
        public string SerDeserr { get; set; } = null!;
        public bool? SerTrace { get; set; }
        public bool SerEmail { get; set; }
        public string? SerSyserrmail { get; set; }
        public string? SerApperrmail { get; set; }
        public bool? SerEnable { get; set; }
        public DateTime? SerLastrun { get; set; }
    }
}
