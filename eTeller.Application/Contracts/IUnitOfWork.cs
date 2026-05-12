using eTeller.Application.Contracts;
using eTeller.Application.Contracts.AntirecAppearer;
using eTeller.Application.Contracts.Corsi;
using eTeller.Application.Contracts.CurrencyCouple;
using eTeller.Application.Contracts.ForceTrx;
using eTeller.Application.Contracts.Help;
using eTeller.Application.Contracts.Manager;
using eTeller.Application.Contracts.Operazioni.ContoCorrenti.Prelievo;
using eTeller.Application.Contracts.Personalisation;
using eTeller.Application.Contracts.ST_CurrencyType;
using eTeller.Application.Contracts.Tabella;
using eTeller.Application.Contracts.Trace;
using eTeller.Application.Contracts.Vigilanza;
using static eTeller.Application.Contracts.Commons.IBaseSimpleRepository;

namespace eTeller.Application.Contracts
{
    public interface IUnitOfWork
    {
        IAccountRepository AccountRepository { get; }
        IAntirecAppearerSelectRepository AntirecAppearerSelectRepository { get; }
        ITransazioneRepository TransazioneRepository { get; }
        ITransactionMovRepository TransactionMovRepository { get; }
        IGiornaleAntiriciclaggioRepository GiornaleAntiriciclaggioRepository { get; }
        ITotalicCassaRepository TotalicCassaRepository { get; }
        IVigilanzaRepository VigilanzaRepository { get; }
        IST_CurrencyTypeRepository ST_CurrencyTypeRepository { get; }
        ITabellaRepository TabellaRepository { get; }
        ITraceRepository TraceRepository { get; }
        IUserRepository UserRepository { get; }
        IUserSessionRepository UserSessionRepository { get; }
        IClientRepository ClientRepository { get; }
        ICustomersRepository CustomersRepository { get; }
        ICustomerAccountRepository CustomerAccountRepository { get; }
        Operazioni.ContoCorrenti.Prelievo.IErrorCodeRepository ErrorCodeRepository { get; }
        IManagerRepository ManagerRepository { get; }
        IPersonalisationRepository PersonalisationRepository { get; }
        ICurrencyRepository CurrencyRepository { get; }
        ICurrencyCoupleRepository CurrencyCoupleRepository { get; }
        ICorsiRepository CorsiRepository { get; }
        IForceTrxRepository ForceTrxRepository { get; }
        IHelpInfoRepository HelpInfoRepository { get; }
        IBaseSimpleRepository<TEntity> Repository<TEntity>() where TEntity : class;
        Task BeginTransactionAsync();
        Task<int> Complete();

        Task CommitAsync();
        Task Rollback();
    }
}
