using eTeller.Application.Contracts.StoreProcedures.AntirecAppearer;
using eTeller.Domain.Models.View;
using eTeller.Infrastructure.Context;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace eTeller.Infrastructure.Repositories.StoreProcedures.AntirecAppearer
{
    public class AntirecAppearerSelectRepository : BaseSimpleRepository<AntirecAppearerView>, IAntirecAppearerSelectRepository
    {
        public AntirecAppearerSelectRepository(eTellerDbContext dbContext) : base(dbContext)
        {
        }
        public async Task<IEnumerable<AntirecAppearerView>> GetAntirecAppearerByAreaIdAsync(int araId)
        {

            return await _context.AntirecAppearerView
                        .FromSqlRaw(
                            "EXEC dbo.ANTIREC_APPEARER_ByAraId_Select @param",
                            new SqlParameter("@param", araId)
                        )
                        .AsNoTracking()
                        .ToListAsync();
        }
    }
}
