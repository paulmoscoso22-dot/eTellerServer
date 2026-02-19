using eTeller.Domain.Models;
using eTeller.Domain.Models.StoredProcedure;
using eTeller.Domain.Models.View;
using Microsoft.EntityFrameworkCore;

namespace eTeller.Infrastructure.Context
{
    public class eTellerDbContext : DbContext
    {
        public virtual DbSet<Account> Account { get; set; }
        public virtual DbSet<AntirecAppearerView> AntirecAppearerView { get; set; }
        public virtual DbSet<Transaction> Transaction { get; set; }
        public virtual DbSet<TransactionMov> TransactionMov { get; set; }
        public virtual DbSet<GiornaleAntiriciclaggio> GiornaleAntiriciclaggio { get; set; }
        public virtual DbSet<Currency> Currency { get; set; }
        public virtual DbSet<Branch> Branch { get; set; }
        public virtual DbSet<ST_CurrencyType> ST_CurrencyType { get; set; }
        public virtual DbSet<ST_OperationType> ST_OperationType { get; set; }
        public virtual DbSet<TotalicCassa> TotalicCassa { get; set; }
        public eTellerDbContext(DbContextOptions options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Account>().HasNoKey();
            modelBuilder.Entity<AntirecAppearerView>().HasNoKey();
            modelBuilder.Entity<Transaction>().HasNoKey();
            modelBuilder.Entity<TransactionMov>().HasNoKey();
            modelBuilder.Entity<Currency>().HasNoKey();
            modelBuilder.Entity<Branch>().HasNoKey();
            modelBuilder.Entity<ST_CurrencyType>().HasNoKey();
            modelBuilder.Entity<ST_OperationType>().HasNoKey();
            modelBuilder.Entity<TotalicCassa>().HasNoKey();
            //modelBuilder.Entity<GiornaleAntiriciclaggio>().HasNoKey();
        }
    }
}

