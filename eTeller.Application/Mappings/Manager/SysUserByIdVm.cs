namespace eTeller.Application.Mappings.Manager
{
    public class SysUserByIdVm
    {
        public string UsrId { get; set; }
        public string UsrStatus { get; set; }
        public string? UsrExtref { get; set; }
        public string? UsrHostId { get; set; }
        public string UsrBraId { get; set; }
        public bool UsrChgPas { get; set; }
        public string UsrLingua { get; set; }
    }
}