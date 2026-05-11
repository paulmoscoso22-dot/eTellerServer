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
        public virtual DbSet<CurrencyCouple> CurrencyCouple { get; set; }
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
        public virtual DbSet<Na_TabellaServInt> Na_TabellaServInt { get; set; }
        public virtual DbSet<USERS_AllAccess> OperationTypes { get; set; }
        public virtual DbSet<Trace> Traces { get; set; }
        public virtual DbSet<Personalisation> Personalisation { get; set; }
        public virtual DbSet<eTeller.Domain.Models.StoredProcedure.UserSelectRole> UserSelectRole { get; set; }
        public virtual DbSet<FunctionRole> FunctionRole { get; set; }
        public virtual DbSet<StFunAcctyp> StFunAcctyp { get; set; }
        public virtual DbSet<UserRole> UserRole { get; set; }
        public virtual DbSet<FUNZIONISCEDULE> FUNZIONISCEDULE { get; set; }
        public virtual DbSet<Device> Devices { get; set; }
        public virtual DbSet<ClientDevice> ClientDevices { get; set; }
        public virtual DbSet<DeviceType> DeviceTypes { get; set; }
        public virtual DbSet<StBookingRc> StBookingRc { get; set; }
        public virtual DbSet<StAccountType> StAccountType { get; set; }
        public virtual DbSet<StForceCode> StForceCodes { get; set; }
        public virtual DbSet<ST_PERIODICITA> ST_PERIODICITA { get; set; }
        public virtual DbSet<SERVIZI> SERVIZI { get; set; }
        public virtual DbSet<InformationSchemaTable> InformationSchemaTables { get; set; }

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
            modelBuilder.Entity<CurrencyCouple>().HasKey(c => new { c.CucCur1, c.CucCur2 });
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
            modelBuilder.Entity<Na_TabellaServInt>().HasNoKey();
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

            modelBuilder.Entity<FUNZIONISCEDULE>().ToTable("FUNZIONISHEDULE").HasKey(f => f.FutId);

            // Vista di sistema — sola lettura, usata per controlli di esistenza tabella
            modelBuilder.Entity<InformationSchemaTable>().HasNoKey().ToView("TABLES", "INFORMATION_SCHEMA");

            modelBuilder.Entity<StBookingRc>().ToTable("ST_BOOKING_RC").HasKey(x => new { x.BrcCutId, x.BrcOptId, x.BrcActId });
            modelBuilder.Entity<StAccountType>().ToTable("ST_ACCOUNTTYPE").HasKey(x => x.ActId);

            // Configure StForceCode entity with primary key
            modelBuilder.Entity<StForceCode>().HasKey(f => f.FocId);

            modelBuilder.Entity<ST_PERIODICITA>().ToTable("ST_PERIODICITA").HasKey(p => p.PeriodicId);

            modelBuilder.Entity<SERVIZI>().ToTable("SERVIZI").HasKey(s => s.SerId);

            modelBuilder.Entity<Device>().HasKey(d => d.DevId);
            modelBuilder.Entity<ClientDevice>().HasKey(cd => new { cd.CliId, cd.DevId });
            modelBuilder.Entity<DeviceType>().HasKey(dt => dt.DtyId);

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
