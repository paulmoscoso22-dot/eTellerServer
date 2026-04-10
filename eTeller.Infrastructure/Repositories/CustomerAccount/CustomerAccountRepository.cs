using eTeller.Application.Contracts;
using eTeller.Domain.Models;
using eTeller.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using CustomerAccountModel = eTeller.Domain.Models.CustomerAccount;

namespace eTeller.Infrastructure.Repositories.StoreProcedures.CustomerAccount
{
    public class CustomerAccountRepository : BaseSimpleRepository<CustomerAccountModel>, ICustomerAccountRepository
    {
        public CustomerAccountRepository(eTellerDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<IEnumerable<CustomerAccountModel>> GetCustomerAccountsByCliIdAsync(string cliId)
        {
            return await _context.CustomerAccount
                .FromSqlInterpolated($@"
                    EXEC dbo.sp_CustomerAccounts_SelectByCliID
                        @CliID = {cliId}
                ")
                .AsNoTracking()
                .ToListAsync();
        }
    }
}
