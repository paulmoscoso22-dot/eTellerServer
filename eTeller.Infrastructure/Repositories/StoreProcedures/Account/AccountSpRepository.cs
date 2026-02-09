using eTeller.Application.Contracts.StoreProcedures;
using eTeller.Domain.Models;
using eTeller.Domain.Models.View;
using eTeller.Infrastructure.Context;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace eTeller.Infrastructure.Repositories.StoreProcedures
{
    public class AccountSpRepository : BaseSimpleRepository<Account>, IAccountSpRepository
    {
        public AccountSpRepository(eTellerDbContext dbContext) : base(dbContext)
        {

        }

        public async Task<IEnumerable<Account>> GetAccountAsync()
        {
            return await _context.Account
                        .FromSqlRaw("EXEC dbo.sp_Account_Select")
                        .AsNoTracking()
                        .ToListAsync();
        }

        public async Task<IEnumerable<Account>> GetAccountByCriteria(string accType, string branch, string cliId, string currency, string currencyType)
        {
            return await _context.Account
                    .FromSqlInterpolated($@"
                        EXEC dbo.sp_Account_SelectByCriteria
                            @AccType = {accType},
                            @Branch = {branch},
                            @CliId = {cliId},
                            @Currency = {currency},
                            @CurrencyType = {currencyType}
                    ")
                    .AsNoTracking()
                    .ToListAsync();
        }
    }
}
