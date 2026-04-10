using eTeller.Application.Contracts;
using eTeller.Application.Contracts.AntirecAppearer;
using eTeller.Application.Contracts.Manager;
using eTeller.Application.Contracts.Personalisation;
using eTeller.Application.Contracts.Trace;
using eTeller.Application.Contracts.Vigilanza;
using eTeller.Infrastructure.Context;
using eTeller.Infrastructure.Repositories.StoreProcedures;
using eTeller.Infrastructure.Repositories.StoreProcedures.AntirecAppearer;
using eTeller.Infrastructure.Repositories.StoreProcedures.Manager;
using eTeller.Infrastructure.Repositories.StoreProcedures.Operazioni.ContoCorrenti.Prelievo;
using eTeller.Infrastructure.Repositories.StoreProcedures.Personalisation;
using eTeller.Infrastructure.Repositories.StoreProcedures.Trace;
using eTeller.Infrastructure.Repositories.StoreProcedures.Vigilanza;
using eTeller.Application.Contracts.ST_CurrencyType;
using eTeller.Application.Contracts.Tabella;
using eTeller.Infrastructure.Repositories.StoreProcedures.ST_CurrencyType;
using eTeller.Infrastructure.Repositories.StoreProcedures.Tabella;
using System.Collections;
using static eTeller.Application.Contracts.Commons.IBaseSimpleRepository;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using CustomersSpRepo = eTeller.Infrastructure.Repositories.StoreProcedures.CustomersRepository;
using TotalicCassaSpRepo = eTeller.Infrastructure.Repositories.StoreProcedures.TotalicCassaRepository;
using eTeller.Infrastructure.Repositories.User;
using eTeller.Infrastructure.Repositories.StoreProcedures.Client;
using eTeller.Infrastructure.Repositories.Transaction;
using eTeller.Infrastructure.Repositories.GiornaleAntiriciclaggio;

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
        //public IClientRepository ClientRepository => _clientRepository ??= new ClientRepository(_context);

        private IAccountRepository? _accountRepository;
        public IAccountRepository AccountRepository => _accountRepository ??= new AccountRepository(_context);

        private IAntirecAppearerSelectRepository? _antirecAppearerSelectRepository;
        public IAntirecAppearerSelectRepository AntirecAppearerSelectRepository => _antirecAppearerSelectRepository ??= new AntirecAppearerSelectSpRepository(_context);

        //private ITransactioneRepository? _transactionRepository;
        //public ITransactionRepository TransactionRepository => _transactionRepository ??= new TransactionRepository(
        //    _context, 
        //    _loggerFactory.CreateLogger<TransactionSpRepository>(),
            //null!, // IHostCommunicationService - can be null for now
            //null!); // IErrorCodeRepository - can be null for now

        private ITransactionMovRepository? _transactionMovRepository;
        public ITransactionMovRepository TransactionMovRepository => _transactionMovRepository ??= new TransactionMovRepository(_context);

        private IGiornaleAntiriciclaggioRepository? _giornaleAntiriciclaggioRepository;
        public IGiornaleAntiriciclaggioRepository GiornaleAntiriciclaggioRepository => _giornaleAntiriciclaggioRepository ??= new GiornaleAntiriciclaggioSpRepository(_context);

        private ITotalicCassaRepository? _totalicCassaRepository;
        public ITotalicCassaRepository TotalicCassaRepository => _totalicCassaRepository ??= new TotalicCassaSpRepo(_context);

        private IVigilanzaRepository? _vigilanzaRepository;
        public IVigilanzaRepository VigilanzaRepository => _vigilanzaRepository ??= new VigilanzaSpRepository(
            _context,
            _loggerFactory.CreateLogger<VigilanzaSpRepository>());

        private IST_CurrencyTypeRepository? _stCurrencyTypeRepository;
        public IST_CurrencyTypeRepository ST_CurrencyTypeRepository => _stCurrencyTypeRepository ??= new ST_CurrencyTypeRepository(_context);

        private ITabellaRepository? _tabellaRepository;
        public ITabellaRepository TabellaRepository => _tabellaRepository ??= new TabellaRepository(_context);

        private ITraceRepository? _traceRepository;
        public ITraceRepository TraceRepository => _traceRepository ??= new TraceRepository(_context);

        private ICustomersRepository? _customersRepository;
        public ICustomersRepository CustomersRepository => _customersRepository ??= new CustomersSpRepo(_context);

        private ICustomerAccountRepository? _customerAccountRepository;
        public ICustomerAccountRepository CustomerAccountRepository => _customerAccountRepository ??= new eTeller.Infrastructure.Repositories.StoreProcedures.CustomerAccount.CustomerAccountRepository(_context);

        private ITransazioneRepository? _transazioneRepository;
        public ITransazioneRepository TransazioneRepository => _transazioneRepository ??= new TransazioneRepository(_context);

        private IManagerRepository? _managerRepository;
        public IManagerRepository ManagerRepository => _managerRepository ??= new ManagerRepository(_context);

        private IPersonalisationRepository? _personalisationRepository;
        public IPersonalisationRepository PersonalisationRepository => _personalisationRepository ??= new PersonalisationRepository(_context);

        private Application.Contracts.Operazioni.ContoCorrenti.Prelievo.IErrorCodeRepository? _errorCodeRepository;
        public Application.Contracts.Operazioni.ContoCorrenti.Prelievo.IErrorCodeRepository ErrorCodeRepository => _errorCodeRepository ??= new ErrorCodeRepository(
            _context, 
            _cache, 
            _loggerFactory.CreateLogger<ErrorCodeRepository>());

        public IClientRepository ClientRepository => throw new NotImplementedException();

        public UnitOfWork(eTellerDbContext context, ILogger<UnitOfWork> logger, IMemoryCache cache, ILoggerFactory loggerFactory)
        {
            _context = context;
            _logger = logger;
            _cache = cache;
            _loggerFactory = loggerFactory;
        }

        public async Task<int> Complete()
        {
            try
            {
                return await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during Complete, initiating rollback");
                await Rollback();
                throw;
            }
        }

        public async Task Rollback()
        {
            try
            {
                _logger.LogInformation("Initiating transaction rollback for all repositories");

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

                if (_context.Database.CurrentTransaction != null)
                {
                    await _context.Database.CurrentTransaction.RollbackAsync();
                    _logger.LogInformation("Database transaction rolled back");
                }


                _logger?.LogInformation("Transaction rollback completed successfully for all repositories");
                await Task.CompletedTask;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error during transaction rollback");
                throw;
            }
        }

        public void Dispose()
        {
            _context.Dispose();
        }

        public async Task BeginTransactionAsync()
        {
            await _context.Database.BeginTransactionAsync();
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

        public async Task CommitAsync()
        {
            try
            {
                if (_context.Database.CurrentTransaction != null)
                {
                    await _context.Database.CurrentTransaction.CommitAsync();
                    _logger.LogInformation("Transaction committed successfully");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during CommitAsync");
                await Rollback();
                throw;
            }
        }
    }
}
