using eTeller.Domain.Models;
using static eTeller.Application.Contracts.Commons.IBaseSimpleRepository;

namespace eTeller.Application.Contracts
{
    public interface ITransactionMovRepository : IBaseSimpleRepository<TransactionMov>
    {
        Task<IEnumerable<TransactionMov>> GetTransactionMovByTrxId(int trxId);

        Task InsertMovimentoAsync(
        int        transactionId,
        string     tipoMovimento,   // "CTP" | "CASSA" | "AGIO"
        string     filiale,
        string     tipoAccount,
        string     account,
        string     divisa,
        decimal    importo,
        decimal    importoCtv,
        DateTime   dataValuta,
        string?    text1,
        string?    text2,
        string     codiceCausale,   // da BookingRC
        string     hostCod,         // da CurrencyDB
        bool       updatePosition,  // da UpdatePosition
        CancellationToken ct = default);

    Task DeleteByTrxIdAsync(
        int transactionId,
        CancellationToken ct = default);
    }
}
