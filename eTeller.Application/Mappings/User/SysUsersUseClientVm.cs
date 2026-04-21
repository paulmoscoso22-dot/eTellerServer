namespace eTeller.Application.Mappings.User
{
    public class SysUsersUseClientVm
    {
        public int UsrCliId { get; set; }
        public string CliId { get; set; } = null!;
        public string UsrId { get; set; } = null!;
        public System.DateTime DataIn { get; set; }
        public System.DateTime? DataOut { get; set; }
        public bool Forced { get; set; }
    }
}
