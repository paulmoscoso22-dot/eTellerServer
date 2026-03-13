using System.Data;
using Dapper;
using eTeller.Application.Contracts.StoreProcedures;
using eTeller.Domain.Models;
using eTeller.Infrastructure.Context;
using Microsoft.Data.SqlClient;
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

        public async Task InsertMovimentoAsync(
        int       transactionId,
        string    tipoMovimento,
        string    filiale,
        string    tipoAccount,
        string    account,
        string    divisa,
        decimal   importo,
        decimal   importoCtv,
        DateTime  dataValuta,
        string?   text1,
        string?   text2,
        string    codiceCausale,
        string    hostCod,
        bool      updatePosition,
        CancellationToken ct = default)
        {
            // Vecchio: eTellerDAL.TransactionMovDB.InsertT(...)
            // SP:      dbo.sp_TransactionMov_Insert

            var conn = _context.Database.GetDbConnection();
            if (conn.State != ConnectionState.Open)
            {
                await conn.OpenAsync(ct);
            }

            try
            {
                var parameters = new
                {
                    TRM_TRX_ID = transactionId,
                    TRM_MOVTYP = tipoMovimento,
                    TRM_BRA_ID = filiale,
                    TRM_ACCTYPE = tipoAccount,
                    TRM_ACCOUNT = account,
                    TRM_ACCCUR = divisa,
                    TRM_AMOUNT = importo,
                    TRM_AMTCTV = importoCtv,
                    TRM_VALDAT = dataValuta,
                    TRM_DATOPE = DateTime.Now,
                    TRM_REACOD = codiceCausale,
                    TRM_FREETX1 = text1,
                    TRM_FREETX2 = text2,
                    TRM_HOSTCOD = hostCod,
                    TRM_UPDPOS = updatePosition
                };

                await conn.ExecuteAsync(
                    "dbo.sp_TransactionMov_Insert",
                    parameters,
                    commandType: CommandType.StoredProcedure,
                    transaction: null,
                    commandTimeout: null);
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
            }
        }

        public async Task DeleteByTrxIdAsync(int transactionId, CancellationToken ct = default)
        {

            await _context.Database.ExecuteSqlRawAsync(
                "EXEC dbo.sp_TransactionMov_DeleteByTrxID @TRM_TRX_ID = {0}",
                new object[] { transactionId }, ct);
        }

    }
}
