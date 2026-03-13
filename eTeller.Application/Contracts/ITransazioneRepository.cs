using eTeller.Domain.Models;

namespace eTeller.Application.Features.ContiCorrenti.Prelievo;

/// <summary>
/// Repository interface per la gestione delle transazioni di prelievo/versamento
/// </summary>
public interface ITransazioneRepository
{
    /// <summary>
    /// Recupera una transazione per ID
    /// </summary>
    Task<Transaction?> GetByIdAsync(int id, CancellationToken ct = default);

    /// <summary>
    /// Aggiunge una nuova transazione
    /// </summary>
    Task AddAsync(Transaction transazione, CancellationToken ct = default);

    /// <summary>
    /// Aggiorna una transazione esistente
    /// </summary>
    Task UpdateAsync(Transaction transazione, CancellationToken ct = default);

    /// <summary>
    /// Elimina una transazione
    /// </summary>
    Task DeleteAsync(Transaction transazione, CancellationToken ct = default);

    /// <summary>
    /// Recupera tutte le transazioni di un cassiere in una data specifica
    /// </summary>
    Task<List<Transaction>> GetByCassaAndDateAsync(string cassaId, DateTime dataOperazione, CancellationToken ct = default);
}
