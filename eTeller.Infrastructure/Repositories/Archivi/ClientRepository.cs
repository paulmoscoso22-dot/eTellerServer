using eTeller.Application.Contracts;
using eTeller.Domain.Models;
using eTeller.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace eTeller.Infrastructure.Repositories.Archivi
{
    public class ClientRepository : BaseSimpleRepository<Client>, IClientRepository
    {
        public ClientRepository(eTellerDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<Client?> WhoIsLogged(string ip)
        {
            return await _context.Clients
                .FirstOrDefaultAsync(c => c.CliIp == ip);
        }
    }
}
