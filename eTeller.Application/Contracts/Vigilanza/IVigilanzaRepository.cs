using eTeller.Application.DTOs;
using eTeller.Domain.Models;
using eTeller.Domain.Models.StoredProcedure;
using static eTeller.Application.Contracts.Commons.IBaseSimpleRepository;

namespace eTeller.Application.Contracts.Vigilanza;

// ─────────────────────────────────────────────────────────────────────────────
// Value objects per la verifica vigilanza
// ─────────────────────────────────────────────────────────────────────────────

/// <summary>
/// Contesto necessario per determinare se la vigilanza è richiesta.
/// Corrisponde ai parametri passati a ValidateAntiRecyclingRules() nel vecchio codice.
/// </summary>
public sealed record VigilanzaVerificaRequest(
    string TipoOperazione,      // operationType
    string TipoDivisa,          // typeDV
    decimal ImportoControvalore, // impctv
    string NumeroConto,         // tbConto.Text
    string CategoriaCliente,    // acc.categoria
    DateTime DataOperazione,    // TbTRX_DATOPE
    /// <summary>Dati antiriciclaggio già compilati dall'operatore (null se non compilati)</summary>
    DatiAntiRiciclaggio? DatiCompilati
);

/// <summary>Dati antiriciclaggio compilati dall'operatore nella form.</summary>
public sealed record DatiAntiRiciclaggio(
    int     AppearerId,
    int     BeneficiaryId,
    bool    Forced,
    string? Motivation
);

/// <summary>Esito della verifica vigilanza.</summary>
public enum VigilanzaEsito
{
    /// <summary>Operazione non soggetta a vigilanza antiriciclaggio.</summary>
    NonRichiesta = 0,

    /// <summary>Vigilanza richiesta e dati validi — procedere con SalvaAsync.</summary>
    Richiesta = 1,

    /// <summary>Vigilanza richiesta ma dati mancanti/non validi — bloccare (errore 9024).</summary>
    NonValida = 2
}

/// <summary>Risultato della verifica vigilanza.</summary>
public sealed record VigilanzaVerificaResult(
    VigilanzaEsito Esito,
    /// <summary>Popolato solo se Esito = Richiesta, contiene i dati da passare a SalvaAsync.</summary>
    DatiAntiRiciclaggio? DatiValidati = null,
    /// <summary>Messaggio di errore, popolato solo se Esito = NonValida.</summary>
    string? ErrorMessage = null
)
{
    public bool IsRichiesta   => Esito == VigilanzaEsito.Richiesta;
    public bool IsNonRichiesta => Esito == VigilanzaEsito.NonRichiesta;
    public bool IsNonValida   => Esito == VigilanzaEsito.NonValida;

    // Factory methods
    public static VigilanzaVerificaResult NonRichiesta() =>
        new(VigilanzaEsito.NonRichiesta);

    public static VigilanzaVerificaResult Valida(DatiAntiRiciclaggio dati) =>
        new(VigilanzaEsito.Richiesta, DatiValidati: dati);

    public static VigilanzaVerificaResult NonValida(string errorMessage) =>
        new(VigilanzaEsito.NonValida, ErrorMessage: errorMessage);
}

public interface IVigilanzaRepository : IBaseSimpleRepository<SpTransactionGiornaleAntiriciclagio>
    {
        Task<List<SpTransactionGiornaleAntiriciclagio>> GetTransactionsForGiornaleAntiriciclaggio(
            string trxCassa,
            string trxLocalita,
            DateTime trxDataDal,
            DateTime trxDataAl,
            bool? trxReverse,
            string trxCutId,
            string trxOptId,
            string trxDivope,
            decimal? trxImpopeDA,
            decimal? trxImpopeA,
            string arcAppName,
            bool? arcForced);

        Task<List<SpAntirecRules>> GetAntirecRulesByParameters(
            string? arlOpTypeId,
            string? arlCurTypeId,
            string? arlAcctId,
            string? arlAcctType);

        Task<List<AppearerAll>> GetAppearerByParameters(
            string? nome1,
            string? nome2,
            string? nome3,
            string? nome4,
            DateTime? araBirthdate,
            bool? araRecComplete,
            DateTime? minRecdate);

        Task<List<AppearerAll>> GetAppearerByParametersWithExpiry(
            string araName,
            string? araBirthdate,
            bool araRecComplete,
            bool showExpiredRecords,
            int recordValidityDays = 365);

        Task<AppearerAll?> GetAppearerAllByAraId(int araId);

        Task<int> InsertHisAntirecAppearer(
            DateTime hisDate,
            int araId,
            DateTime araRecdate,
            string araName,
            DateTime? araBirthdate,
            string? araBirthplace,
            string? araNationality,
            string? araIddocnum,
            DateTime? araDocexpdate,
            bool araRecComplete,
            string? araRepresents,
            string? araAddress);

        Task<int> UpdateAntirecAppearer(
            int araId,
            DateTime araRecdate,
            string araName,
            DateTime? araBirthdate,
            string? araBirthplace,
            string? araNationality,
            string? araIddocnum,
            DateTime? araDocexpdate,
            string? araRepresents,
            string? araAddress,
            bool araRecComplete,
            bool araIsupdated);

        Task<int> InsertARA(
            string traUser,
            string traStation,
            DateTime araRecdate,
            string araName,
            string? araBirthdate,
            string? araBirthplace,
            string? araIddocnum,
            string? araNationality,
            string? araDocexpdate,
            string? araRepresents,
            string? araAddress,
            bool araRecComplete);

        Task<int> UpdateARA(
            string traUser,
            string traStation,
            int araId,
            string araName,
            string? araBirthdate,
            string? araBirthplace,
            string? araNationality,
            string? araIddocnum,
            string? araDocexpdate,
            string? araRepresents,
            string? araAddress,
            bool araRecComplete);

        Task<bool> DeleteARA(
            string traUser,
            string traStation,
            int araId);

        // ── READ ──────────────────────────────────────────────────────────────────

        /// <summary>
        /// Recupera il record antiriciclaggio associato a una transazione.
        /// SP: ANTIRECYCLING_ByTrxId_Select @ARC_TRX_ID
        /// Restituisce null se non esiste alcun record.
        /// </summary>
        Task<Antirecycling?> GetByTrxIdAsync(
            int trxId,
            CancellationToken cancellationToken = default);

        // ── WRITE ─────────────────────────────────────────────────────────────────

        /// <summary>
        /// Inserisce un nuovo record antiriciclaggio e restituisce l'ARC_ID generato.
        /// SP: ANTIRECYCLING_Insert — con parametro OUTPUT @ARC_ID (SCOPE_IDENTITY).
        /// Chiamare all'interno della UnitOfWork transaction atomica.
        /// </summary>
        Task<SalvaAntiRecyclingResult> SalvaAsync(
            int trxId,
            int appearerId,
            int beneficiaryId,
            bool forced,
            string? motivation,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Elimina tutti i record antiriciclaggio di una transazione.
        /// SP: ANTIRECYCLING_Delete_ByTrxId @ARC_TRX_ID
        /// Usato quando saveAntiRecycling = false per pulire dati precedenti.
        /// </summary>
        Task DeleteByTrxIdAsync(
            int trxId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Inserisce un record nello storico soggetti antiriciclaggio.
        /// SP: HIS_ANTIREC_SUBJECTS_Insert @HIS_ANS_ID, @ANS_ID
        /// Copia da ANTIREC_SUBJECTS → HIS_ANTIREC_SUBJECTS per storico immodificabile.
        /// Chiamare dopo SalvaAsync per comparente (e beneficiario se BeneficiaryId > 0).
        /// </summary>
        Task SalvaHistoryAsync(
            SalvaHistoryAntiRecyclingRequest request,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Salva i dati antiriciclaggio per una transazione.
        ///
        /// Incapsula l'intera logica di wucAntiRecycling.saveAntiRecycling()
        /// del vecchio ContiCorrenti.aspx.cs — FASE 12.
        ///
        /// Risolve l'AppearerId dal nome e documento forniti, quindi:
        /// 1. Inserisce record in ANTIRECYCLING tramite ANTIRECYCLING_Insert
        /// 2. Registra lo storico comparente in HIS_ANTIREC_SUBJECTS
        ///
        /// Corrisponde a:
        ///   wucAntiRecycling.saveAntiRecycling(userId, cassa, trxId,
        ///                                       nomeComparente, docTipo, docNumero)
        /// </summary>
        Task<SalvaAntiRecyclingResult> SalvaAntiRecyclingAsync(
            string userId,
            string codiceCassa,
            int    trxId,
            string nomeComparente,
            string documentoTipo,
            string documentoNumero,
            CancellationToken cancellationToken = default);

        // ── VALIDAZIONE ───────────────────────────────────────────────────────────

        /// <summary>
        /// Verifica se la vigilanza antiriciclaggio è richiesta e valida per l'operazione.
        ///
        /// Replica i due step del vecchio ContiCorrenti.aspx.cs — FASE 10:
        ///
        ///   STEP 1 — ValidateAntiRecyclingRules(operationType, typeDV, impctv,
        ///                                        conto, categoria, data)
        ///     Valuta le regole configurate nel DB per determinare se la vigilanza
        ///     è obbligatoria (required) o sempre forzata (forced) per questa
        ///     combinazione di tipoOperazione / importo / categoria / data.
        ///
        ///   STEP 2 — wucAntiRecycling.IsValid()
        ///     Se required o forced: verifica che i dati dell'operatore siano
        ///     presenti e validi (minimo: nomeComparente non vuoto).
        ///
        /// Valore di ritorno:
        ///   true  = procedi    (vigilanza non richiesta  OPPURE  richiesta e valida)
        ///   false = BLOCCA     (vigilanza richiesta ma dati mancanti → errore 9024)
        ///
        /// Parametri corrispondenti al vecchio codice:
        ///   tipoOperazione    = operationType          ("DEP" / "WITH")
        ///   tipoDivisa        = typeDV                 (tipo documento vigilanza, es. "DV")
        ///   importoControvalore = impctv               (controvalore CHF)
        ///   numeroConto       = tbConto.Text
        ///   categoriaCliente  = acc.categoria
        ///   dataOperazione    = TbTRX_DATOPE.Text      (formato "dd.MM.yyyy")
        ///   nomeComparente    = wucAntiRecycling campo NomeComparente dell'operatore
        /// </summary>
        Task<bool> IsVigilanzaValidaAsync(
            string   tipoOperazione,
            string   tipoDivisa,
            decimal  importoControvalore,
            string   numeroConto,
            string   categoriaCliente,
            string   dataOperazione,
            string?  nomeComparente,
            CancellationToken cancellationToken = default);
}
