using Dapper;
using eTeller.Application.Contracts.BookingRc;
using eTeller.Domain.Models;
using eTeller.Infrastructure.Context;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace eTeller.Infrastructure.Repositories.BookingRc
{
    public class BookingRcRepository : BaseSimpleRepository<StBookingRc>, IBookingRcRepository
    {
        static BookingRcRepository()
        {
            SqlMapper.SetTypeMap(typeof(StAccountType), new CustomPropertyTypeMap(
                typeof(StAccountType),
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

        public BookingRcRepository(eTellerDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<StAccountType>> GetAccountTypesAsync()
        {
            using var connection = new SqlConnection(_context.Database.GetConnectionString());
            await connection.OpenAsync();
            return await connection.QueryAsync<StAccountType>(
                "dbo.ST_AccountType_Select",
                commandType: System.Data.CommandType.StoredProcedure);
        }

        public async Task<int> InsertAsync(StBookingRc item)
        {
            using var connection = new SqlConnection(_context.Database.GetConnectionString());
            await connection.OpenAsync();
            return await connection.ExecuteAsync(
                "dbo.sp_STBookingRC_Insert",
                new
                {
                    CutID = item.BrcCutId,
                    OptID = item.BrcOptId,
                    ActID = item.BrcActId,
                    BRC_CODCAU = item.BrcCodcau,
                    BRC_CODCAUSTO = item.BrcCodcausto,
                    BRC_TEXT1 = item.BrcText1,
                    BRC_TEXT2 = item.BrcText2
                },
                commandType: System.Data.CommandType.StoredProcedure);
        }

        public async Task<int> UpdateAsync(StBookingRc item)
        {
            using var connection = new SqlConnection(_context.Database.GetConnectionString());
            await connection.OpenAsync();
            return await connection.ExecuteAsync(
                "dbo.sp_STBookingRC_Update",
                new
                {
                    CutID = item.BrcCutId,
                    OptID = item.BrcOptId,
                    ActID = item.BrcActId,
                    BRC_CODCAU = item.BrcCodcau,
                    BRC_CODCAUSTO = item.BrcCodcausto,
                    BRC_TEXT1 = item.BrcText1,
                    BRC_TEXT2 = item.BrcText2
                },
                commandType: System.Data.CommandType.StoredProcedure);
        }
    }
}
