using eTeller.Application.Contracts;
using CurModel = eTeller.Domain.Models;
using eTeller.Infrastructure.Context;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Dapper;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace eTeller.Infrastructure.Repositories.StoreProcedures.Currency
{
    public class CurrencyRepository : BaseSimpleRepository<Domain.Models.Currency>, ICurrencyRepository
    {
        static CurrencyRepository()
        {
            // Configura Dapper per il mapping usando gli attributi [Column()]
            SqlMapper.SetTypeMap(typeof(Domain.Models.Currency), new CustomPropertyTypeMap(
                typeof(Domain.Models.Currency),
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

        public CurrencyRepository(eTellerDbContext dbContext) : base(dbContext)
        {
        }

        //public async Task<List<CurModel.Currency>> GetAllCurrencies()
        //{
        //    using (var connection = new SqlConnection(_context.Database.GetConnectionString()))
        //    {
        //        await connection.OpenAsync();

        //        var result = await connection.QueryAsync<CurModel.Currency>(
        //            "dbo.sp_Currency_SelectAll",
        //            commandType: System.Data.CommandType.StoredProcedure);

        //        return result.ToList();
        //    }
        //}
    }
}
