using eTeller.Application.Contracts.StoreProcedures.Trace;
using eTeller.Application.Mappings.Trace;
using eTeller.Domain.Models;
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

        public async Task<IEnumerable<eTeller.Domain.Models.Trace>> GetTraceAll(
            string? traUser,
            string? traFunCode,
            string? traStation,
            string? traTabNam,
            string? traEntCode,
            bool? traError,
            DateTime? dataFrom,
            DateTime? dataTo)
        {
            using (var connection = new SqlConnection(_context.Database.GetConnectionString()))
            {
                await connection.OpenAsync();

                var parameters = new DynamicParameters();
                parameters.Add("@TRA_User", traUser);
                parameters.Add("@TRA_FunCode", traFunCode);
                parameters.Add("@TRA_Station", traStation);
                parameters.Add("@TRA_TabNam", traTabNam);
                parameters.Add("@TRA_EntCode", traEntCode);
                parameters.Add("@TRA_Error", traError);
                parameters.Add("@DataFrom", dataFrom);
                parameters.Add("@DataTo", dataTo);

                var results = await connection.QueryAsync<eTeller.Domain.Models.Trace>(
                    "dbo.sp_Trace_SelectByAll",
                    parameters,
                    commandType: System.Data.CommandType.StoredProcedure);

                return results;
            }
        }

        //public async Task<IEnumerable<TraceWithFunctionVm>> GetTraceWithFunctionAsync(
        //    string? traUser,
        //    string? traFunCode,
        //    string? traStation,
        //    string? traTabNam,
        //    string? traEntCode,
        //    bool? traError,
        //    DateTime? dataFrom,
        //    DateTime? dataTo)
        //{
        //    var results = new List<TraceWithFunctionVm>();
        //    var whereClauses = new List<string>();
        //    var parameters = new List<SqlParameter>();

        //    var sql = @"
        //        SELECT 
        //            t.TRA_ID,
        //            t.TRA_Time,
        //            t.TRA_User,
        //            t.TRA_FunCode,
        //            t.TRA_SubFun,
        //            t.TRA_Station,
        //            t.TRA_TabNam,
        //            t.TRA_EntCode,
        //            t.TRA_RevTrxTrace,
        //            t.TRA_Des,
        //            t.TRA_ExtRef,
        //            t.TRA_Error,
        //            f.TFC_DES
        //        FROM TRACE t
        //        INNER JOIN ST_TRACE_FUNCTION f ON t.TRA_FunCode = f.TFC_ID";

        //    if (!string.IsNullOrEmpty(traUser))
        //    {
        //        whereClauses.Add("t.TRA_User = @TraUser");
        //        parameters.Add(new SqlParameter("@TraUser", traUser));
        //    }

        //    if (!string.IsNullOrEmpty(traFunCode))
        //    {
        //        whereClauses.Add("t.TRA_FunCode = @TraFunCode");
        //        parameters.Add(new SqlParameter("@TraFunCode", traFunCode));
        //    }

        //    if (!string.IsNullOrEmpty(traStation))
        //    {
        //        whereClauses.Add("t.TRA_Station = @TraStation");
        //        parameters.Add(new SqlParameter("@TraStation", traStation));
        //    }

        //    if (!string.IsNullOrEmpty(traTabNam))
        //    {
        //        whereClauses.Add("t.TRA_TabNam = @TraTabNam");
        //        parameters.Add(new SqlParameter("@TraTabNam", traTabNam));
        //    }

        //    if (!string.IsNullOrEmpty(traEntCode))
        //    {
        //        whereClauses.Add("t.TRA_EntCode = @TraEntCode");
        //        parameters.Add(new SqlParameter("@TraEntCode", traEntCode));
        //    }

        //    if (traError.HasValue)
        //    {
        //        whereClauses.Add("t.TRA_Error = @TraError");
        //        parameters.Add(new SqlParameter("@TraError", traError.Value));
        //    }

        //    if (dataFrom.HasValue)
        //    {
        //        whereClauses.Add("t.TRA_Time >= @DataFrom");
        //        parameters.Add(new SqlParameter("@DataFrom", dataFrom.Value));
        //    }

        //    if (dataTo.HasValue)
        //    {
        //        whereClauses.Add("t.TRA_Time <= @DataTo");
        //        parameters.Add(new SqlParameter("@DataTo", dataTo.Value));
        //    }

        //    if (whereClauses.Count > 0)
        //    {
        //        sql += " WHERE " + string.Join(" AND ", whereClauses);
        //    }

        //    using var connection = new SqlConnection(_context.Database.GetConnectionString());
        //    await connection.OpenAsync();

        //    using var command = new SqlCommand(sql, connection);
        //    command.Parameters.AddRange(parameters.ToArray());

        //    using var reader = await command.ExecuteReaderAsync();

        //    while (await reader.ReadAsync())
        //    {
        //        results.Add(new TraceWithFunctionVm
        //        {
        //            TraId = reader.GetInt32(0),
        //            TraTime = reader.GetDateTime(1),
        //            TraUser = reader.GetString(2),
        //            TraFunCode = reader.GetString(3),
        //            TraSubFun = reader.IsDBNull(4) ? null : reader.GetString(4),
        //            TraStation = reader.GetString(5),
        //            TraTabNam = reader.GetString(6),
        //            TraEntCode = reader.GetString(7),
        //            TraRevTrxTrace = reader.IsDBNull(8) ? null : reader.GetString(8),
        //            TraDes = reader.IsDBNull(9) ? null : reader.GetString(9),
        //            TraExtRef = reader.IsDBNull(10) ? null : reader.GetString(10),
        //            TraError = reader.GetBoolean(11),
        //            TfcDes = reader.GetString(12)
        //        });
        //    }

        //    return results;
        //}
    }
}
