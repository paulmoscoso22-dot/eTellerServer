using eTeller.Application.Contracts.StoreProcedures.Vigilanza;
using eTeller.Domain.Models;
using eTeller.Domain.Models.StoredProcedure;
using eTeller.Infrastructure.Context;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Dapper;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace eTeller.Infrastructure.Repositories.StoreProcedures.Vigilanza
{
    public class VigilanzaSpRepository : BaseSimpleRepository<SpTransactionGiornaleAntiriciclagio>, IVigilanzaSpRepository
    {
        private const string FunctionCodeARA = "ARA";
        private const string TableNameAntirecAppearer = "ANTIREC_APPEARER";

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

            // Configura Dapper per il mapping di SpAntirecRules
            SqlMapper.SetTypeMap(typeof(SpAntirecRules), new CustomPropertyTypeMap(
                typeof(SpAntirecRules),
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

            // Configura Dapper per il mapping di AppearerAll
            SqlMapper.SetTypeMap(typeof(AppearerAll), new CustomPropertyTypeMap(
                typeof(AppearerAll),
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

        public async Task<List<SpAntirecRules>> GetAntirecRulesByParameters(
            string? arlOpTypeId,
            string? arlCurTypeId,
            string? arlAcctId,
            string? arlAcctType)
        {
            using (var connection = new SqlConnection(_context.Database.GetConnectionString()))
            {
                await connection.OpenAsync();

                var parameters = new
                {
                    ARL_OP_TYPE_ID = arlOpTypeId,
                    ARL_CUR_TYPE_ID = arlCurTypeId,
                    ARL_ACCT_ID = arlAcctId,
                    ARL_ACCT_TYPE = arlAcctType
                };

                var result = await connection.QueryAsync<SpAntirecRules>(
                    "dbo.ANTIREC_RULES_ByParameters_Select",
                    parameters,
                    commandType: System.Data.CommandType.StoredProcedure);

                return result.ToList();
            }
        }

        //Gestione grid

        public async Task<List<AppearerAll>> GetAppearerByParameters(
            string? nome1,
            string? nome2,
            string? nome3,
            string? nome4,
            DateTime? araBirthdate,
            bool? araRecComplete,
            DateTime? minRecdate)
        {
            using (var connection = new SqlConnection(_context.Database.GetConnectionString()))
            {
                await connection.OpenAsync();

                var parameters = new
                {
                    Nome1 = nome1,
                    Nome2 = nome2,
                    Nome3 = nome3,
                    Nome4 = nome4,
                    ARA_BIRTHDATE = araBirthdate,
                    ARA_REC_COMPLETE = araRecComplete,
                    MIN_RECDATE = minRecdate
                };

                var result = await connection.QueryAsync<AppearerAll>(
                    "dbo.ANTIREC_APPEARER_ByParameters_Select",
                    parameters,
                    commandType: System.Data.CommandType.StoredProcedure);

                return result.ToList();
            }
        }

        public async Task<List<AppearerAll>> GetAppearerByParametersWithExpiry(
            string araName,
            string? araBirthdate,
            bool araRecComplete,
            bool showExpiredRecords,
            int recordValidityDays = 365)
        {
            // Split name into up to 4 parts for LIKE matching
            var nameParts = SplitNameIntoParts(araName);

            // Calculate minimum record date if filtering expired records
            DateTime? minRecdate = showExpiredRecords 
                ? null 
                : DateTime.Now.AddDays(-recordValidityDays);

            // Parse birthdate string to DateTime
            DateTime? parsedBirthdate = TryParseDateTime(araBirthdate);

            using (var connection = new SqlConnection(_context.Database.GetConnectionString()))
            {
                await connection.OpenAsync();

                var parameters = new
                {
                    Nome1 = nameParts[0],
                    Nome2 = nameParts[1],
                    Nome3 = nameParts[2],
                    Nome4 = nameParts[3],
                    ARA_BIRTHDATE = parsedBirthdate,
                    ARA_REC_COMPLETE = araRecComplete,
                    MIN_RECDATE = minRecdate
                };

                var result = await connection.QueryAsync<AppearerAll>(
                    "dbo.ANTIREC_APPEARER_ByParameters_Select",
                    parameters,
                    commandType: System.Data.CommandType.StoredProcedure);

                return result.ToList();
            }
        }

        public async Task<AppearerAll?> GetAppearerAllByAraId(int araId)
        {
            using (var connection = new SqlConnection(_context.Database.GetConnectionString()))
            {
                await connection.OpenAsync();

                var parameters = new
                {
                    ARA_ID = araId
                };

                var result = await connection.QueryFirstOrDefaultAsync<AppearerAll>(
                    "dbo.ANTIREC_APPEARER_ByAraId_Select",
                    parameters,
                    commandType: System.Data.CommandType.StoredProcedure);

                return result;
            }
        }

        public async Task<int> InsertHisAntirecAppearer(
            DateTime hisDate,
            int araId,
            DateTime araRecdate,
            string araName,
            DateTime? araBirthdate,
            string? araBirthplace,
            string? araNationality,
            string? araIddocnum,
            DateTime? araDocexpdate,
            bool araRecComplete,
            string? araRepresents,
            string? araAddress)
        {
            using (var connection = new SqlConnection(_context.Database.GetConnectionString()))
            {
                await connection.OpenAsync();

                var parameters = new DynamicParameters();
                parameters.Add("@HIS_DATE", hisDate);
                parameters.Add("@ARA_ID", araId);
                parameters.Add("@ARA_RECDATE", araRecdate);
                parameters.Add("@ARA_NAME", araName);
                parameters.Add("@ARA_BIRTHDATE", araBirthdate);
                parameters.Add("@ARA_BIRTHPLACE", araBirthplace);
                parameters.Add("@ARA_NATIONALITY", araNationality);
                parameters.Add("@ARA_IDDOCNUM", araIddocnum);
                parameters.Add("@ARA_DOCEXPDATE", araDocexpdate);
                parameters.Add("@ARA_REC_COMPLETE", araRecComplete);
                parameters.Add("@ARA_REPRESENTS", araRepresents);
                parameters.Add("@ARA_ADDRESS", araAddress);
                parameters.Add("@HIS_ID", dbType: System.Data.DbType.Int32, direction: System.Data.ParameterDirection.Output);

                await connection.ExecuteAsync(
                    "dbo.HIS_ANTIREC_APPEARER_Insert",
                    parameters,
                    commandType: System.Data.CommandType.StoredProcedure);

                return parameters.Get<int>("@HIS_ID");
            }
        }

        public async Task<int> UpdateAntirecAppearer(
            int araId,
            DateTime araRecdate,
            string araName,
            DateTime? araBirthdate,
            string? araBirthplace,
            string? araNationality,
            string? araIddocnum,
            DateTime? araDocexpdate,
            string? araRepresents,
            string? araAddress,
            bool araRecComplete,
            bool araIsupdated)
        {
            using (var connection = new SqlConnection(_context.Database.GetConnectionString()))
            {
                await connection.OpenAsync();

                var parameters = new DynamicParameters();
                parameters.Add("@ARA_ID", araId);
                parameters.Add("@ARA_RECDATE", araRecdate);
                parameters.Add("@ARA_NAME", araName);
                parameters.Add("@ARA_BIRTHDATE", araBirthdate);
                parameters.Add("@ARA_BIRTHPLACE", araBirthplace);
                parameters.Add("@ARA_NATIONALITY", araNationality);
                parameters.Add("@ARA_IDDOCNUM", araIddocnum);
                parameters.Add("@ARA_DOCEXPDATE", araDocexpdate);
                parameters.Add("@ARA_REPRESENTS", araRepresents);
                parameters.Add("@ARA_ADDRESS", araAddress);
                parameters.Add("@ARA_REC_COMPLETE", araRecComplete);
                parameters.Add("@ARA_ISUPDATED", araIsupdated);

                await connection.ExecuteAsync(
                    "dbo.ANTIREC_APPEARER_Update",
                    parameters,
                    commandType: System.Data.CommandType.StoredProcedure);

                return araId;
            }
        }

        public async Task<int> InsertARA(
            string traUser,
            string traStation,
            DateTime araRecdate,
            string araName,
            string? araBirthdate,
            string? araBirthplace,
            string? araIddocnum,
            string? araNationality,
            string? araDocexpdate,
            string? araRepresents,
            string? araAddress,
            bool araRecComplete)
        {
            using var connection = new SqlConnection(_context.Database.GetConnectionString());
            await connection.OpenAsync();
            
            using var transaction = connection.BeginTransaction();
            try
            {
                // Insert the appearer record and get the generated ARA_ID
                var araId = await InsertAntirecAppearer(
                    araRecdate,
                    araName,
                    araBirthdate,
                    araBirthplace,
                    araNationality,
                    araIddocnum,
                    araDocexpdate,
                    araRepresents,
                    araAddress,
                    araRecComplete,
                    connection,
                    transaction);

                if (araId > 0)
                {
                    // Insert history record
                    await InsertARAhistory(
                        araId,
                        araName,
                        araBirthdate,
                        araBirthplace,
                        araNationality,
                        araIddocnum,
                        araDocexpdate,
                        araRepresents,
                        araAddress,
                        araRecComplete,
                        connection,
                        transaction);

                    // Build description string for trace
                    var description = new System.Text.StringBuilder()
                        .Append($"ARA_RECDATE: {araRecdate:dd.MM.yyyy}")
                        .Append($" ARA_NAME: {araName}")
                        .Append($" ARA_BIRTHDATE: {araBirthdate ?? ""}")
                        .Append($" ARA_IDDOCNUM: {araIddocnum ?? ""}")
                        .Append($" ARA_NATIONALITY: {araNationality ?? ""}")
                        .Append($" ARA_DOCEXPDATE: {araDocexpdate ?? ""}")
                        .Append($" ARA_BIRTHPLACE: {araBirthplace ?? ""}")
                        .Append($" ARA_ADDRESS: {araAddress ?? ""}")
                        .Append($" ARA_REPRESENTS: {araRepresents ?? ""}")
                        .Append($" ARA_REC_COMPLETE: {araRecComplete}")
                        .ToString();

                    // Insert trace record
                    await InsertTraceRecord(
                        traUser,
                        traStation,
                        FunctionCodeARA,
                        "GestioneComparenti",
                        TableNameAntirecAppearer,
                        araId.ToString(),
                        description,
                        connection,
                        transaction);

                    transaction.Commit();
                    return araId;
                }
                else
                {
                    transaction.Rollback();
                    return 0;
                }
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        public async Task<int> UpdateARA(
            string traUser,
            string traStation,
            int araId,
            string araName,
            string? araBirthdate,
            string? araBirthplace,
            string? araNationality,
            string? araIddocnum,
            string? araDocexpdate,
            string? araRepresents,
            string? araAddress,
            bool araRecComplete)
        {
            using var connection = new SqlConnection(_context.Database.GetConnectionString());
            await connection.OpenAsync();
            
            using var transaction = connection.BeginTransaction();
            try
            {
                // Insert history record first
                await InsertARAhistory(
                    araId,
                    araName,
                    araBirthdate,
                    araBirthplace,
                    araNationality,
                    araIddocnum,
                    araDocexpdate,
                    araRepresents,
                    araAddress,
                    araRecComplete,
                    connection,
                    transaction);

                // Update the appearer record
                await UpdateAntirecAppearer(
                    araId,
                    DateTime.Now,
                    araName,
                    araBirthdate,
                    araBirthplace,
                    araNationality,
                    araIddocnum,
                    araDocexpdate,
                    araRepresents,
                    araAddress,
                    araRecComplete,
                    connection,
                    transaction);

                // Build description string for trace
                var description = new System.Text.StringBuilder()
                    .Append($"ARA_ID: {araId}")
                    .Append($" ARA_NAME: {araName}")
                    .Append($" ARA_BIRTHDATE: {araBirthdate ?? ""}")
                    .Append($" ARA_BIRTHPLACE: {araBirthplace ?? ""}")
                    .Append($" ARA_NATIONALITY: {araNationality ?? ""}")
                    .Append($" ARA_IDDOCNUM: {araIddocnum ?? ""}")
                    .Append($" ARA_DOCEXPDATE: {araDocexpdate ?? ""}")
                    .Append($" ARA_REPRESENTS: {araRepresents ?? ""}")
                    .Append($" ARA_ADDRESS: {araAddress ?? ""}")
                    .Append($" ARA_REC_COMPLETE: {araRecComplete}")
                    .ToString();

                // Insert trace record
                await InsertTraceRecord(
                    traUser,
                    traStation,
                    "ARAE",
                    "GestioneComparenti",
                    TableNameAntirecAppearer,
                    araId.ToString(),
                    description,
                    connection,
                    transaction);

                transaction.Commit();
                return araId;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        public async Task<bool> DeleteARA(
            string traUser,
            string traStation,
            int araId)
        {
            using var connection = new SqlConnection(_context.Database.GetConnectionString());
            await connection.OpenAsync();
            
            using var transaction = connection.BeginTransaction();
            try
            {
                // Delete the appearer record
                var affectedRows = await DeleteAntirecAppearer(
                    araId,
                    connection,
                    transaction);

                // Insert trace record for the delete operation
                await InsertTraceRecord(
                    traUser,
                    traStation,
                    "ARAD",
                    "ANTIREC_APPEARER_Delete",
                    TableNameAntirecAppearer,
                    araId.ToString(),
                    "DELETE ANTIRECYCLING APPEARER",
                    connection,
                    transaction);

                transaction.Commit();
                return affectedRows > 0;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        private static DateTime? TryParseDateTime(string? dateString)
        {
            if (string.IsNullOrEmpty(dateString))
                return null;

            return DateTime.TryParse(dateString, out DateTime result) ? result : null;
        }

        private static string[] SplitNameIntoParts(string araName)
        {
            var nameParts = new string[4];
            
            // Initialize all parts with empty strings
            for (int i = 0; i < nameParts.Length; i++)
                nameParts[i] = string.Empty;

            // Split the name and populate the array
            if (!string.IsNullOrWhiteSpace(araName))
            {
                var nameArray = araName.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
                
                for (int i = 0; i < Math.Min(nameArray.Length, nameParts.Length); i++)
                {
                    nameParts[i] = nameArray[i];
                }
            }

            return nameParts;
        }

        private static async Task<int> InsertARAhistory(
            int araId,
            string araName,
            string? araBirthdate,
            string? araBirthplace,
            string? araNationality,
            string? araIddocnum,
            string? araDocexpdate,
            string? araRepresents,
            string? araAddress,
            bool araRecComplete,
            SqlConnection connection,
            SqlTransaction transaction)
        {
            var parsedBirthdate = TryParseDateTime(araBirthdate);
            var parsedDocexpdate = TryParseDateTime(araDocexpdate);
            
            var parameters = new DynamicParameters();
            parameters.Add("@HIS_DATE", DateTime.Now);
            parameters.Add("@ARA_ID", araId);
            parameters.Add("@ARA_RECDATE", DateTime.Now);
            parameters.Add("@ARA_NAME", araName);
            parameters.Add("@ARA_BIRTHDATE", parsedBirthdate);
            parameters.Add("@ARA_BIRTHPLACE", araBirthplace);
            parameters.Add("@ARA_NATIONALITY", araNationality);
            parameters.Add("@ARA_IDDOCNUM", araIddocnum);
            parameters.Add("@ARA_DOCEXPDATE", parsedDocexpdate);
            parameters.Add("@ARA_REC_COMPLETE", araRecComplete);
            parameters.Add("@ARA_REPRESENTS", araRepresents);
            parameters.Add("@ARA_ADDRESS", araAddress);
            parameters.Add("@HIS_ID", dbType: System.Data.DbType.Int32, direction: System.Data.ParameterDirection.Output);

            await connection.ExecuteAsync(
                "dbo.HIS_ANTIREC_APPEARER_Insert",
                parameters,
                transaction: transaction,
                commandType: System.Data.CommandType.StoredProcedure);

            return parameters.Get<int>("@HIS_ID");
        }

        private static async Task<int> InsertAntirecAppearer(
            DateTime araRecdate,
            string araName,
            string? araBirthdate,
            string? araBirthplace,
            string? araNationality,
            string? araIddocnum,
            string? araDocexpdate,
            string? araRepresents,
            string? araAddress,
            bool araRecComplete,
            SqlConnection connection,
            SqlTransaction transaction)
        {
            var parsedBirthdate = TryParseDateTime(araBirthdate);
            var parsedDocexpdate = TryParseDateTime(araDocexpdate);
            
            var parameters = new DynamicParameters();
            parameters.Add("@ARA_RECDATE", araRecdate);
            parameters.Add("@ARA_NAME", araName);
            parameters.Add("@ARA_BIRTHDATE", parsedBirthdate);
            parameters.Add("@ARA_BIRTHPLACE", araBirthplace);
            parameters.Add("@ARA_NATIONALITY", araNationality);
            parameters.Add("@ARA_IDDOCNUM", araIddocnum);
            parameters.Add("@ARA_DOCEXPDATE", parsedDocexpdate);
            parameters.Add("@ARA_REC_COMPLETE", araRecComplete);
            parameters.Add("@ARA_REPRESENTS", araRepresents);
            parameters.Add("@ARA_ADDRESS", araAddress);
            parameters.Add("@ARA_ID", dbType: System.Data.DbType.Int32, direction: System.Data.ParameterDirection.Output);

            await connection.ExecuteAsync(
                "dbo.ANTIREC_APPEARER_Insert",
                parameters,
                transaction: transaction,
                commandType: System.Data.CommandType.StoredProcedure);

            return parameters.Get<int>("@ARA_ID");
        }

        private static async Task InsertTraceRecord(
            string traUser,
            string traStation,
            string traFunCode,
            string traSubFun,
            string traTabNam,
            string traEntCode,
            string traDes,
            SqlConnection connection,
            SqlTransaction transaction)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@TRA_Time", DateTime.Now);
            parameters.Add("@TRA_User", traUser);
            parameters.Add("@TRA_FunCode", traFunCode);
            parameters.Add("@TRA_SubFun", traSubFun);
            parameters.Add("@TRA_Station", traStation);
            parameters.Add("@TRA_TabNam", traTabNam);
            parameters.Add("@TRA_EntCode", traEntCode);
            parameters.Add("@TRA_RevTrxTrace", string.Empty);
            parameters.Add("@TRA_Des", traDes);
            parameters.Add("@TRA_ExtRef", string.Empty);
            parameters.Add("@TRA_Error", false);

            await connection.ExecuteAsync(
                "dbo.sp_Trace_Insert",
                parameters,
                transaction: transaction,
                commandType: System.Data.CommandType.StoredProcedure);
        }

        private static async Task<int> UpdateAntirecAppearer(
            int araId,
            DateTime araRecdate,
            string araName,
            string? araBirthdate,
            string? araBirthplace,
            string? araNationality,
            string? araIddocnum,
            string? araDocexpdate,
            string? araRepresents,
            string? araAddress,
            bool araRecComplete,
            SqlConnection connection,
            SqlTransaction transaction)
        {
            var parsedBirthdate = TryParseDateTime(araBirthdate);
            var parsedDocexpdate = TryParseDateTime(araDocexpdate);
            
            var parameters = new DynamicParameters();
            parameters.Add("@ARA_ID", araId);
            parameters.Add("@ARA_RECDATE", araRecdate);
            parameters.Add("@ARA_NAME", araName);
            parameters.Add("@ARA_BIRTHDATE", parsedBirthdate);
            parameters.Add("@ARA_BIRTHPLACE", araBirthplace);
            parameters.Add("@ARA_NATIONALITY", araNationality);
            parameters.Add("@ARA_IDDOCNUM", araIddocnum);
            parameters.Add("@ARA_DOCEXPDATE", parsedDocexpdate);
            parameters.Add("@ARA_REPRESENTS", araRepresents);
            parameters.Add("@ARA_ADDRESS", araAddress);
            parameters.Add("@ARA_REC_COMPLETE", araRecComplete);
            parameters.Add("@ARA_ISUPDATED", false);

            await connection.ExecuteAsync(
                "dbo.ANTIREC_APPEARER_Update",
                parameters,
                transaction: transaction,
                commandType: System.Data.CommandType.StoredProcedure);

            return araId;
        }

        private static async Task<int> DeleteAntirecAppearer(
            int araId,
            SqlConnection connection,
            SqlTransaction transaction)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@ARA_ID", araId);

            var affectedRows = await connection.ExecuteAsync(
                "dbo.ANTIREC_APPEARER_Delete",
                parameters,
                transaction: transaction,
                commandType: System.Data.CommandType.StoredProcedure);

            return affectedRows;
        }
    }
}
