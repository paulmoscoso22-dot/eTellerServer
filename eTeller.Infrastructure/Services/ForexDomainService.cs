using eTeller.Domain.Services;
using eTeller.Infrastructure.Context;
using Microsoft.Extensions.Logging;

namespace eTeller.Infrastructure.Services;

/// <summary>
/// Implementazione del Domain Service per le operazioni Forex.
/// Fornisce logica di business per calcolo tassi, validazioni e tagli minimi.
/// </summary>
public sealed class ForexDomainService : IForexDomainService
{
    private readonly eTellerDbContext _context;
    private readonly ILogger<ForexDomainService> _logger;

    public ForexDomainService(eTellerDbContext context, ILogger<ForexDomainService> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Verifica che l'importo rispetti il taglio minimo della valuta per i Biglietti Banca.
    /// </summary>
    public async Task<bool> RispettaTaglioMinimoAsync(
        string codiceDivisa,
        decimal importo,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var taglioMinimo = await GetTaglioMinimoAsync(codiceDivisa, cancellationToken);
            return importo >= taglioMinimo;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Errore durante verifica taglio minimo per divisa {Divisa}", codiceDivisa);
            // Per ora, consentiamo l'operazione in caso di errore
            return true;
        }
    }

    /// <summary>
    /// Restituisce il taglio minimo configurato per la divisa specificata.
    /// </summary>
    public async Task<decimal> GetTaglioMinimoAsync(
        string codiceDivisa,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // TODO: Implementare logica per lettura taglio minimo dal database
            // Per ora, restituiamo un valore di default
            await Task.CompletedTask;
            return 10m;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Errore durante recupero taglio minimo per divisa {Divisa}", codiceDivisa);
            return 10m;
        }
    }

    /// <summary>
    /// Calcola il cambio cross tra due divise applicando eventualmente lo spread.
    /// </summary>
    public async Task<CambioCalcolatoDto> CalcolaCambioAsync(
        string divisaConto,
        string divisaBanconote,
        string tipoOperazione,
        bool applicaSpread,
        bool isDipendente,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // TODO: Implementare logica per calcolo cambio dal database
            // Per ora, restituiamo un valore di default
            await Task.CompletedTask;

            return new CambioCalcolatoDto
            {
                Prezzo = 1.05m,
                PrezzoDivisa1 = 1.0m,
                PrezzoDivisa2 = 0.95m,
                ScalaDivisa1 = 1m,
                ScalaDivisa2 = 1m,
                Direzione = true
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Errore durante calcolo cambio tra {Divisa1} e {Divisa2}", 
                divisaConto, divisaBanconote);
            throw;
        }
    }

    /// <summary>
    /// Verifica che il tasso inserito dall'operatore rientri nella tolleranza.
    /// </summary>
    public async Task<bool> IsFuoriTolleranzaAsync(
        string codiceDivisa,
        decimal tassoOperatore,
        decimal tassoDiSistema,
        string tipoOperazione,
        bool forzaCambio,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Se forzato dall'operatore, non è mai fuori tolleranza
            if (forzaCambio)
                return false;

            // TODO: Implementare logica per verifica tolleranza dal database
            // Per ora, consentiamo una tolleranza del 5%
            await Task.CompletedTask;
            
            var differenza = Math.Abs(tassoOperatore - tassoDiSistema) / tassoDiSistema;
            return differenza > 0.05m;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Errore durante verifica tolleranza cambio per divisa {Divisa}", codiceDivisa);
            return false;
        }
    }

    /// <summary>
    /// Arrotonda l'importo al taglio minimo della divisa.
    /// </summary>
    public async Task<decimal> ArrotondaAlTaglioMinimoAsync(
        string codiceDivisa,
        decimal importo,
        bool applicaArrotondamento,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (!applicaArrotondamento)
                return importo;

            var taglioMinimo = await GetTaglioMinimoAsync(codiceDivisa, cancellationToken);
            
            // Arrotonda per eccesso al taglio minimo più vicino
            return Math.Ceiling(importo / taglioMinimo) * taglioMinimo;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Errore durante arrotondamento importo per divisa {Divisa}", codiceDivisa);
            return importo;
        }
    }
}
