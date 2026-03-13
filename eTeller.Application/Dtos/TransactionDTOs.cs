namespace eTeller.Application.DTOs;

// ─────────────────────────────────────────────────────────────────────────────
// DTOs per ITransactionSpRepository
// ─────────────────────────────────────────────────────────────────────────────

/// <summary>
/// Risultato della verifica di validità di una transazione.
/// Contiene informazioni su errori, requisiti beneficiario e stato di validazione.
/// </summary>
public sealed record VerificaTransazioneResult(
    /// <summary>
    /// false = transazione NON valida (MsgError popolato) → throw BadRequestException in handler
    /// true = transazione valida, procedere con il flusso
    /// </summary>
    bool Successo,

    /// <summary>
    /// true = beneficiario obbligatorio per questa operazione
    /// Mostra i bottoni per l'inserimento/selezione del beneficiario in UI
    /// </summary>
    bool BenefondoRichiesto,

    /// <summary>
    /// Messaggio di errore da visualizzare all'utente (es. in pnlOsservazioni)
    /// null se Successo = true
    /// Contiene descrizione della violazione di regole di vigilanza, limiti, etc.
    /// </summary>
    string? MessaggioErrore
);
