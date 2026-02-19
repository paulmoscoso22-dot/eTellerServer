using eTeller.Application.Contracts.StoreProcedures;
using eTeller.Domain.Models;
using eTeller.Domain.Models.View;
using eTeller.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System.Linq;

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

        public async Task<IEnumerable<Account>> GetAccountByIacId(int iacId)
        {
            return await _context.Account
                .FromSqlInterpolated($@"EXEC dbo.sp_Account_SelectByIAC_ID @IAC_ID = {iacId}")
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IEnumerable<Account>> GetAccountByPara(string iacAccId, string iacCutId, string iacCurId, string iacDes, string iacActId, string iacCliCassa, string iacBraId)
        {
            return await _context.Account
                .FromSqlInterpolated($@"
                    EXEC dbo.sp_Account_SelectBypara
                        @IAC_ACC_ID = {iacAccId},
                        @IAC_CUTID = {iacCutId},
                        @IAC_CUR_ID = {iacCurId},
                        @IAC_DES = {iacDes},
                        @IAC_ACT_ID = {iacActId},
                        @IAC_CLI_CASSA = {iacCliCassa},
                        @IAC_BRA_ID = {iacBraId}
                ")
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IEnumerable<Account>> GetAccountForBalance(string clientId)
        {
            return await _context.Account
                .FromSqlInterpolated($@"EXEC dbo.sp_Account_SelectForBalance @ClientID = {clientId}")
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<int> GetAccountForCheck(string iacCutId, string iacCurId, string iacActId, string iacCliCassa)
        {
            // 
            var query = _context.Database.SqlQuery<int>($@"EXEC dbo.sp_Account_SelectForCheck
                        @IAC_CUTID = {iacCutId},
                        @IAC_CUR_ID = {iacCurId},
                        @IAC_ACT_ID = {iacActId},
                        @IAC_CLI_CASSA = {iacCliCassa}");

            var list = await query.ToListAsync();
            return list.FirstOrDefault();
        }

        public async Task<int> GetAccountMaxIacId()
        {
            var query = _context.Database.SqlQuery<int>($"EXEC dbo.sp_Account_SelectMaxIAC_ID");
            var list = await query.ToListAsync();
            return list.FirstOrDefault();
        }

        public async Task<int> UpdateAccount(int iacId, string iacAccId, string iacCutId, string iacCurId, string iacDes, string iacActId, string iacCliCassa, string iacBraId, string iacHostprefix)
        {
            // Execute stored procedure using interpolated parameters
            var result = await _context.Database.ExecuteSqlInterpolatedAsync($@"EXEC dbo.sp_Account_Update
                        @IAC_ID = {iacId},
                        @IAC_ACC_ID = {iacAccId},
                        @IAC_CUTID = {iacCutId},
                        @IAC_CUR_ID = {iacCurId},
                        @IAC_DES = {iacDes},
                        @IAC_ACT_ID = {iacActId},
                        @IAC_CLI_CASSA = {iacCliCassa},
                        @IAC_BRA_ID = {iacBraId},
                        @IAC_HOSTPREFIX = {iacHostprefix}");
            return result;
        }
    }
}
