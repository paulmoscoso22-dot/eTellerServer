using eTeller.Domain.Models;
using eTeller.Domain.Models.View;
using static eTeller.Application.Contracts.Commons.IBaseSimpleRepository;

namespace eTeller.Application.Contracts.StoreProcedures
{
    public interface IAccountSpRepository : IBaseSimpleRepository<Account>
    {
        Task<IEnumerable<Account>> GetAccountAsync();

        Task<IEnumerable<Account>> GetAccountByCriteria(string accType, string branch, string cliId, string currency, string currencyType);
    }
}
