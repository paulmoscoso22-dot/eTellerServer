using eTeller.Application.Contracts.StoreProcedures.Vigilanza;
using eTeller.Domain.Models.StoredProcedure;
using eTeller.Infrastructure.Context;
using eTeller.Infrastructure.Repositories;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Dapper;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace eTeller.Infrastructure.Repositories.StoreProcedures.Vigilanza
{
    public class VigilanzaSpRepository : BaseSimpleRepository<SpTransactionGiornaleAntiriciclagio>, IVigilanzaSpRepository
    {
        static VigilanzaSpRepository()
        {
            // Configura Dapper per il mapping usando gli attributi [Column()]
            SqlMapper.SetTypeMap(typeof(SpTransactionGiornaleAntiriciclagio), new CustomPropertyTypeMap(
                typeof(SpTransactionGiornaleAntiriciclagio),
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
                    
                    return property!;
                }));
        }

        public VigilanzaSpRepository(eTellerDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<List<SpTransactionGiornaleAntiriciclagio>> GetTransactionsForGiornaleAntiriciclaggio(
            string trxCassa,
            string trxLocalita,
            DateTime trxDataDal,
            DateTime trxDataAl,
            bool? trxReverse,
            string trxCutId,
            string trxOptId,
            string trxDivope,
            decimal? trxImpopeDA,
            decimal? trxImpopeA,
            string arcAppName,
            bool? arcForced)
        {
            using (var connection = new SqlConnection(_context.Database.GetConnectionString()))
            {
                await connection.OpenAsync();

                var parameters = new
                {
                    TRX_CASSA = trxCassa,
                    TRX_LOCALITA = trxLocalita,
                    TRX_DATADAL = trxDataDal,
                    TRX_DATAAL = trxDataAl,
                    TRX_REVERSE = trxReverse,
                    TRX_CUT_ID = trxCutId,
                    TRX_OPTID = trxOptId,
                    TRX_DIVOPE = trxDivope,
                    TRX_IMPOPE_DA = trxImpopeDA,
                    TRX_IMPOPE_A = trxImpopeA,
                    ARC_APP_NAME = arcAppName,
                    ARC_FORCED = arcForced
                };

                var result = await connection.QueryAsync<SpTransactionGiornaleAntiriciclagio>(
                    "dbo.sp_Transaction_SelectWithFiltersForGiornaleAntiriciclaggio",
                    parameters,
                    commandType: System.Data.CommandType.StoredProcedure);

                return result.ToList();
            }
        }
    }
}
