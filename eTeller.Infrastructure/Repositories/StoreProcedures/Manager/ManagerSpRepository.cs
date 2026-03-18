using eTeller.Application.Contracts.StoreProcedures.Manager;
using eTeller.Domain.Models;
using eTeller.Domain.Models.StoredProcedure;
using eTeller.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace eTeller.Infrastructure.Repositories.StoreProcedures.Manager
{
    public class ManagerSpRepository : BaseSimpleRepository<InfoAutorizzazioneUtente>, IManagerSpRepository
    {
        public ManagerSpRepository(eTellerDbContext dbContext) : base(dbContext)
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

        public async Task<IEnumerable<UsersRoleFunction>> GetUsersRoleByFunIdAsync(int funId, CancellationToken cancellationToken = default)
        {
            return await _context.Set<UsersRoleFunction>()
                .FromSqlInterpolated($@"EXEC dbo.sp_getUsersByFUN_ID @FUN_ID = {funId}")
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }
    }
}
