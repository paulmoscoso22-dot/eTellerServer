using eTeller.Application.Contracts.StoreProcedures.Tabella;
using eTeller.Domain.Models;
using eTeller.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace eTeller.Infrastructure.Repositories.StoreProcedures.Tabella
{
    public class TabellaRepository : ITabellaRepository
    {
        private readonly eTellerDbContext _context;

        public TabellaRepository(eTellerDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Na_TabellaServVarchar>> GetTabellaServVarchar(string nomeTabella, string? id, string? desLike)
        {
            var result = await _context.Set<Na_TabellaServVarchar>()
                .FromSqlInterpolated($"EXEC [dbo].[na_TabellaServVarchar_Select] @NOMETABELLA = {nomeTabella}, @ID = {id}, @DES_LIKE = {desLike}")
                .AsNoTracking()
                .ToListAsync();

            return result;
        }

        public async Task<Na_TabellaServVarchar?> GetTabellaServVarcharById(string id, string nomeTabella)
        {
            var result = await _context.Set<Na_TabellaServVarchar>()
                .FromSqlInterpolated($"EXEC [dbo].[na_TabellaServVarchar_SelectByID] @ID = {id}, @NOMETABELLA = {nomeTabella}")
                .AsNoTracking()
                .ToListAsync();

            return result.FirstOrDefault();
        }
    }
}
