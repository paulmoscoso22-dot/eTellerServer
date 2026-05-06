using Dapper;
using eTeller.Application.Contracts.ForceTrx;
using eTeller.Domain.Models.StoredProcedure;
using eTeller.Infrastructure.Context;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace eTeller.Infrastructure.Repositories.ForceTrx
{
    public class ForceTrxRepository : BaseSimpleRepository<ForceTrxResult>, IForceTrxRepository
    {
        static ForceTrxRepository()
        {
            SqlMapper.SetTypeMap(typeof(ForceTrxResult), new CustomPropertyTypeMap(
                typeof(ForceTrxResult),
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

        public ForceTrxRepository(eTellerDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<IEnumerable<ForceTrxResult>> GetAllAsync(string lanCode)
        {
            using var connection = new SqlConnection(_context.Database.GetConnectionString());
            await connection.OpenAsync();

            return await connection.QueryAsync<ForceTrxResult>(
                "dbo.sp_ForceTRX_info_Select",
                new { LANCODE = lanCode },
                commandType: System.Data.CommandType.StoredProcedure);
        }

        public async Task<IEnumerable<ForceTrxResult>> GetByIdAsync(string lanCode, int trfId)
        {
            using var connection = new SqlConnection(_context.Database.GetConnectionString());
            await connection.OpenAsync();

            return await connection.QueryAsync<ForceTrxResult>(
                "dbo.sp_ForceTRX_info_SelectByTrx_ID",
                new { LANCODE = lanCode, TRX_ID = trfId },
                commandType: System.Data.CommandType.StoredProcedure);
        }
    }
}
