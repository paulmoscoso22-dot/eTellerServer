using eTeller.Domain.Models;
using static eTeller.Application.Contracts.Commons.IBaseSimpleRepository;

namespace eTeller.Application.Contracts.StoreProcedures
{
    public interface ICustomerAccountSpRepository : IBaseSimpleRepository<CustomerAccount>
    {
        Task<IEnumerable<CustomerAccount>> GetCustomerAccountsByCliIdAsync(string cliId);
    }
}
