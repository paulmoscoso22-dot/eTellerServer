using Dapper;
using eTeller.Application.Contracts.Corsi;
using eTeller.Domain.Models.StoredProcedure;
using eTeller.Infrastructure.Context;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace eTeller.Infrastructure.Repositories.Corsi
{
    public class CorsiRepository : BaseSimpleRepository<CorsiResult>, ICorsiRepository
    {
        static CorsiRepository()
        {
            SqlMapper.SetTypeMap(typeof(CorsiResult), new CustomPropertyTypeMap(
                typeof(CorsiResult),
                (type, columnName) =>
                {
                    var properties = type.GetProperties();

                    var property = properties.FirstOrDefault(p =>
                    {
                        var columnAttr = p.GetCustomAttribute<ColumnAttribute>();
                        return columnAttr?.Name?.Equals(columnName, StringComparison.OrdinalIgnoreCase) == true;
                    });

                    if (property == null)
                    {
                        property = properties.FirstOrDefault(p =>
                            p.Name.Equals(columnName, StringComparison.OrdinalIgnoreCase));
                    }

                    return property;
                }));
        }

        public CorsiRepository(eTellerDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<IEnumerable<CorsiResult>> GetAllAsync(string? curId, string? curLondes, string? curCutId, DateTime? dateFrom, DateTime? dateTo)
        {
            var londesParam = curLondes != null ? "%" + curLondes + "%" : null;
            var cutIdParam = string.IsNullOrEmpty(curCutId) ? null : curCutId;

            var dateFromParam = dateFrom ?? new DateTime(1900, 1, 1);
            var dateToParam = dateTo ?? new DateTime(2500, 12, 31, 23, 59, 59);

            if (dateToParam.TimeOfDay == TimeSpan.Zero)
                dateToParam = dateToParam.Date.AddHours(23).AddMinutes(59).AddSeconds(59);

            using var connection = new SqlConnection(_context.Database.GetConnectionString());
            await connection.OpenAsync();

            var parameters = new
            {
                CUR_ID = curId,
                CUR_LONDES = londesParam,
                CUR_CUT_ID = cutIdParam,
                DateFrom = dateFromParam,
                DateTo = dateToParam
            };

            var result = (await connection.QueryAsync<CorsiResult>(
                "dbo.na_CURRENCY_CURRENCY_PRICE_Select",
                parameters,
                commandType: System.Data.CommandType.StoredProcedure)).ToList();

            if (!result.Any())
            {
                var fallbackParameters = new
                {
                    CUR_ID = curId,
                    CUR_LONDES = londesParam,
                    CUR_CUT_ID = "DV",
                    DateFrom = dateFromParam,
                    DateTo = dateToParam
                };

                result = (await connection.QueryAsync<CorsiResult>(
                    "dbo.na_CURRENCY_CURRENCY_PRICE_Select",
                    fallbackParameters,
                    commandType: System.Data.CommandType.StoredProcedure)).ToList();
            }

            return result;
        }
    }
}
