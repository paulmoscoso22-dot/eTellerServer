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
    public class TotalicCassaSpRepository : BaseSimpleRepository<TotalicCassa>, ITotalicCassaSpRepository
    {
        static TotalicCassaSpRepository()
        {
            // Configure Dapper for mapping using [Column()] attributes
            SqlMapper.SetTypeMap(typeof(TotalicCassa), new CustomPropertyTypeMap(
                typeof(TotalicCassa),
                (type, columnName) =>
                {
                    var properties = type.GetProperties();
                    
                    // First: search by mapping [Column()] attributes to SQL column name
                    var property = properties.FirstOrDefault(p =>
                    {
                        var columnAttr = p.GetCustomAttribute<ColumnAttribute>();
                        return columnAttr?.Name?.Equals(columnName, StringComparison.OrdinalIgnoreCase) == true;
                    });
                    
                    // If not found by attribute, try case-insensitive match on property name
                    if (property == null)
                    {
                        property = properties.FirstOrDefault(p => 
                            p.Name.Equals(columnName, StringComparison.OrdinalIgnoreCase));
                    }
                    
                    return property;
                }));
        }

        public TotalicCassaSpRepository(eTellerDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<IEnumerable<TotalicCassa>> GetTotaliCassaByClientIDAndDataAndCutID(string tocCliId, DateTime tocData, string tocCutId, string tocBraId)
        {
            using (var connection = new SqlConnection(_context.Database.GetConnectionString()))
            {
                await connection.OpenAsync();

                var parameters = new
                {
                    TOC_CLI_ID = tocCliId,
                    TOC_DATA = tocData,
                    TOC_CUT_ID = tocCutId,
                    TOC_BRA_ID = tocBraId
                };

                var result = await connection.QueryAsync<TotalicCassa>(
                    "dbo.sp_TotaliCassa_SelectByClientIDAndDataAndCutID",
                    parameters,
                    commandType: System.Data.CommandType.StoredProcedure);

                return result;
            }
        }
    }
}
