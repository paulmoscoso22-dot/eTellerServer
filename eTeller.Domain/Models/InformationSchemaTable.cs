namespace eTeller.Domain.Models
{
    /// <summary>
    /// Proiezione keyless della vista di sistema INFORMATION_SCHEMA.TABLES.
    /// Configurazione HasNoKey() definita in eTellerDbContext.OnModelCreating via Fluent API.
    /// Usata esclusivamente per controlli di esistenza tabella tramite LINQ — nessun raw SQL.
    /// </summary>
    public class InformationSchemaTable
    {
        public string TABLE_CATALOG { get; set; } = string.Empty;
        public string TABLE_SCHEMA  { get; set; } = string.Empty;
        public string TABLE_NAME    { get; set; } = string.Empty;
        public string TABLE_TYPE    { get; set; } = string.Empty;
    }
}
