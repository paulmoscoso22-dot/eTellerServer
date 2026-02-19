using eTeller.Application.Contracts.StoreProcedures;
using eTeller.Domain.Models;
using eTeller.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace eTeller.Infrastructure.Repositories.StoreProcedures
{
    public class TransactionMovSpRepository : BaseSimpleRepository<TransactionMov>, ITransactionMovSpRepository
    {
        public TransactionMovSpRepository(eTellerDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<IEnumerable<TransactionMov>> GetTransactionMovByTrxId(int trxId)
        {
            // Use FromSqlInterpolated to execute stored procedure
            var result = await _context.TransactionMov
                .FromSqlInterpolated($@"
                    EXEC dbo.sp_TransactionMov_SelectByTrxID
                        @TRX_ID = {trxId}
                ")
                .AsNoTracking()
                .ToListAsync();
            return result;
        }
    }
}
