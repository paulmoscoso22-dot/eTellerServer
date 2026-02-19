using eTeller.Domain.Models;
using static eTeller.Application.Contracts.Commons.IBaseSimpleRepository;

namespace eTeller.Application.Contracts.StoreProcedures
{
    public interface ITransactionSpRepository : IBaseSimpleRepository<Transaction>
    {
        Task<IEnumerable<Transaction>> GetTransactionWaitingForBef(string trxCassa, DateTime trxDataDal, DateTime trxDataAl, int trxStatus, string trxBraId);

        Task<IEnumerable<Transaction>> GetTransactionWithFiltersForGiornale(string trxCassa, DateTime trxDataDal, DateTime trxDataAl, int trxStatus, string trxBraId);

        Task<IEnumerable<Transaction>> GetTransactionWithFilters(string trxCassa, DateTime trxDataDal, DateTime trxDataAl, int trxStatus, string trxBraId);
    }
}
