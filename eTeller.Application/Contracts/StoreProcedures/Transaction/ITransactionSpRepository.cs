using eTeller.Domain.Models;
using eTeller.Application.DTOs;
using static eTeller.Application.Contracts.Commons.IBaseSimpleRepository;

namespace eTeller.Application.Contracts.StoreProcedures
{
    public interface ITransactionSpRepository : IBaseSimpleRepository<Transaction>
    {
        Task<IEnumerable<Transaction>> GetSpTransactionWaitingForBef(string trxCassa, DateTime trxDataDal, DateTime trxDataAl, int trxStatus, string trxBraId);

        Task<IEnumerable<Transaction>> GetSpTransactionWithFiltersForGiornale(string trxCassa, DateTime trxDataDal, DateTime trxDataAl, int trxStatus, string trxBraId);

        Task<IEnumerable<Transaction>> GetSpTransactionWithFilters(string trxCassa, DateTime trxDataDal, DateTime trxDataAl, int trxStatus, string trxBraId);

        /// <summary>
        /// Verifica la validità della transazione prima del completamento.
        /// Controlla regole di vigilanza, beneficiario obbligatorio e altre validazioni.
        /// </summary>
        /// <param name="trxId">ID della transazione da verificare</param>
        /// <param name="tipoOperazione">"DEP" (deposito) | "WITH" (prelievo)</param>
        /// <param name="ct">Token di cancellazione</param>
        /// <returns>Risultato della verifica con flag e messaggi di errore</returns>
        Task<VerificaTransazioneResult> VerificaTransazioneAsync(
            int trxId,
            string tipoOperazione,
            CancellationToken ct = default);
    }
}
