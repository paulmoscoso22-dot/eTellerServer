using eTeller.Application.Contracts;
using eTeller.Domain.Models;
using eTeller.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace eTeller.Infrastructure.Repositories.User
{
    public class UserSessionRepository : BaseSimpleRepository<UserSession>, IUserSessionRepository
    {
        public UserSessionRepository(eTellerDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<UserSession?> GetActiveSessionByUserIdAsync(string usrId)
        {
            return await _context.UserSessions
                .Where(s => s.UsrId == usrId && s.IsActive)
                .OrderByDescending(s => s.LoginTime)
                .FirstOrDefaultAsync();
        }

        public async Task<UserSession?> GetActiveSessionByIpAsync(string ipAddress)
        {
            return await _context.UserSessions
                .Where(s => s.IpAddress == ipAddress && s.IsActive)
                .OrderByDescending(s => s.LoginTime)
                .FirstOrDefaultAsync();
        }

        public async Task<bool> IsUserLoggedAsync(string usrId)
        {
            return await _context.UserSessions
                .AnyAsync(s => s.UsrId == usrId && s.IsActive);
        }

        public async Task<bool> IsUserLoggedOnIpAsync(string ipAddress, string usrId)
        {
            return await _context.UserSessions
                .AnyAsync(s => s.IpAddress == ipAddress && s.UsrId == usrId && s.IsActive);
        }

        public async Task<UserSession?> CreateSessionAsync(string usrId, string? cliId, string ipAddress, bool forcedLogin)
        {
            // Terminate existing sessions if forced login
            if (forcedLogin)
            {
                await TerminateSessionAsync(usrId);
            }

            var session = new UserSession
            {
                UsrId = usrId,
                CliId = cliId,
                IpAddress = ipAddress,
                LoginTime = DateTime.Now,
                LastActivity = DateTime.Now,
                IsActive = true,
                ForcedLogin = forcedLogin
            };

            _context.UserSessions.Add(session);
            await _context.SaveChangesAsync();

            return session;
        }

        public async Task<bool> TerminateSessionAsync(string usrId)
        {
            var sessions = await _context.UserSessions
                .Where(s => s.UsrId == usrId && s.IsActive)
                .ToListAsync();

            foreach (var session in sessions)
            {
                session.IsActive = false;
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> TerminateSessionByIdAsync(int sessionId)
        {
            var session = await _context.UserSessions
                .FirstOrDefaultAsync(s => s.SessionId == sessionId);

            if (session == null)
                return false;

            session.IsActive = false;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task UpdateLastActivityAsync(int sessionId)
        {
            var session = await _context.UserSessions
                .FirstOrDefaultAsync(s => s.SessionId == sessionId);

            if (session != null)
            {
                session.LastActivity = DateTime.Now;
                await _context.SaveChangesAsync();
            }
        }
    }
}
