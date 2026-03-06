namespace eTeller.Application.Mappings.Vigilanza
{
    public class AppearerAllVm
    {
        public string Nome { get; set; } = string.Empty;
        public int AraId { get; set; }
        public DateTime AraRecdate { get; set; }
        public string AraName { get; set; } = string.Empty;
        public DateTime? AraBirthdate { get; set; }
        public string? AraBirthplace { get; set; }
        public string? AraNationality { get; set; }
        public string? AraIddocnum { get; set; }
        public DateTime? AraDocexpdate { get; set; }
        public bool AraRecComplete { get; set; }
        public bool AraIsupdated { get; set; }
        public bool AraIscanceled { get; set; }
        public string? AraCusId { get; set; }
        public string? AraRepresents { get; set; }
        public string? AraAddress { get; set; }
    }
}
