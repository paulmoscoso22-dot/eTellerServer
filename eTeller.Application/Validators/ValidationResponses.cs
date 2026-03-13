using eTeller.Application.Mappings.Prelievo;

namespace eTeller.Application.Validators;

/// <summary>
/// Risposta standard dell'API in caso di errore di validazione.
/// Angular riceve questo oggetto e mostra i messaggi all'utente.
/// </summary>
public class ValidationErrorResponse
{
    public bool Success { get; set; } = false;

    /// <summary>Lista degli errori di validazione</summary>
    public List<ValidationError> Errors { get; set; } = new();
}

public class ValidationError
{
    /// <summary>Nome del campo che ha causato l'errore (es. "NumeroConto")</summary>
    public string Field { get; set; } = string.Empty;

    /// <summary>Messaggio leggibile dall'utente</summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>Codice errore del vecchio sistema (es. "1305") — utile per compatibilità</summary>
    public string ErrorCode { get; set; } = string.Empty;
}

/// <summary>
/// Risposta in caso di successo della validazione.
/// Contiene i dati validati e normalizzati, pronti per la Fase 2.
/// </summary>
public class CaricaValidataResponse
{
    public bool Success { get; set; } = true;

    /// <summary>Dati normalizzati pronti per il calcolo del cambio (Fase 2)</summary>
    public PrelievoViewVm DatiValidati { get; set; } = new();

    /// <summary>Eventuali avvisi non bloccanti (es. data valuta corretta automaticamente)</summary>
    public List<string> Avvisi { get; set; } = new();
}
