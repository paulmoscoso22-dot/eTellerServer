using eTeller.Application.Contracts.Manager;
using eTeller.Domain.Models;
using eTeller.Domain.Models.StoredProcedure;
using eTeller.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using System.Data;
using Dapper;

namespace eTeller.Infrastructure.Repositories.StoreProcedures.Manager
{
    public class ManagerRepository : BaseSimpleRepository<InfoAutorizzazioneUtente>, IManagerRepository
    {
        public ManagerRepository(eTellerDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<IEnumerable<InfoAutorizzazioneUtente>> GetAllUsersByUsrIdAsync(string usrId, string? funlikeName = null, string? funlikeDes = null, bool tutti = false, CancellationToken cancellationToken = default)
        {
            return await _context.Set<InfoAutorizzazioneUtente>()
                .FromSqlInterpolated($@"
                    EXEC dbo.sys_USERS_AllAccess_ByUSR_ID
                        @USR_ID = {usrId},
                        @FUNLIKE_NAME = {funlikeName},
                        @FUNLIKE_DES = {funlikeDes},
                        @TUTTI = {(tutti ? 1 : 0)}")
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<SysFunctions>> GetSysFunctionsAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Set<SysFunctions>()
                .FromSqlInterpolated($@"EXEC dbo.sys_FUNCTIONS_Select")
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<USERS_AllAccess>> GetUsersAllAccessAsync(string usrId, string? funlikeName = null, string? funlikeDes = null, bool tutti = false, CancellationToken cancellationToken = default)
        {
            return await _context.Set<USERS_AllAccess>()
                .FromSqlInterpolated($@"
                    EXEC dbo.sys_USERS_AllAccess_ByUSR_ID
                        @USR_ID = {usrId},
                        @FUNLIKE_NAME = {funlikeName},
                        @FUNLIKE_DES = {funlikeDes},
                        @TUTTI = {(tutti ? 1 : 0)}")
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<sys_ROLE>> GetSysRoleByFunIdAsync(int funId, CancellationToken cancellationToken = default)
        {
            return await _context.Set<sys_ROLE>()
                .FromSqlInterpolated($@"EXEC dbo.sys_ROLE_SelectByFUN_ID @FUN_ID = {funId}")
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<sys_ROLE>> GetAllRoleAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Set<sys_ROLE>()
                .FromSqlInterpolated($@"EXEC dbo.sys_ROLE_Select")
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<sys_ROLE>> GetSysRoleByUsrIdAsync(string usrId, CancellationToken cancellationToken = default)
        {
            return await _context.Set<sys_ROLE>()
                .FromSqlInterpolated($@"EXEC dbo.sys_ROLE_Select_BYUSR_ID @USR_ID = {usrId}")
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<sys_ROLE>> GetSysRoleNotForUsrIdAsync(string usrId, CancellationToken cancellationToken = default)
        {
            return await _context.Set<sys_ROLE>()
                .FromSqlInterpolated($@"EXEC dbo.sys_ROLE_Select_NotforUSR_ID @USR_ID = {usrId}")
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<eTeller.Domain.Models.StoredProcedure.UserSelectRole>> GetUsersByRoleIdAsync(int roleId, CancellationToken cancellationToken = default)
        {
            return await _context.Set<eTeller.Domain.Models.StoredProcedure.UserSelectRole>()
                .FromSqlInterpolated($@"EXEC dbo.sys_USERS_SelectByROLE_ID @ROLE_ID = {roleId}")
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<UsersRoleFunction>> GetUsersRoleByFunIdAsync(int funId, CancellationToken cancellationToken = default)
        {
            return await _context.Set<UsersRoleFunction>()
                .FromSqlInterpolated($@"EXEC dbo.sp_getUsersByFUN_ID @FUN_ID = {funId}")
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<FunctionRole>> GetFunctionRoleByRoleIdAsync(int roleId, string? funLikeName, string? funLikeDes, CancellationToken cancellationToken = default)
        {
            return await _context.Set<FunctionRole>()
                .FromSqlInterpolated($@"EXEC dbo.sys_FUNCTION_FUNCTION_ROLE_SelectByRoleID2 @ROLE_ID = {roleId}, @FUNLIKE_NAME = {funLikeName}, @FUNLIKE_DES = {funLikeDes}")
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public async Task<bool> InsertSysFunctionAsync(string traUser, string traStation, string funName, string? funDescription, int funHostcode, bool offline, CancellationToken cancellationToken = default)
        {
            using var connection = new SqlConnection(_context.Database.GetConnectionString());
            await connection.OpenAsync(cancellationToken);

            using var transaction = connection.BeginTransaction();

            try
            {
                var nameCount = await ExecuteScalarIntAsync(connection, transaction, "dbo.sys_FUNCTIONS_CountByName", new { FUN_NAME = funName });
                if (nameCount != 0)
                    throw new Exception("FUN NAME ALREADY EXISTS");

                if (funHostcode != 0)
                {
                    var hostCount = await ExecuteScalarIntAsync(connection, transaction, "dbo.sys_FUNCTIONS_CountByHostCode", new { FUN_HOSTCODE = funHostcode });
                    if (hostCount != 0)
                        throw new Exception("FUN_HOSTCODE ALREADY EXISTS");
                }

                var rows = await connection.ExecuteAsync(
                    "dbo.sys_FUNCTIONS_Insert",
                    new { FUN_NAME = funName, FUN_DESCRIPTION = funDescription, FUN_HOSTCODE = funHostcode, Offline = offline },
                    commandType: CommandType.StoredProcedure,
                    transaction: transaction);

                var funId = await ExecuteScalarIntAsync(connection, transaction, "dbo.sys_FUNCTIONS_IDByName", new { FUN_NAME = funName });

                var str = $"ID: {funId} FUN_NAME: {funName} FUN_DESCRIPTION: {funDescription} FUN_HOSTCODE: {funHostcode} Offline: {offline}";

                await InsertTraceAsync(connection, transaction, traUser, traStation, "OPE", "INSERTFUNCTIONS", "sys_FUNCTIONS", funId.ToString(), "UPDATE FUNCTION: " + str);

                transaction.Commit();

                return rows > 0;
            }
            catch
            {
                try { transaction.Rollback(); } catch { }
                throw;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }
        }

        public async Task<bool> UpdateSysFunctionAsync(string traUser, string traStation, int funId, string funName, string? funDescription, int funHostcode, bool offline, CancellationToken cancellationToken = default)
        {
            using var connection = new SqlConnection(_context.Database.GetConnectionString());
            await connection.OpenAsync(cancellationToken);

            using var transaction = connection.BeginTransaction();

            try
            {
                var idByHost = await ExecuteScalarNullableIntAsync(connection, transaction, "dbo.sys_FUNCTIONS_IDByHOSTCODE", new { HOSTCODE = funHostcode });
                if (idByHost.HasValue)
                {
                    var idByHostVal = idByHost.Value;
                    if (funHostcode != 0 && idByHostVal != funId)
                        throw new Exception("HOSTCODE ALREADY USED");
                }

                var idByName = await ExecuteScalarNullableIntAsync(connection, transaction, "dbo.sys_FUNCTIONS_IDByName", new { FUN_NAME = funName });
                if (idByName.HasValue && idByName.Value != funId)
                    throw new Exception("FUN_NAME ALREADY USED");

                var rows = await connection.ExecuteAsync(
                    "dbo.sys_FUNCTIONS_Update",
                    new { FUN_ID = funId, FUN_NAME = funName, FUN_DESCRIPTION = funDescription, FUN_HOSTCODE = funHostcode, Offline = offline },
                    commandType: CommandType.StoredProcedure,
                    transaction: transaction);

                var str = $"ID: {funId} FUN_NAME: {funName} FUN_DESCRIPTION: {funDescription} FUN_HOSTCODE: {funHostcode} Offline: {offline}";

                await InsertTraceAsync(connection, transaction, traUser, traStation, "MOD", "UPDATEFUNCTIONS", "sys_FUNCTIONS", funId.ToString(), "UPDATE FUNCTION: " + str);

                transaction.Commit();

                return rows > 0;
            }
            catch
            {
                try { transaction.Rollback(); } catch { }
                throw;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }
        }

        public async Task<bool> DeleteSysFunctionAsync(string traUser, string traStation, int funId, CancellationToken cancellationToken = default)
        {
            using var connection = new SqlConnection(_context.Database.GetConnectionString());
            await connection.OpenAsync(cancellationToken);

            using var transaction = connection.BeginTransaction();

            try
            {
                var usedCount = await ExecuteScalarIntAsync(connection, transaction, "dbo.sys_ROLE_CountByFUN_ID", new { FUN_ID = funId });
                if (usedCount > 0)
                    throw new Exception("FUNCTION ALREADY USED");

                var rows = await connection.ExecuteAsync(
                    "dbo.sys_FUNCTIONS_Delete",
                    new { Original_FUN_ID = funId },
                    commandType: CommandType.StoredProcedure,
                    transaction: transaction);

                await InsertTraceAsync(connection, transaction, traUser, traStation, "EST", "DELETEFUNCTIONS", "sys_FUNCTIONS", funId.ToString(), "DELETE FUNCTION: " + funId);

                transaction.Commit();

                return rows > 0;
            }
            catch
            {
                try { transaction.Rollback(); } catch { }
                throw;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }
        }

        private static async Task<int> ExecuteScalarIntAsync(SqlConnection conn, IDbTransaction? tx, string proc, object? parameters = null)
        {
            if (parameters is null)
                return await conn.QuerySingleAsync<int>(proc, commandType: CommandType.StoredProcedure, transaction: tx);

            return await conn.QuerySingleAsync<int>(proc, parameters, commandType: CommandType.StoredProcedure, transaction: tx);
        }

        private static async Task<int?> ExecuteScalarNullableIntAsync(SqlConnection conn, IDbTransaction? tx, string proc, object? parameters = null)
        {
            if (parameters is null)
                return await conn.QueryFirstOrDefaultAsync<int?>(proc, commandType: CommandType.StoredProcedure, transaction: tx);

            return await conn.QueryFirstOrDefaultAsync<int?>(proc, parameters, commandType: CommandType.StoredProcedure, transaction: tx);
        }

        private static async Task InsertTraceAsync(SqlConnection conn, IDbTransaction tx, string traUser, string traStation, string traFunCode, string traSubFun, string traTabNam, string traEntCode, string traDes, bool traError = false)
        {
            var traceParams = new DynamicParameters();
            traceParams.Add("TRA_Time", DateTime.Now);
            traceParams.Add("TRA_User", traUser);
            traceParams.Add("TRA_FunCode", traFunCode);
            traceParams.Add("TRA_SubFun", traSubFun);
            traceParams.Add("TRA_Station", traStation);
            traceParams.Add("TRA_TabNam", traTabNam);
            traceParams.Add("TRA_EntCode", traEntCode);
            traceParams.Add("TRA_RevTrxTrace", string.Empty);
            traceParams.Add("TRA_Des", traDes);
            traceParams.Add("TRA_ExtRef", string.Empty);
            traceParams.Add("TRA_Error", traError);

            await conn.ExecuteAsync(
                "dbo.sp_Trace_Insert",
                traceParams,
                commandType: CommandType.StoredProcedure,
                transaction: tx);
        }

        public async Task<int> ResetPasswordAsync(string usrId, bool chgPas, string usrPass, CancellationToken cancellationToken = default)
        {
            using var connection = new SqlConnection(_context.Database.GetConnectionString());
            await connection.OpenAsync(cancellationToken);

            var rows = await connection.ExecuteAsync(
                "dbo.sys_USERS_UpdatePassByID",
                new { USR_ID = usrId, CHG_PAS = chgPas, USR_PASS = usrPass },
                commandType: CommandType.StoredProcedure);

            return rows;
        }
    }
}
