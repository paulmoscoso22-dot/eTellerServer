using eTeller.Domain.Models;
using static eTeller.Application.Contracts.Commons.IBaseSimpleRepository;

namespace eTeller.Application.Contracts
{
    public interface ICustomerAccountRepository : IBaseSimpleRepository<CustomerAccount>
    {
        Task<IEnumerable<CustomerAccount>> GetCustomerAccountsByCliIdAsync(string cliId);
    }
}
