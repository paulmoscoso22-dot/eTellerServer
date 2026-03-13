using eTeller.Domain.Models;

namespace eTeller.Application.Contracts;

/// <summary>
/// Servizio per la comunicazione con l'acquirer/host (es. switching ACI).
/// Gestisce l'invio e la ricezione di messaggi ISO 8583 e simili.
/// </summary>
public interface IHostCommunicationService
{
    /// <summary>
    /// Controlla se il sistema è online e può comunicare con l'host.
    /// </summary>
    Task<bool> IsOnlineAsync(CancellationToken ct = default);

    /// <summary>
    /// Invia un messaggio di verifica della transazione all'host.
    /// Equivalente al vecchio oe.Verifica() in ContiCorrenti.aspx.cs.
    /// </summary>
    /// <param name="trxId">ID della transazione</param>
    /// <param name="tipoOperazione">"DEP" (deposito) o "WITH" (prelievo)</param>
    /// <param name="transaction">Dati della transazione principale</param>
    /// <param name="movimenti">Movimenti associati alla transazione</param>
    /// <param name="ct">Token di cancellazione</param>
    /// <returns>Risultato della verifica da parte dell'host</returns>
    Task<HostVerifyResult> SendVerifyAsync(
        int trxId,
        string tipoOperazione,
        Transaction transaction,
        IEnumerable<TransactionMov> movimenti,
        CancellationToken ct = default);
}

/// <summary>
/// Risultato della verifica di una transazione presso l'host.
/// Contiene informazioni sulla comunicazione e sulla risposta dell'host.
/// </summary>
public sealed record HostVerifyResult(
    /// <summary>
    /// true = comunicazione socket completata senza errori
    /// false = errore di comunicazione (connessione, timeout, ecc.)
    /// </summary>
    bool SocketOk,

    /// <summary>
    /// Codice di errore restituito dall'host (se SocketOk = false)
    /// Es: "T9", "T11", "0001", ecc.
    /// </summary>
    string? ErrorCode,

    /// <summary>
    /// true = la risposta dell'host contiene codici di errore/rifiuto (tipo E, S, C)
    /// false = risposta OK
    /// </summary>
    bool HasErrors,

    /// <summary>
    /// Testo dell'errore da visualizzare all'utente
    /// Riempito se HasErrors = true, null altrimenti
    /// </summary>
    string? ErrorText,

    /// <summary>
    /// true = host richiede l'inserimento dell'IBAN beneficiario (tipicamente per prelievi)
    /// false = beneficiario non richiesto
    /// </summary>
    bool HasBenefondo
);
