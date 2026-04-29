using eTeller.Domain.Models;
using static eTeller.Application.Contracts.Commons.IBaseSimpleRepository;

namespace eTeller.Application.Contracts.ClientDevice
{
    public interface IClientDeviceRepository : IBaseSimpleRepository<Client_Device>
    {
    }
}