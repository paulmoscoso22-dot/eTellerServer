using eTeller.Application.Contracts.StoreProcedures;
using eTeller.Application.Contracts.StoreProcedures.AntirecAppearer;
using eTeller.Application.Contracts.StoreProcedures.Vigilanza;
using static eTeller.Application.Contracts.Commons.IBaseSimpleRepository;

namespace eTeller.Application.Contracts
{
    public interface IUnitOfWork
    {
        IAccountSpRepository AccountSpRepository { get; }
        IAntirecAppearerSelectRepository AntirecAppearerSelectRepository { get; }
        ITransactionSpRepository TransactionSpRepository { get; }
        ITransactionMovSpRepository TransactionMovSpRepository { get; }
        IGiornaleAntiriciclaggioSpRepository GiornaleAntiriciclaggioSpRepository { get; }
        ITotalicCassaSpRepository TotalicCassaSpRepository { get; }
        IVigilanzaSpRepository VigilanzaSpRepository { get; }
        IBaseSimpleRepository<TEntity> Repository<TEntity>() where TEntity : class;
        Task<int> Complete();
    }
}
