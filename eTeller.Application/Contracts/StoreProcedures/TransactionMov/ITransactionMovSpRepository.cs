using eTeller.Domain.Models;
using static eTeller.Application.Contracts.Commons.IBaseSimpleRepository;

namespace eTeller.Application.Contracts.StoreProcedures
{
    public interface ITransactionMovSpRepository : IBaseSimpleRepository<TransactionMov>
    {
        Task<IEnumerable<TransactionMov>> GetTransactionMovByTrxId(int trxId);
    }
}
