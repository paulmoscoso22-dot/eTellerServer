namespace eTeller.Application.Mappings.Vigilanza
{
    public class HisAntirecAppearerVm
    {
        public int HisId { get; set; }
        public DateTime HisDate { get; set; }
        public int AraId { get; set; }
        public DateTime AraRecdate { get; set; }
        public string AraName { get; set; }
        public DateTime? AraBirthdate { get; set; }
        public string? AraBirthplace { get; set; }
        public string? AraNationality { get; set; }
        public string? AraIddocnum { get; set; }
        public DateTime? AraDocexpdate { get; set; }
        public bool AraRecComplete { get; set; }
        public string? AraAddress { get; set; }
        public string? AraRepresents { get; set; }
    }
}
