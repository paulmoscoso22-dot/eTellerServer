namespace eTeller.Application.Mappings.HelpInfoViewModels
{
    public class HelpInfoVm
    {
        public string? LastUser { get; set; }
        public DateTime? LastLogoutDate { get; set; }
        public string SystemLanguage { get; set; } = string.Empty;
        public string? FichesPrinter { get; set; }
        public bool IsTwinSafeEnabled { get; set; }
        public bool IsCassaOperationsEnabled { get; set; }
        public string WebApplicationVersion { get; set; } = string.Empty;
        public string? HostUser { get; set; }
        public int? HostVersion { get; set; }
        public DateTime? HostCompileDate { get; set; }
    }
}
