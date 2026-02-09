namespace eTeller.Domain.Models.View
{
    public class AntirecAppearerView
    {
        public string? Nome { get; set; }
        public int? ARA_ID { get; set; }
        public DateTime? ARA_RECDATE { get; set; }
        public string? ARA_NAME { get; set; }
        public DateTime? ARA_BIRTHDATE { get; set; }
        public string? ARA_BIRTHPLACE { get; set; }
        public string? ARA_NATIONALITY { get; set; }
        public string? ARA_IDDOCNUM { get; set; }
        public DateTime? ARA_DOCEXPDATE { get; set; }
        public bool? ARA_REC_COMPLETE { get; set; }
        public bool? ARA_ISUPDATED { get; set; }
        public bool? ARA_ISCANCELED { get; set; }
        public string? ARA_CUS_ID { get; set; }
        public string? ARA_REPRESENTS { get; set; }
        public string? ARA_ADDRESS { get; set; }
    }
}
