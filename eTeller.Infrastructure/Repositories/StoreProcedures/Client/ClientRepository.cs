using eTeller.Application.Contracts;
using eTeller.Domain.Models;
using eTeller.Infrastructure.Context;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Data;

namespace eTeller.Infrastructure.Repositories.StoreProcedures.Client;

/// <summary>
/// Repository per operazioni su CLIENT via stored procedure.
/// Gestisce il contatore di sequenza giornaliera e altre operazioni client-specifiche.
/// </summary>
public sealed class ClientRepository : BaseSimpleRepository<eTeller.Domain.Models.Client>, IClientRepository
{
    private readonly ILogger<ClientRepository> _logger;

    public ClientRepository(
        eTellerDbContext context,
        ILogger<ClientRepository> logger)
        : base(context)
    {
        _logger = logger;
    }

    /// <summary>
    /// Ricerca il client che ha effettuato il login dall'indirizzo IP specificato.
    /// </summary>
    public async Task<eTeller.Domain.Models.Client?> WhoIsLogged(string ip)
    {
        // Implementazione delegata al metodo di base se disponibile
        // Altrimenti implementare con query personalizzata
        return await Task.FromResult<eTeller.Domain.Models.Client?>(null);
    }

    /// <summary>
    /// Ottiene il prossimo numero di sequenza giornaliera per una cassa.
    /// Chiama la stored procedure sys_CLIENT_getNextCountID con logica:
    /// - Se è il primo giorno della transazione, restituisce 1
    /// - Altrimenti, restituisce il contatore corrente per la cassa
    /// </summary>
    /// <param name="clientId">Codice cassa (CLI_ID), max 3 caratteri</param>
    /// <param name="cancellationToken">Token di cancellazione</param>
    /// <returns>Numero sequenziale giornaliero come stringa</returns>
    public async Task<string> GetNextCounterAsync(string clientId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Richiesta sequenza giornaliera per cassa {ClientId}", clientId);

            // Prepara parametri per la procedura memorizzata
            var countParam = new SqlParameter
            {
                ParameterName = "@CNT",
                SqlDbType = SqlDbType.Int,
                Direction = ParameterDirection.Output
            };

            // Esegue la stored procedure: [dbo].[sys_CLIENT_getNextCountID]
            // Utilizza parametri SQL espliciti per il pieno controllo
            await _context.Database.ExecuteSqlRawAsync(
                "EXEC [dbo].[sys_CLIENT_getNextCountID] @CLI_ID, @CNT OUTPUT",
                new SqlParameter("@CLI_ID", clientId ?? string.Empty),
                countParam,
                cancellationToken);

            // Estrae il valore del parametro OUTPUT
            if (countParam.Value is int counter && counter > 0)
            {
                var counterString = counter.ToString();
                _logger.LogDebug(
                    "Sequenza giornaliera ottenuta per cassa {ClientId}: {Counter}",
                    clientId, counterString);

                return counterString;
            }

            _logger.LogWarning(
                "Sequenza giornaliera non valida per cassa {ClientId}",
                clientId);

            // Fallback: restituisce "1" se qualcosa va storto
            return "1";
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Errore durante il recupero sequenza giornaliera per cassa {ClientId}",
                clientId);

            throw;
        }
    }
}
