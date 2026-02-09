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
