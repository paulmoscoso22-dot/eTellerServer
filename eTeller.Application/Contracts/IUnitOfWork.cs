using eTeller.Application.Contracts.StoreProcedures;
using eTeller.Application.Contracts.StoreProcedures.AntirecAppearer;
using eTeller.Application.Contracts.StoreProcedures.Manager;
using eTeller.Application.Contracts.StoreProcedures.Operazioni.ContoCorrenti.Prelievo;
using eTeller.Application.Contracts.StoreProcedures.ST_CurrencyType;
using eTeller.Application.Contracts.StoreProcedures.Trace;
using eTeller.Application.Contracts.StoreProcedures.Vigilanza;
using eTeller.Application.Features.ContiCorrenti.Prelievo;
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
        IST_CurrencyTypeSpRepository ST_CurrencyTypeSpRepository { get; }
        ITraceSpRepository TraceSpRepository { get; }
        IUserRepository UserRepository { get; }
        IUserSessionRepository UserSessionRepository { get; }
        IClientRepository ClientRepository { get; }
        ICustomersSpRepository CustomersSpRepository { get; }
        ICustomerAccountSpRepository CustomerAccountSpRepository { get; }
        global::eTeller.Application.Contracts.StoreProcedures.Operazioni.ContoCorrenti.Prelievo.IErrorCodeRepository ErrorCodeRepository { get; }
        ITransazioneRepository TransazioneRepository { get; }
        IManagerSpRepository ManagerSpRepository { get; }
        IBaseSimpleRepository<TEntity> Repository<TEntity>() where TEntity : class;
        Task<int> Complete();
        Task Rollback();
    }
}
