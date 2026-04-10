using eTeller.Domain.Models;
using static eTeller.Application.Contracts.Commons.IBaseSimpleRepository;

namespace eTeller.Application.Contracts
{
    public interface ITransazioneRepository : IBaseSimpleRepository<Transaction>
    {
        Task<IEnumerable<Transaction>> GetSpTransactionWithFilters(string trxCassa, DateTime trxDataDal, DateTime trxDataAl, int trxStatus, string trxBraId);

        Task<Transaction?> GetByIdAsync(int id, CancellationToken ct = default);
        Task AddAsync(Transaction transazione, CancellationToken ct = default);
        Task UpdateAsync(Transaction transazione, CancellationToken ct = default);
        Task DeleteAsync(Transaction transazione, CancellationToken ct = default);
        Task<List<Transaction>> GetByCassaAndDateAsync(string cassaId, DateTime dataOperazione, CancellationToken ct = default);
    }
}