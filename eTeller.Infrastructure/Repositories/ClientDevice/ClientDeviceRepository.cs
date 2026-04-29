using eTeller.Application.Contracts.ClientDevice;
using eTeller.Domain.Models;
using eTeller.Infrastructure.Context;

namespace eTeller.Infrastructure.Repositories.ClientDevice
{
    public class ClientDeviceRepository : BaseSimpleRepository<Client_Device>, IClientDeviceRepository
    {
        public ClientDeviceRepository(eTellerDbContext dbContext) : base(dbContext)
        {
        }
    }
}