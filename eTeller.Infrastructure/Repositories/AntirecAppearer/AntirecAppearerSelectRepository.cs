using eTeller.Application.Contracts.AntirecAppearer;
using eTeller.Domain.Models.View;
using eTeller.Infrastructure.Context;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace eTeller.Infrastructure.Repositories.StoreProcedures.AntirecAppearer
{
    public class AntirecAppearerSelectSpRepository : BaseSimpleRepository<AntirecAppearerView>, IAntirecAppearerSelectRepository
    {
        public AntirecAppearerSelectSpRepository(eTellerDbContext dbContext) : base(dbContext)
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
