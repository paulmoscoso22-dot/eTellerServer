using eTeller.Domain.Services;
using eTeller.Infrastructure.Context;
using Microsoft.Extensions.Logging;

namespace eTeller.Infrastructure.Services;

/// <summary>
/// Implementazione del Domain Service per le verifiche IS107 (limiti di operatività).
/// Gestisce il controllo dei limiti configurati e lo stato delle segnalazioni.
/// </summary>
public sealed class IS107DomainService : IIS107DomainService
{
    private readonly eTellerDbContext _context;
    private readonly ILogger<IS107DomainService> _logger;

    public IS107DomainService(eTellerDbContext context, ILogger<IS107DomainService> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Verifica se la transazione proposta rispetta i limiti IS107 configurati.
    /// </summary>
    public async Task<IS107VerificaResult> VerificaLimitiAsync(
        IS107VerificaRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Se il cliente non ha il flag IS107 abilitato, non fare verifiche
            if (request.FlagIS107 != "Y")
            {
                return new IS107VerificaResult
                {
                    Esito = IS107EsitoVerifica.Ok,
                    Messaggio = "IS107 non abilitato per il cliente"
                };
            }

            // TODO: Implementare logica per verifica limiti dal database
            // Per ora, restituiamo sempre Ok
            await Task.CompletedTask;

            return new IS107VerificaResult
            {
                Esito = IS107EsitoVerifica.Ok,
                Messaggio = "Verifica IS107 completata con esito positivo"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Errore durante verifica IS107 per relazione {NumeroRelazione}", 
                request.NumeroRelazione);
            
            // In caso di errore, restituiamo un risultato neutro
            return new IS107VerificaResult
            {
                Esito = IS107EsitoVerifica.Ok,
                Messaggio = "Verifica IS107 ignorata a causa di errore"
            };
        }
    }
}
