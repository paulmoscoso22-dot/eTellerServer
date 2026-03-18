using eTeller.Application.Contracts.StoreProcedures;
using eTeller.Domain.Models;
using eTeller.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace eTeller.Infrastructure.Repositories.StoreProcedures
{
    public class CustomersSpRepository : BaseSimpleRepository<Customers>, ICustomersSpRepository
    {
        public CustomersSpRepository(eTellerDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<IEnumerable<Customers>> GetCustomersByCriteriaAsync(string? cliId, string? descrizione)
        {
            string?[] descParts = new string?[6];

            if (!string.IsNullOrEmpty(descrizione))
            {
                descrizione = descrizione.Trim();
                string[] split = descrizione.Split(' ');

                for (int i = 0; i < split.Length && i < 6; i++)
                {
                    descParts[i] = split[i];
                }
            }

            if (string.IsNullOrEmpty(cliId))
                cliId = null;

            return await _context.Customers
                .FromSqlInterpolated($@"
                    EXEC dbo.sp_Customers_SelectByResearch
                        @CliID = {cliId},
                        @Desc1 = {descParts[0]},
                        @Desc2 = {descParts[1]},
                        @Desc3 = {descParts[2]},
                        @Desc4 = {descParts[3]},
                        @Desc5 = {descParts[4]},
                        @Desc6 = {descParts[5]}
                ")
                .AsNoTracking()
                .ToListAsync();
        }
    }
}
