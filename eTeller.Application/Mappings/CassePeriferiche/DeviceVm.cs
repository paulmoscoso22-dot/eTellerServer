namespace eTeller.Application.Mappings.CassePeriferiche
{
    public class DeviceVm
    {
        public int DevId { get; set; }
        public string DevType { get; set; } = null!;
        public string DevName { get; set; } = null!;
        public string? DevIoaddress { get; set; }
        public string? DevDriverAddress { get; set; }
        public string DevBraId { get; set; } = null!;
    }
}
