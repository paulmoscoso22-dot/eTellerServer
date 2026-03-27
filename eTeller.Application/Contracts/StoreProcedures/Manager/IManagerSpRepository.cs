using System.Collections.Generic;
using System.Threading;
using eTeller.Domain.Models.StoredProcedure;
using eTeller.Domain.Models;
using static eTeller.Application.Contracts.Commons.IBaseSimpleRepository;

namespace eTeller.Application.Contracts.StoreProcedures.Manager
{
    /// <summary>
    /// Repository for manager-related stored-procedure operations.
    /// Provides query and command methods for sys_* entities.
    /// </summary>
    public interface IManagerSpRepository : IBaseSimpleRepository<InfoAutorizzazioneUtente>
    {
        // Query: Users / Access
        /// <summary>Get authorizations for a user by user id.</summary>
        Task<IEnumerable<InfoAutorizzazioneUtente>> GetAllUsersByUsrIdAsync(string usrId, string? funLikeName = null, string? funLikeDescription = null, bool tutti = false, CancellationToken cancellationToken = default);

        /// <summary>Get all system functions.</summary>
        Task<IEnumerable<SysFunctions>> GetSysFunctionsAsync(CancellationToken cancellationToken = default);

        /// <summary>Get roles for a given function id.</summary>
        Task<IEnumerable<sys_ROLE>> GetSysRoleByFunIdAsync(int funId, CancellationToken cancellationToken = default);

        /// <summary>Get all-access entries for a user.</summary>
        Task<IEnumerable<USERS_AllAccess>> GetUsersAllAccessAsync(string usrId, string? funLikeName = null, string? funLikeDescription = null, bool tutti = false, CancellationToken cancellationToken = default);

        /// <summary>Get users associated to a function id.</summary>
        Task<IEnumerable<UsersRoleFunction>> GetUsersRoleByFunIdAsync(int funId, CancellationToken cancellationToken = default);

        // Commands: insert / update / delete
        /// <summary>Insert a new sys_FUNCTIONS record and write a trace entry.</summary>
        Task<bool> InsertSysFunctionAsync(string traUser, string traStation, string funName, string? funDescription, int funHostcode, bool offline, CancellationToken cancellationToken = default);

        /// <summary>Update an existing sys_FUNCTIONS record and write a trace entry.</summary>
        Task<bool> UpdateSysFunctionAsync(string traUser, string traStation, int funId, string funName, string? funDescription, int funHostcode, bool offline, CancellationToken cancellationToken = default);

        /// <summary>Delete a sys_FUNCTIONS record (if not used) and write a trace entry.</summary>
        Task<bool> DeleteSysFunctionAsync(string traUser, string traStation, int funId, CancellationToken cancellationToken = default);
    }
}
