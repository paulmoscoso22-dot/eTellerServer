namespace eTeller.Application.Mappings.Manager
{
    public class UserSelectRoleVm
    {
        public string? UsrExtref { get; set; }
        public required string UsrStatus { get; set; }
        public required string UsrBraId { get; set; }
        public string? UsrHostId { get; set; }
        public required string UsrId { get; set; }
    }
}
