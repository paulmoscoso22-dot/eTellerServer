using eTeller.Application.Contracts;
using eTeller.Application.Contracts.StoreProcedures;
using eTeller.Application.Contracts.StoreProcedures.AntirecAppearer;
using eTeller.Infrastructure.Context;
using eTeller.Infrastructure.Repositories.StoreProcedures;
using eTeller.Infrastructure.Repositories.StoreProcedures.AntirecAppearer;
using System.Collections;
using static eTeller.Application.Contracts.Commons.IBaseSimpleRepository;

namespace eTeller.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private Hashtable _repositories;
        private readonly eTellerDbContext _context;

        private IAccountSpRepository _accountSpRepository;
        public IAccountSpRepository AccountSpRepository => _accountSpRepository ??= new AccountSpRepository(_context);


        private IAntirecAppearerSelectRepository _antirecAppearerSelectRepository;
        public IAntirecAppearerSelectRepository AntirecAppearerSelectRepository => _antirecAppearerSelectRepository ??= new AntirecAppearerSelectRepository(_context);


        private ITransactionSpRepository _transactionSpRepository;
        public ITransactionSpRepository TransactionSpRepository => _transactionSpRepository ??= new TransactionSpRepository(_context);

        private ITransactionMovSpRepository _transactionMovSpRepository;
        public ITransactionMovSpRepository TransactionMovSpRepository => _transactionMovSpRepository ??= new TransactionMovSpRepository(_context);

        private IGiornaleAntiriciclaggioSpRepository _giornaleAntiriciclaggioSpRepository;
        public IGiornaleAntiriciclaggioSpRepository GiornaleAntiriciclaggioSpRepository => _giornaleAntiriciclaggioSpRepository ??= new GiornaleAntiriciclaggioSpRepository(_context);

        private ITotalicCassaSpRepository _totalicCassaSpRepository;
        public ITotalicCassaSpRepository TotalicCassaSpRepository => _totalicCassaSpRepository ??= new TotalicCassaSpRepository(_context);


        public UnitOfWork(eTellerDbContext context)
        {
            _context = context;
        }

        public async Task<int> Complete()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }

        public IBaseSimpleRepository<TEntity> Repository<TEntity>() where TEntity : class
        {
            if (_repositories == null)
            {
                _repositories = new Hashtable();
            }

            var type = typeof(TEntity).Name;

            if (!_repositories.ContainsKey(type))
            {
                var repositoryType = typeof(BaseSimpleRepository<>);

                var repositoryInstance = Activator.CreateInstance(repositoryType.MakeGenericType(typeof(TEntity)), _context);
                _repositories.Add(type, repositoryInstance);
            }
            return (IBaseSimpleRepository<TEntity>)_repositories[type];
        }
    }
}
