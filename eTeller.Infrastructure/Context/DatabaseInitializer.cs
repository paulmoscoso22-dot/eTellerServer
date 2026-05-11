using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace eTeller.Infrastructure.Context
{
    /// <summary>
    /// Hosted service che verifica e crea le tabelle nuove non presenti nel DB legacy all'avvio.
    /// Opera in modalità idempotente (IF NOT EXISTS) — sicuro da eseguire su ogni restart.
    /// </summary>
    public class DatabaseInitializer : IHostedService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<DatabaseInitializer> _logger;

        public DatabaseInitializer(IServiceScopeFactory scopeFactory, ILogger<DatabaseInitializer> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<eTellerDbContext>();

            await EnsureUserSessionsTableAsync(context, cancellationToken);
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

        private async Task EnsureUserSessionsTableAsync(eTellerDbContext context, CancellationToken cancellationToken)
        {
            try
            {
                bool exists = await TableExistsAsync(context, "dbo", "sys_USER_SESSIONS", cancellationToken);

                if (exists)
                {
                    _logger.LogDebug("sys_USER_SESSIONS: tabella già presente — nessuna azione necessaria.");
                    return;
                }

                await CreateUserSessionsTableAsync(context, cancellationToken);
                _logger.LogInformation("sys_USER_SESSIONS: tabella creata con successo.");
            }
            catch (SqlException ex) when (ex.Number == 2714)
            {
                // Race condition TOCTOU: un'altra istanza ha creato la tabella tra il check e la CREATE.
                // L'errore 2714 di SQL Server significa "oggetto già esistente" — non è un errore reale.
                _logger.LogWarning("sys_USER_SESSIONS: tabella già creata da un'altra istanza (race condition gestita).");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "sys_USER_SESSIONS: errore durante la verifica/creazione della tabella.");
                throw;
            }
        }

        /// <summary>
        /// Verifica l'esistenza di una tabella tramite LINQ su InformationSchemaTable —
        /// EF Core traduce il predicato in una query parametrizzata, nessun raw SQL.
        /// </summary>
        private static Task<bool> TableExistsAsync(
            eTellerDbContext context,
            string schema,
            string tableName,
            CancellationToken cancellationToken)
        {
            return context.InformationSchemaTables
                .AnyAsync(
                    t => t.TABLE_SCHEMA == schema && t.TABLE_NAME == tableName,
                    cancellationToken);
        }

        /// <summary>
        /// Crea la tabella sys_USER_SESSIONS in modo atomico.
        /// Le 3 istruzioni DDL (CREATE TABLE + 2 indici) sono avvolte in una transazione:
        /// se anche solo un indice fallisce, il rollback riporta il DB allo stato pulito
        /// e il restart successivo riprova la creazione completa.
        /// Il DDL usa esclusivamente costanti hardcoded — nessun input esterno concatenato.
        /// </summary>
        private static async Task CreateUserSessionsTableAsync(eTellerDbContext context, CancellationToken cancellationToken)
        {
            await using var transaction = await context.Database.BeginTransactionAsync(cancellationToken);

            await context.Database.ExecuteSqlRawAsync("""
                CREATE TABLE [dbo].[sys_USER_SESSIONS] (
                    [SESSION_ID]    INT           IDENTITY(1,1)  NOT NULL,
                    [USR_ID]        NVARCHAR(50)  NOT NULL,
                    [CLI_ID]        NVARCHAR(50)  NULL,
                    [IP_ADDRESS]    NVARCHAR(50)  NOT NULL,
                    [LOGIN_TIME]    DATETIME      NOT NULL,
                    [LAST_ACTIVITY] DATETIME      NOT NULL,
                    [IS_ACTIVE]     BIT           NOT NULL  CONSTRAINT [DF_sys_USER_SESSIONS_IS_ACTIVE]    DEFAULT (1),
                    [FORCED_LOGIN]  BIT           NOT NULL  CONSTRAINT [DF_sys_USER_SESSIONS_FORCED_LOGIN] DEFAULT (0),
                    CONSTRAINT [PK_sys_USER_SESSIONS] PRIMARY KEY CLUSTERED ([SESSION_ID] ASC)
                )
                """, cancellationToken);

            await context.Database.ExecuteSqlRawAsync("""
                CREATE NONCLUSTERED INDEX [IX_sys_USER_SESSIONS_USR_ID_IsActive]
                    ON [dbo].[sys_USER_SESSIONS] ([USR_ID] ASC, [IS_ACTIVE] ASC)
                """, cancellationToken);

            await context.Database.ExecuteSqlRawAsync("""
                CREATE NONCLUSTERED INDEX [IX_sys_USER_SESSIONS_IP_IsActive]
                    ON [dbo].[sys_USER_SESSIONS] ([IP_ADDRESS] ASC, [IS_ACTIVE] ASC)
                """, cancellationToken);

            await transaction.CommitAsync(cancellationToken);
        }
    }
}
