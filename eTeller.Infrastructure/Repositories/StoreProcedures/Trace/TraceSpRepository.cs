using eTeller.Application.Contracts.StoreProcedures.Trace;
using eTeller.Infrastructure.Context;
using eTeller.Infrastructure.Repositories;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Dapper;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace eTeller.Infrastructure.Repositories.StoreProcedures.Trace
{
    public class TraceSpRepository : BaseSimpleRepository<eTeller.Domain.Models.Trace>, ITraceSpRepository
    {
        static TraceSpRepository()
        {
            // Configura Dapper per il mapping usando gli attributi [Column()]
            SqlMapper.SetTypeMap(typeof(eTeller.Domain.Models.Trace), new CustomPropertyTypeMap(
                typeof(eTeller.Domain.Models.Trace),
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

        public TraceSpRepository(eTellerDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<int> InsertTrace(
            DateTime traTime,
            string traUser,
            string traFunCode,
            string? traSubFun,
            string traStation,
            string traTabNam,
            string traEntCode,
            string? traRevTrxTrace,
            string? traDes,
            string? traExtRef,
            bool traError)
        {
            using (var connection = new SqlConnection(_context.Database.GetConnectionString()))
            {
                await connection.OpenAsync();

                var parameters = new DynamicParameters();
                parameters.Add("@TRA_Time", traTime);
                parameters.Add("@TRA_User", traUser);
                parameters.Add("@TRA_FunCode", traFunCode);
                parameters.Add("@TRA_SubFun", traSubFun);
                parameters.Add("@TRA_Station", traStation);
                parameters.Add("@TRA_TabNam", traTabNam);
                parameters.Add("@TRA_EntCode", traEntCode);
                parameters.Add("@TRA_RevTrxTrace", traRevTrxTrace);
                parameters.Add("@TRA_Des", traDes);
                parameters.Add("@TRA_ExtRef", traExtRef);
                parameters.Add("@TRA_Error", traError);

                await connection.ExecuteAsync(
                    "dbo.sp_Trace_Insert",
                    parameters,
                    commandType: System.Data.CommandType.StoredProcedure);

                return 1; // Insert successful
            }
        }
    }
}
