using eTeller.Application.Contracts;
using eTeller.Application.Contracts.StoreProcedures;
using eTeller.Application.Contracts.StoreProcedures.AntirecAppearer;
using eTeller.Application.Contracts.StoreProcedures.Manager;
using eTeller.Application.Contracts.StoreProcedures.Trace;
using eTeller.Application.Contracts.StoreProcedures.Vigilanza;
using eTeller.Application.Features.ContiCorrenti.Prelievo;
using eTeller.Infrastructure.Context;
using eTeller.Infrastructure.Repositories.Archivi.Report.StoreProcedure;
using eTeller.Infrastructure.Repositories.Archivi.Ricerca.StoreProcedures;
using eTeller.Infrastructure.Repositories.StoreProcedures;
using eTeller.Infrastructure.Repositories.StoreProcedures.AntirecAppearer;
using eTeller.Infrastructure.Repositories.StoreProcedures.Manager;
using eTeller.Infrastructure.Repositories.StoreProcedures.Operazioni.ContoCorrenti.Prelievo;
using eTeller.Infrastructure.Repositories.StoreProcedures.Trace;
using eTeller.Infrastructure.Repositories.StoreProcedures.Vigilanza;
using eTeller.Application.Contracts.StoreProcedures.ST_CurrencyType;
using eTeller.Application.Contracts.StoreProcedures.Tabella;
using eTeller.Infrastructure.Repositories.StoreProcedures.ST_CurrencyType;
using eTeller.Infrastructure.Repositories.StoreProcedures.Tabella;
using eTeller.Infrastructure.Repositories.Archivi;
using System.Collections;
using static eTeller.Application.Contracts.Commons.IBaseSimpleRepository;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using eTeller.Infrastructure.Repositories.ContiCorrenti.Prelievo;

namespace eTeller.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ILogger<UnitOfWork> _logger;
        private Hashtable? _repositories;
        private readonly eTellerDbContext _context;
        private readonly IMemoryCache _cache;
        private readonly ILoggerFactory _loggerFactory;

        private IUserRepository? _userRepository;
        public IUserRepository UserRepository => _userRepository ??= new UserRepository(_context);

        private IUserSessionRepository? _userSessionRepository;
        public IUserSessionRepository UserSessionRepository => _userSessionRepository ??= new UserSessionRepository(_context);

        private IClientRepository? _clientRepository;
        public IClientRepository ClientRepository => _clientRepository ??= new ClientRepository(_context);

        private IAccountSpRepository? _accountSpRepository;
        public IAccountSpRepository AccountSpRepository => _accountSpRepository ??= new AccountSpRepository(_context);

        private IAntirecAppearerSelectRepository? _antirecAppearerSelectRepository;
        public IAntirecAppearerSelectRepository AntirecAppearerSelectRepository => _antirecAppearerSelectRepository ??= new AntirecAppearerSelectRepository(_context);

        private ITransactionSpRepository? _transactionSpRepository;
        public ITransactionSpRepository TransactionSpRepository => _transactionSpRepository ??= new TransactionSpRepository(
            _context, 
            _loggerFactory.CreateLogger<TransactionSpRepository>(),
            null!, // IHostCommunicationService - can be null for now
            null!); // IErrorCodeRepository - can be null for now

        private ITransactionMovSpRepository? _transactionMovSpRepository;
        public ITransactionMovSpRepository TransactionMovSpRepository => _transactionMovSpRepository ??= new TransactionMovSpRepository(_context);

        private IGiornaleAntiriciclaggioSpRepository? _giornaleAntiriciclaggioSpRepository;
        public IGiornaleAntiriciclaggioSpRepository GiornaleAntiriciclaggioSpRepository => _giornaleAntiriciclaggioSpRepository ??= new GiornaleAntiriciclaggioSpRepository(_context);

        private ITotalicCassaSpRepository? _totalicCassaSpRepository;
        public ITotalicCassaSpRepository TotalicCassaSpRepository => _totalicCassaSpRepository ??= new TotalicCassaSpRepository(_context);

        private IVigilanzaSpRepository? _vigilanzaSpRepository;
        public IVigilanzaSpRepository VigilanzaSpRepository => _vigilanzaSpRepository ??= new VigilanzaSpRepository(
            _context,
            _loggerFactory.CreateLogger<VigilanzaSpRepository>());

        private IST_CurrencyTypeSpRepository? _stCurrencyTypeSpRepository;
        public IST_CurrencyTypeSpRepository ST_CurrencyTypeSpRepository => _stCurrencyTypeSpRepository ??= new ST_CurrencyTypeSpRepository(_context);

        private ITabellaRepository? _tabellaRepository;
        public ITabellaRepository TabellaRepository => _tabellaRepository ??= new TabellaRepository(_context);

        private ITraceSpRepository? _traceSpRepository;
        public ITraceSpRepository TraceSpRepository => _traceSpRepository ??= new TraceSpRepository(_context);

        private ICustomersSpRepository? _customersSpRepository;
        public ICustomersSpRepository CustomersSpRepository => _customersSpRepository ??= new CustomersSpRepository(_context);

        private ICustomerAccountSpRepository? _customerAccountSpRepository;
        public ICustomerAccountSpRepository CustomerAccountSpRepository => _customerAccountSpRepository ??= new eTeller.Infrastructure.Repositories.StoreProcedures.CustomerAccount.CustomerAccountSpRepository(_context);

        private ITransazioneRepository? _transazioneRepository;
        public ITransazioneRepository TransazioneRepository => _transazioneRepository ??= new TransazioneRepository(_context);

        private IManagerSpRepository? _managerSpRepository;
        public IManagerSpRepository ManagerSpRepository => _managerSpRepository ??= new ManagerSpRepository(_context);

        private Application.Contracts.StoreProcedures.Operazioni.ContoCorrenti.Prelievo.IErrorCodeRepository? _errorCodeRepository;
        public Application.Contracts.StoreProcedures.Operazioni.ContoCorrenti.Prelievo.IErrorCodeRepository ErrorCodeRepository => _errorCodeRepository ??= new ErrorCodeRepository(
            _context, 
            _cache, 
            _loggerFactory.CreateLogger<ErrorCodeRepository>());

        public UnitOfWork(eTellerDbContext context, ILogger<UnitOfWork> logger, IMemoryCache cache, ILoggerFactory loggerFactory)
        {
            _context = context;
            _logger = logger;
            _cache = cache;
            _loggerFactory = loggerFactory;
        }

        public async Task<int> Complete()
        {
            return await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Rollback all pending transactions and changes in the current context
        /// </summary>
        public async Task Rollback()
        {
            try
            {
                var changedEntries = _context.ChangeTracker.Entries()
                    .Where(x => x.State != EntityState.Unchanged)
                    .ToList();

                foreach (var entry in changedEntries)
                {
                    switch (entry.State)
                    {
                        case EntityState.Modified:
                        case EntityState.Added:
                            entry.State = EntityState.Detached;
                            break;
                        case EntityState.Deleted:
                            entry.Reload();
                            break;
                    }
                }

                _logger?.LogInformation("Transaction rollback completed successfully");
                await Task.CompletedTask;
            }
            catch (Exception ex)
            {
                _logger?.LogError($"Error during transaction rollback: {ex.Message}", ex);
                throw;
            }
        }

        public void Dispose()
        {
            _context.Dispose();
        }

        public IBaseSimpleRepository<TEntity> Repository<TEntity>() where TEntity : class
        {
            _repositories ??= new Hashtable();

            var type = typeof(TEntity).Name;

            if (!_repositories.ContainsKey(type))
            {
                var repositoryType = typeof(BaseSimpleRepository<>);

                var repositoryInstance = Activator.CreateInstance(repositoryType.MakeGenericType(typeof(TEntity)), _context);
                _repositories.Add(type, repositoryInstance!);
            }
            return (IBaseSimpleRepository<TEntity>)_repositories[type]!;
        }
    }
}
