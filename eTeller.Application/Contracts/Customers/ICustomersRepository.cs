using eTeller.Domain.Models;
using static eTeller.Application.Contracts.Commons.IBaseSimpleRepository;

namespace eTeller.Application.Contracts
{
    public interface ICustomersRepository : IBaseSimpleRepository<Customers>
    {
        Task<IEnumerable<Customers>> GetCustomersByCriteriaAsync(string? cliId, string? descrizione);
    }
}
