using eTeller.Domain.Models;
using static eTeller.Application.Contracts.Commons.IBaseSimpleRepository;

namespace eTeller.Application.Contracts.StoreProcedures
{
    public interface ITransactionSpRepository : IBaseSimpleRepository<Transaction>
    {
        Task<IEnumerable<Transaction>> GetSpTransactionWaitingForBef(string trxCassa, DateTime trxDataDal, DateTime trxDataAl, int trxStatus, string trxBraId);

        Task<IEnumerable<Transaction>> GetSpTransactionWithFiltersForGiornale(string trxCassa, DateTime trxDataDal, DateTime trxDataAl, int trxStatus, string trxBraId);

        Task<IEnumerable<Transaction>> GetSpTransactionWithFilters(string trxCassa, DateTime trxDataDal, DateTime trxDataAl, int trxStatus, string trxBraId);
    }
}
