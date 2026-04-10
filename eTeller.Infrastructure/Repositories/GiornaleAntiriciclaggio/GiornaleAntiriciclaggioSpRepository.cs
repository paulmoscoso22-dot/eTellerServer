using eTeller.Application.Contracts;
using eTeller.Infrastructure.Context;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Dapper;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace eTeller.Infrastructure.Repositories.GiornaleAntiriciclaggio
{
    public class GiornaleAntiriciclaggioSpRepository : BaseSimpleRepository<Domain.Models.StoredProcedure.GiornaleAntiriciclaggio>, IGiornaleAntiriciclaggioRepository
    {
        static GiornaleAntiriciclaggioSpRepository()
        {
            // Configura Dapper per il mapping usando gli attributi [Column()]
            SqlMapper.SetTypeMap(typeof(Domain.Models.StoredProcedure.GiornaleAntiriciclaggio), new CustomPropertyTypeMap(
                typeof(Domain.Models.StoredProcedure.GiornaleAntiriciclaggio),
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

        public GiornaleAntiriciclaggioSpRepository(eTellerDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<List<Domain.Models.StoredProcedure.GiornaleAntiriciclaggio>> GetSpTransactionWithFiltersForGiornaleAntiriciclaggio(
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
                    TRX_CASSA = string.IsNullOrEmpty(trxCassa) ? null : trxCassa,
                    TRX_LOCALITA = string.IsNullOrEmpty(trxLocalita) ? null : trxLocalita,
                    TRX_DATADAL = trxDataDal,
                    TRX_DATAAL = trxDataAl,
                    TRX_REVERSE = trxReverse,
                    TRX_CUT_ID = string.IsNullOrEmpty(trxCutId) ? null : trxCutId,
                    TRX_OPTID = string.IsNullOrEmpty(trxOptId) ? null : trxOptId,
                    TRX_DIVOPE = string.IsNullOrEmpty(trxDivope) ? null : trxDivope,
                    TRX_IMPOPE_DA = trxImpopeDA ?? 0,
                    TRX_IMPOPE_A = trxImpopeA ?? 999999999,
                    ARC_APP_NAME = string.IsNullOrEmpty(arcAppName) ? null : arcAppName,
                    ARC_FORCED = arcForced
                };

                var result = await connection.QueryAsync<Domain.Models.StoredProcedure.GiornaleAntiriciclaggio>(
                    "dbo.sp_Transaction_SelectWithFiltersForGiornaleAntiriciclaggio",
                    parameters,
                    commandType: System.Data.CommandType.StoredProcedure);

                return result.ToList();
            }
        }

    }
}
