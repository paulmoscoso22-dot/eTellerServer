using eTeller.Application.Contracts.StoreProcedures;
using eTeller.Domain.Models;
using eTeller.Infrastructure.Context;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Dapper;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace eTeller.Infrastructure.Repositories.StoreProcedures
{
    public class TransactionSpRepository : BaseSimpleRepository<Transaction>, ITransactionSpRepository
    {
        static TransactionSpRepository()
        {
            // Configura Dapper per il mapping usando gli attributi [Column()]
            SqlMapper.SetTypeMap(typeof(Transaction), new CustomPropertyTypeMap(
                typeof(Transaction),
                (type, columnName) =>
                {
                    var properties = type.GetProperties();
                    
                    // Primo: cerca mappando gli attributi [Column()] al nome della colonna SQL
                    var property = properties.FirstOrDefault(p =>
                    {
                        var columnAttr = p.GetCustomAttribute<ColumnAttribute>();
                        return columnAttr?.Name?.Equals(columnName, StringComparison.OrdinalIgnoreCase) == true;
                    });
                    
                    // Se non trovato per attributo, prova il match case-insensitive sul nome della proprietà
                    if (property == null)
                    {
                        property = properties.FirstOrDefault(p => 
                            p.Name.Equals(columnName, StringComparison.OrdinalIgnoreCase));
                    }
                    
                    return property;
                }));
        }

        public TransactionSpRepository(eTellerDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<IEnumerable<Transaction>> GetTransactionWaitingForBef(string trxCassa, DateTime trxDataDal, DateTime trxDataAl, int trxStatus, string trxBraId)
        {
            return await ExecuteStoredProcedure("dbo.sp_Transaction_SelectWaitingForBEF", 
                trxCassa, trxDataDal, trxDataAl, trxStatus, trxBraId);
        }

        public async Task<IEnumerable<Transaction>> GetTransactionWithFiltersForGiornale(string trxCassa, DateTime trxDataDal, DateTime trxDataAl, int trxStatus, string trxBraId)
        {
            return await ExecuteStoredProcedure("dbo.sp_Transaction_SelectWithFiltersForGiornale", 
                trxCassa, trxDataDal, trxDataAl, trxStatus, trxBraId);
        }

        public async Task<IEnumerable<Transaction>> GetTransactionWithFilters(string trxCassa, DateTime trxDataDal, DateTime trxDataAl, int trxStatus, string trxBraId)
        {
            return await ExecuteStoredProcedure("dbo.sp_Transaction_SelectWaitingForBEF", 
                trxCassa, trxDataDal, trxDataAl, trxStatus, trxBraId);
        }

        private async Task<IEnumerable<Transaction>> ExecuteStoredProcedure(string procedureName, 
            string trxCassa, DateTime trxDataDal, DateTime trxDataAl, int trxStatus, string trxBraId)
        {
            using (var connection = new SqlConnection(_context.Database.GetConnectionString()))
            {
                await connection.OpenAsync();

                var parameters = new
                {
                    TRX_CASSA = trxCassa,
                    TRX_DATADAL = trxDataDal,
                    TRX_DATAAL = trxDataAl,
                    TRX_STATUS = trxStatus,
                    TRX_BRA_ID = trxBraId
                };

                var result = await connection.QueryAsync<Transaction>(
                    procedureName,
                    parameters,
                    commandType: System.Data.CommandType.StoredProcedure);

                return result;
            }
        }
    }
}

