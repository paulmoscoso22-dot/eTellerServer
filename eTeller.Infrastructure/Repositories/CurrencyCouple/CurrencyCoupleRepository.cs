using eTeller.Application.Contracts.CurrencyCouple;
using eTeller.Application.Mappings.CurrencyCouple;
using eTeller.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace eTeller.Infrastructure.Repositories.CurrencyCouple
{
    public class CurrencyCoupleRepository : ICurrencyCoupleRepository
    {
        private readonly eTellerDbContext _context;

        public CurrencyCoupleRepository(eTellerDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<CurrencyCoupleVm>> GetAllAsync()
        {
            var list = await _context.Set<Domain.Models.CurrencyCouple>().AsNoTracking().ToListAsync();
            return list.Select(Map);
        }

        public async Task<CurrencyCoupleVm?> GetByKeyAsync(string cur1, string cur2)
        {
            var entity = await _context.Set<Domain.Models.CurrencyCouple>()
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.CucCur1 == cur1 && c.CucCur2 == cur2);
            return entity is null ? null : Map(entity);
        }

        public async Task<IEnumerable<CurrencyDvVm>> GetCurrenciesDVAsync()
        {
            var list = await _context.Set<Domain.Models.Currency>().AsNoTracking().ToListAsync();
            return list.Select(c => new CurrencyDvVm
            {
                CurId     = c.CurId,
                CurCutId  = c.CurCutId,
                CurShodes = c.CurShodes,
                CurLondes = c.CurLondes
            });
        }

        public async Task<CurrencyCoupleVm> InsertAsync(string cur1, string cur2, string? londes, string? shodes, decimal? size, string? excdir)
        {
            var entity = new Domain.Models.CurrencyCouple
            {
                CucCur1   = cur1,
                CucCur2   = cur2,
                CucLondes = londes,
                CucShodes = shodes,
                CucSize   = size,
                CucExcdir = excdir
            };
            _context.Set<Domain.Models.CurrencyCouple>().Add(entity);
            await _context.SaveChangesAsync();
            return Map(entity);
        }

        public async Task<CurrencyCoupleVm> UpdateAsync(string cur1, string cur2, string? londes, string? shodes, decimal? size, string? excdir)
        {
            var entity = await _context.Set<Domain.Models.CurrencyCouple>()
                .FirstOrDefaultAsync(c => c.CucCur1 == cur1 && c.CucCur2 == cur2)
                ?? throw new InvalidOperationException($"CurrencyCouple '{cur1}/{cur2}' non trovata.");

            entity.CucLondes = londes;
            entity.CucShodes = shodes;
            entity.CucSize   = size;
            entity.CucExcdir = excdir;
            await _context.SaveChangesAsync();
            return Map(entity);
        }

        public async Task<bool> DeleteAsync(string cur1, string cur2)
        {
            var entity = await _context.Set<Domain.Models.CurrencyCouple>()
                .FirstOrDefaultAsync(c => c.CucCur1 == cur1 && c.CucCur2 == cur2);
            if (entity is null) return false;
            _context.Set<Domain.Models.CurrencyCouple>().Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }

        private static CurrencyCoupleVm Map(Domain.Models.CurrencyCouple e) => new()
        {
            CucCur1   = e.CucCur1,
            CucCur2   = e.CucCur2,
            CucLondes = e.CucLondes,
            CucShodes = e.CucShodes,
            CucSize   = e.CucSize,
            CucExcdir = e.CucExcdir
        };
    }
}
