using eTeller.Application.Contracts.StoreProcedures.ST_CurrencyType;
using eTeller.Domain.Models;
using eTeller.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace eTeller.Infrastructure.Repositories.StoreProcedures.ST_CurrencyType
{
    public class ST_CurrencyTypeSpRepository : BaseSimpleRepository<Domain.Models.ST_CurrencyType>, IST_CurrencyTypeSpRepository
    {
        public ST_CurrencyTypeSpRepository(eTellerDbContext dbContext) : base(dbContext)
        {

        }

        public async Task<Domain.Models.ST_CurrencyType> GetByCutID(string CutID)
        {
            var result = await _context.Set<Domain.Models.ST_CurrencyType>()
                .FromSqlInterpolated($"EXEC [dbo].[st_CurrencyTypeByCutID_Select] @Cut_ID = {CutID}")
                .AsNoTracking()
                .ToListAsync();

            return result.FirstOrDefault();
        }
    }
}
