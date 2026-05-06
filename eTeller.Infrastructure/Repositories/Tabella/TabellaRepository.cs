using eTeller.Application.Contracts.Tabella;
using eTeller.Domain.Models;
using eTeller.Infrastructure.Context;
using eTeller.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace eTeller.Infrastructure.Ripositories.Tabella
{
    public class TabellaRepository : BaseSimpleRepository<Na_TabellaServVarchar>, ITabellaRepository
    {
        public TabellaRepository(eTellerDbContext context) : base(context)
        {
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

        public async Task InsertTabellaServVarchar(string nomeTabella, string id, string des)
        {
            await _context.Database.ExecuteSqlInterpolatedAsync(
                $"EXEC [dbo].[na_TabellaServVarchar_Insert] @NOMETABELLA = {nomeTabella}, @ID = {id}, @DES = {des}");
        }

        public async Task<bool> UpdateTabellaServVarchar(string nomeTabella, string id, string des)
        {
            var rows = await _context.Database.ExecuteSqlInterpolatedAsync(
                $"EXEC [dbo].[na_TabellaServVarchar_Update] @NOMETABELLA = {nomeTabella}, @ID = {id}, @DES = {des}");

            return rows >= 0;
        }
    }
}


