using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eTeller.Domain.Models;

/// <summary>
/// Entità che rappresenta un codice errore del sistema eTeller.
/// Mappata sulla tabella ST_ERRORCODE.
/// </summary>
[Table("ST_ERRORCODE")]
public sealed class ErrorCode
{
    /// <summary>Codice identificativo univoco dell'errore (es. "1305", "9024")</summary>
    [Key]
    [Column("ERR_ID")]
    [MaxLength(25)]
    public string ErrId { get; set; } = string.Empty;

    /// <summary>Tipo di errore (es. "W" = Warning, "E" = Error, "I" = Info)</summary>
    [Column("ERR_TYP")]
    [MaxLength(25)]
    public string? ErrTyp { get; set; }

    /// <summary>Flag: l'errore causa cancellazione operazione</summary>
    [Column("ERR_CANFLAG")]
    public bool ErrCanFlag { get; set; }

    /// <summary>Flag: l'errore causa conferma obbligatoria</summary>
    [Column("ERR_CONFLAG")]
    public bool ErrConFlag { get; set; }

    /// <summary>Flag: l'errore causa forzatura operazione</summary>
    [Column("ERR_FORFLAG")]
    public bool ErrForFlag { get; set; }

    /// <summary>Codice focus control da evidenziare in caso di errore</summary>
    [Column("ERR_FOC_ID")]
    [MaxLength(25)]
    public string? ErrFocId { get; set; }

    /// <summary>Descrizione soluzione suggerita all'operatore</summary>
    [Column("ERR_DES_SOL")]
    [MaxLength(500)]
    public string? ErrDesSol { get; set; }

    /// <summary>Descrizione in italiano (obbligatoria)</summary>
    [Column("ERR_DESC_IT")]
    [MaxLength(500)]
    public string ErrDescIt { get; set; } = string.Empty;

    /// <summary>Descrizione in inglese</summary>
    [Column("ERR_DESC_EN")]
    [MaxLength(500)]
    public string? ErrDescEn { get; set; }

    /// <summary>Descrizione in francese</summary>
    [Column("ERR_DESC_FR")]
    [MaxLength(500)]
    public string? ErrDescFr { get; set; }

    /// <summary>Descrizione in tedesco</summary>
    [Column("ERR_DESC_DE")]
    [MaxLength(500)]
    public string? ErrDescDe { get; set; }

    /// <summary>
    /// Restituisce la descrizione nella lingua richiesta.
    /// Fallback automatico: lingua richiesta → IT → codice errore grezzo.
    /// </summary>
    /// <param name="lingua">Codice lingua ISO 639-1 (es. "IT", "EN", "FR", "DE")</param>
    public string GetDescrizione(string lingua) =>
        lingua.ToUpperInvariant() switch
        {
            "EN" => ErrDescEn.FallbackTo(ErrDescIt, ErrId),
            "FR" => ErrDescFr.FallbackTo(ErrDescIt, ErrId),
            "DE" => ErrDescDe.FallbackTo(ErrDescIt, ErrId),
            _    => ErrDescIt.FallbackTo(ErrId)
        };
}

/// <summary>Extension helper per il fallback delle stringhe nullable.</summary>
internal static class StringFallbackExtensions
{
    internal static string FallbackTo(this string? value, string fallback1, string fallback2) =>
        !string.IsNullOrWhiteSpace(value)    ? value
        : !string.IsNullOrWhiteSpace(fallback1) ? fallback1
        : fallback2;

    internal static string FallbackTo(this string? value, string fallback) =>
        !string.IsNullOrWhiteSpace(value) ? value : fallback;
}
