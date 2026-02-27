using eTeller.Domain.Models;
using static eTeller.Application.Contracts.Commons.IBaseSimpleRepository;

namespace eTeller.Application.Contracts
{
    public interface IUserSessionRepository : IBaseSimpleRepository<UserSession>
    {
        Task<UserSession?> GetActiveSessionByUserIdAsync(string usrId);
        Task<UserSession?> GetActiveSessionByIpAsync(string ipAddress);
        Task<bool> IsUserLoggedAsync(string usrId);
        Task<bool> IsUserLoggedOnIpAsync(string ipAddress, string usrId);
        Task<UserSession?> CreateSessionAsync(string usrId, string? cliId, string ipAddress, bool forcedLogin);
        Task<bool> TerminateSessionAsync(string usrId);
        Task<bool> TerminateSessionByIdAsync(int sessionId);
        Task UpdateLastActivityAsync(int sessionId);
    }
}
