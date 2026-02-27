using eTeller.Application.Contracts;
using eTeller.Domain.Common;
using eTeller.Domain.Models;
using eTeller.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace eTeller.Infrastructure.Repositories.Archivi
{
    public class UserRepository : BaseSimpleRepository<User>, IUserRepository
    {
        public UserRepository(eTellerDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<bool> Exists(string usrId)
        {
            return await _context.Users.AnyAsync(u => u.UsrId == usrId);
        }

        public async Task<User?> GetByIdAsync(string usrId)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.UsrId == usrId);
        }

        public async Task<bool> ValidateCredentialsAsync(string usrId, string password)
        {
            var user = await GetByIdAsync(usrId);
            if (user == null)
                return false;

            // TODO: Implement proper password hashing verification (BCrypt, Argon2, etc.)
            // For now, direct comparison (not recommended for production)
            return user.UsrPass == password;
        }

        public async Task<int> IncrementFailedAttemptsAsync(string usrId)
        {
            var user = await GetByIdAsync(usrId);
            if (user == null)
                return 0;

            user.UsrTentativi++;
            
            // Auto-block if exceeds max attempts
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

            // TODO: Implement proper password hashing (BCrypt, Argon2, etc.)
            user.UsrPass = newPassword;
            user.UsrChgPas = false; // Reset password change flag
            user.UsrTentativi = 0; // Reset failed attempts
            await _context.SaveChangesAsync();
        }

        public async Task<bool> IsPasswordInHistoryAsync(string usrId, string password, int historyCount)
        {
            // TODO: Implement password history table and check
            // For now, just check current password
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
    }
}
