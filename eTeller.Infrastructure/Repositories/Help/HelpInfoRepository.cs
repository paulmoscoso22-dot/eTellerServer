using eTeller.Application.Contracts.Help;
using eTeller.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace eTeller.Infrastructure.Repositories.Help
{
    public class HelpInfoRepository : IHelpInfoRepository
    {
        private readonly eTellerDbContext _context;

        public HelpInfoRepository(eTellerDbContext context)
        {
            _context = context;
        }

        public async Task<string?> GetFichesPrinterAsync(string cliId)
        {
            var device = await _context.ClientDevices
                .Join(
                    _context.Devices,
                    cd => cd.DevId,
                    d => d.DevId,
                    (cd, d) => new { cd.CliId, d.DevType, d.DevName })
                .Where(x => x.CliId == cliId && x.DevType == "PRT-FICHE")
                .AsNoTracking()
                .FirstOrDefaultAsync();

            return device?.DevName;
        }

        public async Task<bool> ExistsTwinSafeAsync(string cliId)
        {
            return await _context.ClientDevices
                .Join(
                    _context.Devices,
                    cd => cd.DevId,
                    d => d.DevId,
                    (cd, d) => new { cd.CliId, d.DevType })
                .Where(x => x.CliId == cliId && x.DevType == "TWINSAFE")
                .AsNoTracking()
                .AnyAsync();
        }
    }
}
