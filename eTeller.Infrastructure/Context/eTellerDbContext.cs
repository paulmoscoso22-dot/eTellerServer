using eTeller.Domain.Models;
using eTeller.Domain.Models.View;
using Microsoft.EntityFrameworkCore;

namespace eTeller.Infrastructure.Context
{
    public class eTellerDbContext : DbContext
    {
        public virtual DbSet<Account> Account { get; set; }
        public virtual DbSet<AntirecAppearerView> AntirecAppearerView { get; set; }
        public eTellerDbContext(DbContextOptions options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Account>().HasNoKey();
            modelBuilder.Entity<AntirecAppearerView>().HasNoKey();
        }
    }
}
