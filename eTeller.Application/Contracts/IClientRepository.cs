using eTeller.Domain.Models;
using static eTeller.Application.Contracts.Commons.IBaseSimpleRepository;

namespace eTeller.Application.Contracts
{
    public interface IClientRepository : IBaseSimpleRepository<Client>
    {
        Task<Client?> WhoIsLogged(string ip);
    }
}
