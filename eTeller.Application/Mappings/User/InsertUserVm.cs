namespace eTeller.Application.Mappings.User
{
    public class InsertUserVm
    {
        public string UsrId { get; set; }
        public string? UsrHostId { get; set; }
        public string UsrBraId { get; set; }
        public string UsrStatus { get; set; }
        public string? UsrExtref { get; set; }
        public string UsrLingua { get; set; }
    }
}