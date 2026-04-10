namespace eTeller.Application.Mappings.Manager
{
    public class FunctionRoleVm
    {
        public int FunId { get; set; }
        public required string FunName { get; set; }
        public string? FunDescription { get; set; }
        public int AccessLevel { get; set; }
    }
}
