using eTeller.Domain.Models.StoredProcedure;
using eTeller.Domain.Models;
using static eTeller.Application.Contracts.Commons.IBaseSimpleRepository;

namespace eTeller.Application.Contracts.StoreProcedures.Manager
{
    public interface IManagerSpRepository : IBaseSimpleRepository<InfoAutorizzazioneUtente>
    {
        Task<IEnumerable<InfoAutorizzazioneUtente>> GetAllUsersByUsrIdAsync(string usrId, string? funlikeName = null, string? funlikeDes = null, bool tutti = false, CancellationToken cancellationToken = default);
        Task<IEnumerable<SysFunctions>> GetSysFunctionsAsync(CancellationToken cancellationToken = default);
        Task<IEnumerable<sys_ROLE>> GetSysRoleByFunIdAsync(int funId, CancellationToken cancellationToken = default);
        Task<IEnumerable<USERS_AllAccess>> GetUsersAllAccessAsync(string usrId, string? funlikeName = null, string? funlikeDes = null, bool tutti = false, CancellationToken cancellationToken = default);
        Task<IEnumerable<UsersRoleFunction>> GetUsersRoleByFunIdAsync(int funId, CancellationToken cancellationToken = default);
    }
}
