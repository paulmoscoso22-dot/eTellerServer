namespace eTeller.Application.DTOs;

// ─────────────────────────────────────────────────────────────────────────────
// DTOs per IVigilanzaRepository
// ─────────────────────────────────────────────────────────────────────────────

/// <summary>
/// Input per <see cref="IVigilanzaRepository.SalvaAntiRecyclingAsync"/>.
///
/// Corrisponde ai parametri passati da wucAntiRecycling.saveAntiRecycling()
/// nel vecchio ContiCorrenti.aspx.cs — FASE 12.
///
/// Il vecchio user control riceveva userId e codiceCassa per recuperare
/// l'AppearerId corrispondente al nome/documento comparente tramite
/// la tabella ANTIREC_SUBJECTS (ricerca per nome + tipo doc + numero doc).
/// </summary>
public sealed record SalvaAntiRecyclingRequest(
    /// <summary>ID dell'operatore che esegue l'operazione (auth.UserId)</summary>
    int     UserId,
    /// <summary>Codice cassa dell'operatore (auth.Cassa)</summary>
    string  CodiceCassa,
    /// <summary>ID della transazione padre (TRX_ID appena creato/aggiornato)</summary>
    int     TrxId,
    /// <summary>Nome e cognome del comparente inserito dall'operatore</summary>
    string  NomeComparente,
    /// <summary>Tipo documento del comparente (es. "CI", "PA", "PT")</summary>
    string  DocumentoTipo,
    /// <summary>Numero documento del comparente</summary>
    string  DocumentoNumero
);

/// <summary>
/// Output di <see cref="IVigilanzaRepository.SalvaAntiRecyclingAsync"/>:
/// contiene l'ARC_ID del record ANTIRECYCLING generato (SCOPE_IDENTITY).
/// </summary>
public sealed record SalvaAntiRecyclingResult(
    int ArcId
);

/// <summary>
/// Input per la SP ANTIRECYCLING_Insert (usato internamente dal repository).
/// Parametri diretti alla SP dopo la risoluzione dell'AppearerId da nome/documento.
/// </summary>
internal sealed record AntiRecyclingInsertRequest(
    int     TrxId,
    int     AppearerId,
    int     BeneficiaryId,
    bool    Forced,
    string? Motivation
);

/// <summary>
/// Input per HIS_ANTIREC_SUBJECTS_Insert.
/// HisAnsId = TrxId (padre storico), AnsId = ID soggetto (comparente o beneficiario).
/// </summary>
public sealed record SalvaHistoryAntiRecyclingRequest(
    int HisAnsId,
    int AnsId
);

/// <summary>
/// DTO interno per leggere i flag delle regole vigilanza.
/// Usato da VigilanzaSpRepository.IsVigilanzaValidaAsync.
/// </summary>
internal sealed class AntiRecyclingRuleDto
{
    /// <summary>
    /// true = vigilanza obbligatoria per questa operazione
    /// (= wucAntiRecycling.required nel vecchio codice)
    /// </summary>
    public bool Required { get; set; }

    /// <summary>
    /// true = vigilanza sempre forzata indipendentemente dall'importo
    /// (= wucAntiRecycling.forced nel vecchio codice)
    /// </summary>
    public bool Forced { get; set; }
}
