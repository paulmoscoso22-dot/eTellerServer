using eTeller.Application.Contracts.Operazioni.ContoCorrenti.Prelievo;
using eTeller.Domain.Models;
using eTeller.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace eTeller.Infrastructure.Repositories.StoreProcedures.Operazioni.ContoCorrenti.Prelievo;

/// <summary>
/// Implementazione di <see cref="IErrorCodeRepository"/> tramite EF Core.
/// Esegue la stored procedure [dbo].[st_ERRORCODE_SelectByID] e mantiene
/// una cache in memoria per evitare chiamate ripetute al DB per gli stessi codici,
/// dato che i codici errore sono dati statici che non cambiano a runtime.
/// </summary>
public sealed class ErrorCodeRepository : IErrorCodeRepository
{
    private const string CacheKeyPrefix    = "ErrorCode_";
    private const int    CacheExpiryHours  = 24;

    private readonly eTellerDbContext      _context;
    private readonly IMemoryCache          _cache;
    private readonly ILogger<ErrorCodeRepository> _logger;

    public ErrorCodeRepository(
        eTellerDbContext context,
        IMemoryCache cache,
        ILogger<ErrorCodeRepository> logger)
    {
        _context = context;
        _cache   = cache;
        _logger  = logger;
    }

    /// <inheritdoc/>
    public string GetDescription(string errorCode, string lingua = "IT")
    {
        // Versione sincrona: usa GetAwaiter per non bloccare il thread pool
        // in contesti non-async (es. FluentValidation sync rules)
        var errorEntity = GetFromCacheOrDb(errorCode);
        return errorEntity?.GetDescrizione(lingua) ?? errorCode;
    }

    /// <inheritdoc/>
    public async Task<string> GetDescriptionAsync(
        string errorCode,
        string lingua = "IT",
        CancellationToken cancellationToken = default)
    {
        var errorEntity = await GetFromCacheOrDbAsync(errorCode, cancellationToken);
        return errorEntity?.GetDescrizione(lingua) ?? errorCode;
    }

    /// <inheritdoc/>
    public async Task<ErrorCode?> GetByIdAsync(
        string errorCode,
        CancellationToken cancellationToken = default)
    {
        return await GetFromCacheOrDbAsync(errorCode, cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<bool> ExistsAsync(
        string errorCode,
        CancellationToken cancellationToken = default)
    {
        var result = await GetFromCacheOrDbAsync(errorCode, cancellationToken);
        return result is not null;
    }

    // ─────────────────────────────────────────────────────────────────────────
    // Cache + DB
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Versione sincrona: controlla la cache, se manca esegue la SP in modo sincrono.
    /// Usata solo da GetDescription() per compatibilità con FluentValidation sync.
    /// </summary>
    private ErrorCode? GetFromCacheOrDb(string errorCode)
    {
        var cacheKey = BuildCacheKey(errorCode);

        if (_cache.TryGetValue(cacheKey, out ErrorCode? cached))
            return cached;

        return GetFromCacheOrDbAsync(errorCode, CancellationToken.None)
            .GetAwaiter()
            .GetResult();
    }

    /// <summary>
    /// Controlla la cache in memoria, se assente esegue la stored procedure
    /// e salva il risultato in cache per <see cref="CacheExpiryHours"/> ore.
    /// </summary>
    private async Task<ErrorCode?> GetFromCacheOrDbAsync(
        string errorCode,
        CancellationToken cancellationToken)
    {
        var cacheKey = BuildCacheKey(errorCode);

        if (_cache.TryGetValue(cacheKey, out ErrorCode? cached))
            return cached;

        var result = await EseguiStoredProcedureAsync(errorCode, cancellationToken);

        if (result is not null)
        {
            var cacheOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromHours(CacheExpiryHours));

            _cache.Set(cacheKey, result, cacheOptions);
        }
        else
        {
            _logger.LogWarning(
                "Codice errore '{ErrorCode}' non trovato nella tabella ST_ERRORCODE.",
                errorCode);
        }

        return result;
    }

    /// <summary>
    /// Esegue la stored procedure [dbo].[st_ERRORCODE_SelectByID] tramite EF Core
    /// con FromSqlRaw e restituisce l'entità mappata.
    /// </summary>
    private async Task<ErrorCode?> EseguiStoredProcedureAsync(
        string errorCode,
        CancellationToken cancellationToken)
    {
        try
        {
            var results = await _context.ErrorCodes
                .FromSqlRaw(
                    "EXEC [dbo].[st_ERRORCODE_SelectByID] @ERR_ID = {0}",
                    errorCode)
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            return results.FirstOrDefault();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Errore durante l'esecuzione di st_ERRORCODE_SelectByID per codice '{ErrorCode}'.",
                errorCode);

            // Fallback: restituisce null, GetDescription ritornerà il codice grezzo
            return null;
        }
    }

    private static string BuildCacheKey(string errorCode) =>
        $"{CacheKeyPrefix}{errorCode.ToUpperInvariant()}";
}
