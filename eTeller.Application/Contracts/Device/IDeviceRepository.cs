using eTeller.Domain.Models;

namespace eTeller.Application.Contracts.Device
{
    public interface IDeviceRepository
    {
        Task<IEnumerable<sys_DEVICE>> GetDevicesByBraIdNotCliIdAsync(string cliId, string braId, CancellationToken cancellationToken = default);
        Task<IEnumerable<sys_DEVICE>> GetDeviceByCliIdAsync(string cliId, CancellationToken cancellationToken = default);
    }
}
