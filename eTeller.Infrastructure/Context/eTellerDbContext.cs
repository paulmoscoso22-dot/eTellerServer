using eTeller.Domain.Models;
using eTeller.Domain.Models.StoredProcedure;
using eTeller.Domain.Models.View;
using Microsoft.EntityFrameworkCore;
using static Dapper.SqlMapper;

namespace eTeller.Infrastructure.Context
{
    public class eTellerDbContext : DbContext
    {
        public virtual DbSet<Account> Account { get; set; }
        public virtual DbSet<CustomerAccount> CustomerAccount { get; set; }
        public virtual DbSet<Customers> Customers { get; set; }
        public virtual DbSet<AntirecAppearerView> AntirecAppearerView { get; set; }
        public virtual DbSet<Transaction> Transaction { get; set; }
        public virtual DbSet<TransactionMov> TransactionMov { get; set; }
        public virtual DbSet<GiornaleAntiriciclaggio> GiornaleAntiriciclaggio { get; set; }
        public virtual DbSet<Currency> Currency { get; set; }
        public virtual DbSet<Branch> Branch { get; set; }
        public virtual DbSet<ST_Categories> ST_Categories { get; set; }
        public virtual DbSet<ST_CurrencyType> ST_CurrencyType { get; set; }
        public virtual DbSet<ST_OperationType> ST_OperationType { get; set; }
        public virtual DbSet<ST_COUNTRY> ST_COUNTRY { get; set; }
        public virtual DbSet<ST_LANGUAGE> ST_LANGUAGE { get; set; }
        public virtual DbSet<ST_STATOENTITA> ST_STATOENTITA { get; set; }
        public virtual DbSet<TotalicCassa> TotalicCassa { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<UserSession> UserSessions { get; set; }
        public virtual DbSet<Client> Clients { get; set; }
        public virtual DbSet<ErrorCode> ErrorCodes { get; set; }
        public virtual DbSet<Antirecycling> AntiRecyclings { get; set; }
        public virtual DbSet<InfoAutorizzazioneUtente> InfoAutorizzazioneUtente { get; set; }
        public virtual DbSet<SysFunctions> SysFunctions { get; set; }
        public virtual DbSet<sys_ROLE> SysRoles { get; set; }
        public virtual DbSet<UsersRoleFunction> UsersRoleFunction { get; set; }
        public virtual DbSet<ST_TRACE_FUNCTION> ST_TRACE_FUNCTION { get; set; }
        public virtual DbSet<SysUsersUseClient> SysUsersUseClients { get; set; }
        public virtual DbSet<Na_TabellaServVarchar> Na_TabellaServVarchar { get; set; }
        public virtual DbSet<USERS_AllAccess> OperationTypes { get; set; }
        public virtual DbSet<Trace> Traces { get; set; }
        public virtual DbSet<Personalisation> Personalisation { get; set; }
        public virtual DbSet<eTeller.Domain.Models.StoredProcedure.UserSelectRole> UserSelectRole { get; set; }
        public virtual DbSet<FunctionRole> FunctionRole { get; set; }
        public virtual DbSet<StFunAcctyp> StFunAcctyp { get; set; }
        public virtual DbSet<UserRole> UserRole { get; set; }

        public eTellerDbContext(DbContextOptions options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Account>().HasNoKey();
            modelBuilder.Entity<CustomerAccount>().HasNoKey();
            modelBuilder.Entity<Customers>().HasNoKey();
            modelBuilder.Entity<AntirecAppearerView>().HasNoKey();
            modelBuilder.Entity<Transaction>().HasNoKey();
            modelBuilder.Entity<TransactionMov>().HasNoKey();
            modelBuilder.Entity<Currency>().HasNoKey();
            modelBuilder.Entity<Branch>().HasNoKey();
            modelBuilder.Entity<ST_Categories>().HasNoKey();
            modelBuilder.Entity<ST_CurrencyType>().HasNoKey();
            modelBuilder.Entity<ST_OperationType>().HasNoKey();
            modelBuilder.Entity<ST_COUNTRY>().HasNoKey();
            modelBuilder.Entity<ST_LANGUAGE>().HasNoKey();
            modelBuilder.Entity<ST_STATOENTITA>().HasNoKey();
            modelBuilder.Entity<TotalicCassa>().HasNoKey();
            modelBuilder.Entity<InfoAutorizzazioneUtente>().HasNoKey();
            modelBuilder.Entity<SysFunctions>().HasNoKey();
            modelBuilder.Entity<UsersRoleFunction>().HasNoKey();
            modelBuilder.Entity<ST_TRACE_FUNCTION>().HasNoKey();
            modelBuilder.Entity<Na_TabellaServVarchar>().HasNoKey();
            modelBuilder.Entity<Trace>().HasNoKey();
            // Configure sys_ROLE entity with primary key
            modelBuilder.Entity<sys_ROLE>().HasKey(r => r.RoleId);
            modelBuilder.Entity<USERS_AllAccess>().HasNoKey();

            // Configure User entity with primary key
            modelBuilder.Entity<User>().HasKey(u => u.UsrId);
            
            // Configure UserSession entity with primary key
            modelBuilder.Entity<UserSession>().HasKey(s => s.SessionId);
            
            // Configure Client entity with primary key
            modelBuilder.Entity<Client>().HasKey(c => c.CliId);

            // Configure ErrorCode entity with primary key
            modelBuilder.Entity<ErrorCode>().HasKey(e => e.ErrId);

            // Configure Antirecycling entity with primary keyu
            modelBuilder.Entity<Antirecycling>().HasKey(a => a.ArcId);

            // Configure Personalisation entity with primary key
            modelBuilder.Entity<Personalisation>().HasKey(p => p.ParId);

            // Configure UserSelectRole entity
            modelBuilder.Entity<eTeller.Domain.Models.StoredProcedure.UserSelectRole>().HasNoKey();

            // Configure FunctionRole entity
            modelBuilder.Entity<FunctionRole>().HasNoKey();

            // Configure StFunAcctyp entity
            modelBuilder.Entity<StFunAcctyp>().HasNoKey();

            //UserRole entity configuration
            modelBuilder.Entity<UserRole>().HasKey(e => new { e.UserId, e.RoleId });

            // Colonna USER_ID
            modelBuilder.Entity<UserRole>().Property(e => e.UserId)
                .HasColumnName("USER_ID")
                .HasMaxLength(20)
                .IsRequired();

            // Colonna ROLE_ID
            modelBuilder.Entity<UserRole>().Property(e => e.RoleId)
                .HasColumnName("ROLE_ID")
                .IsRequired();
            //fine user role configuration

        }
    }
}
