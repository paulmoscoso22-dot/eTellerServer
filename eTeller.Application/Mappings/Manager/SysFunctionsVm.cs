namespace eTeller.Application.Mappings.Manager
{
    public class SysFunctionsVm
    {
        public int FunId { get; set; }

        public string FunName { get; set; }

        public string? FunDescription { get; set; }

        public int? FunHostcode { get; set; }

        public bool Offline { get; set; }
    }
}
