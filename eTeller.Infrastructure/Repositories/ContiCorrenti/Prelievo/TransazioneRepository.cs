using eTeller.Domain.Models;
using eTeller.Infrastructure.Context;
using eTeller.Application.Features.ContiCorrenti.Prelievo;
using Microsoft.EntityFrameworkCore;

namespace eTeller.Infrastructure.Repositories.ContiCorrenti.Prelievo;

/// <summary>
/// Repository per la gestione delle transazioni di prelievo/versamento.
/// Condivide la stessa istanza di DbContext e quindi la stessa transazione DB.
/// </summary>
public class TransazioneRepository : ITransazioneRepository
{
    private readonly eTellerDbContext _context;

    public TransazioneRepository(eTellerDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Recupera una transazione per ID
    /// </summary>
    public async Task<Transaction?> GetByIdAsync(
        int id,
        CancellationToken ct = default)
    {
        return await _context.Transaction
            .FirstOrDefaultAsync(t => t.TrxId == id, ct);
    }

    /// <summary>
    /// Aggiunge una nuova transazione al repositorio
    /// Nota: il salvataggio fisico avviene nel UnitOfWork.Complete()
    /// </summary>
    public async Task AddAsync(
        Transaction transazione,
        CancellationToken ct = default)
    {
        await _context.Transaction.AddAsync(transazione, ct);
        // Nota: il salvataggio fisico avviene nel UnitOfWork.Complete()
    }

    /// <summary>
    /// Aggiorna una transazione esistente
    /// EF Core traccia automaticamente le modifiche agli entity già caricati
    /// Update() è necessario solo per entity detached
    /// </summary>
    public Task UpdateAsync(
        Transaction transazione,
        CancellationToken ct = default)
    {
        // EF Core traccia automaticamente le modifiche agli entity
        // già caricati — Update() è necessario solo per entity detached
        _context.Transaction.Update(transazione);
        return Task.CompletedTask;
    }

    /// <summary>
    /// Elimina una transazione dal repositorio
    /// </summary>
    public Task DeleteAsync(
        Transaction transazione,
        CancellationToken ct = default)
    {
        _context.Transaction.Remove(transazione);
        return Task.CompletedTask;
    }

    /// <summary>
    /// Recupera tutte le transazioni di un cassiere in una data
    /// </summary>
    public async Task<List<Transaction>> GetByCassaAndDateAsync(
        string cassaId,
        DateTime dataOperazione,
        CancellationToken ct = default)
    {
        return await _context.Transaction
            .Where(t => t.TrxCassa == cassaId && t.TrxDatope == dataOperazione)
            .ToListAsync(ct);
    }
}
