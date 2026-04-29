using eTeller.Application.Contracts.Device;
using eTeller.Domain.Models;
using eTeller.Infrastructure.Context;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace eTeller.Infrastructure.Repositories.StoreProcedures.Device
{
    public class DeviceRepository : BaseSimpleRepository<sys_DEVICE>, IDeviceRepository
    {
        public DeviceRepository(eTellerDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<IEnumerable<sys_DEVICE>> GetDevicesByBraIdNotCliIdAsync(string cliId, string braId, CancellationToken cancellationToken = default)
        {
            return await _context.Set<sys_DEVICE>()
                .FromSqlInterpolated($@"EXEC dbo.sys_DEVICE_SelectByBRA_IDnotCLI_ID @CLI_ID = {cliId}, @BRA_ID = {braId}")
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<sys_DEVICE>> GetDeviceByCliIdAsync(string cliId, CancellationToken cancellationToken = default)
        {
            return await _context.Set<sys_DEVICE>()
                .FromSqlInterpolated($@"EXEC dbo.sys_DEVICE_SelectByCLI_ID @CLI_ID = {cliId}")
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }
    }
}
