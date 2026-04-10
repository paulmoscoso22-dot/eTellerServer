using eTeller.Domain.Models;
using static eTeller.Application.Contracts.Commons.IBaseSimpleRepository;

namespace eTeller.Application.Contracts
{
    public interface IUserRepository : IBaseSimpleRepository<User>
    {
        Task<bool> Exists(string usrId);
        Task<User?> GetByIdAsync(string usrId);
        Task<bool> ValidateCredentialsAsync(string usrId, string password);
        Task<int> IncrementFailedAttemptsAsync(string usrId);
        Task ResetFailedAttemptsAsync(string usrId);
        Task BlockUserAsync(string usrId);
        Task UpdatePasswordAsync(string usrId, string newPassword);
        Task<bool> IsPasswordInHistoryAsync(string usrId, string password, int historyCount);
        Task<bool> IsUserBlockedAsync(string usrId);
        Task<bool> IsUserEnabledAsync(string usrId);
        Task<IEnumerable<User>> GetUsersActiveAndBlockedAsync(CancellationToken cancellationToken = default);
    }
}
