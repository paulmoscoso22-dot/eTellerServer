using eTeller.Application.Contracts;
using eTeller.Domain.Common;
using eTeller.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace eTeller.Infrastructure.Repositories.User
{
    public class UserRepository : BaseSimpleRepository<Domain.Models.User>, IUserRepository
    {
        public UserRepository(eTellerDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<bool> Exists(string usrId)
        {
            return await _context.Users.AnyAsync(u => u.UsrId == usrId);
        }

        public async Task<Domain.Models.User?> GetByIdAsync(string usrId)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.UsrId == usrId);
        }

        public async Task<bool> ValidateCredentialsAsync(string usrId, string password)
        {
            var user = await GetByIdAsync(usrId);
            if (user == null)
                return false;

            return user.UsrPass == password;
        }

        public async Task<int> IncrementFailedAttemptsAsync(string usrId)
        {
            var user = await GetByIdAsync(usrId);
            if (user == null)
                return 0;

            user.UsrTentativi++;
            
            if (user.UsrTentativi >= PasswordPolicyConstants.MaxFailedAttempts)
            {
                user.UsrStatus = UserStatusConstants.Blocked;
            }

            await _context.SaveChangesAsync();
            return user.UsrTentativi;
        }

        public async Task ResetFailedAttemptsAsync(string usrId)
        {
            var user = await GetByIdAsync(usrId);
            if (user == null)
                return;

            user.UsrTentativi = 0;
            await _context.SaveChangesAsync();
        }

        public async Task BlockUserAsync(string usrId)
        {
            var user = await GetByIdAsync(usrId);
            if (user == null)
                return;

            user.UsrStatus = UserStatusConstants.Blocked;
            await _context.SaveChangesAsync();
        }

        public async Task UpdatePasswordAsync(string usrId, string newPassword)
        {
            var user = await GetByIdAsync(usrId);
            if (user == null)
                return;

            user.UsrPass = newPassword;
            user.UsrChgPas = false;
            user.UsrTentativi = 0;
            await _context.SaveChangesAsync();
        }

        public async Task<bool> IsPasswordInHistoryAsync(string usrId, string password, int historyCount)
        {
            var user = await GetByIdAsync(usrId);
            if (user == null)
                return false;

            return user.UsrPass == password;
        }

        public async Task<bool> IsUserBlockedAsync(string usrId)
        {
            var user = await GetByIdAsync(usrId);
            if (user == null)
                return true;

            return user.UsrStatus == UserStatusConstants.Blocked;
        }

        public async Task<bool> IsUserEnabledAsync(string usrId)
        {
            var user = await GetByIdAsync(usrId);
            if (user == null)
                return false;

            return user.UsrStatus == UserStatusConstants.Enabled;
        }

        public async Task<IEnumerable<Domain.Models.User>> GetUsersActiveAndBlockedAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Users
                .Where(u => u.UsrStatus == "ENABLED" || u.UsrStatus == "BLOCKED")
                .OrderBy(u => u.UsrId)
                .ToListAsync(cancellationToken);
        }
    }
}
