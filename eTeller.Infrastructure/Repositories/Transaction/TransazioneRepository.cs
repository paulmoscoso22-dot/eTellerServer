using eTeller.Application.Contracts;

using eTeller.Domain.Models;
using eTeller.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace eTeller.Infrastructure.Repositories.Transaction;

/// <summary>
/// Repository per la gestione delle transazioni di prelievo/versamento.
/// Condivide la stessa istanza di DbContext e quindi la stessa transazione DB.
/// </summary>
public class TransazioneRepository : BaseSimpleRepository<Domain.Models.Transaction>, ITransazioneRepository
{
    public TransazioneRepository(eTellerDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<Domain.Models.Transaction?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        return await _context.Transaction.FirstOrDefaultAsync(t => t.TrxId == id, ct);
    }

    public async Task AddAsync(Domain.Models.Transaction transazione, CancellationToken ct = default)
    {
        await _context.Transaction.AddAsync(transazione, ct);
    }

    public Task UpdateAsync(Domain.Models.Transaction transazione, CancellationToken ct = default)
    {
        _context.Transaction.Update(transazione);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Domain.Models.Transaction transazione, CancellationToken ct = default)
    {
        _context.Transaction.Remove(transazione);
        return Task.CompletedTask;
    }

    public async Task<List<Domain.Models.Transaction>> GetByCassaAndDateAsync(string cassaId, DateTime dataOperazione, CancellationToken ct = default)
    {
        return await _context.Transaction
            .Where(t => t.TrxCassa == cassaId && t.TrxDatope == dataOperazione)
            .ToListAsync(ct);
    }

    public Task<IEnumerable<Domain.Models.Transaction>> GetSpTransactionWithFilters(string trxCassa, DateTime trxDataDal, DateTime trxDataAl, int trxStatus, string trxBraId)
    {
        throw new NotImplementedException();
    }
}