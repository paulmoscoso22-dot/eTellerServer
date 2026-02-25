using eTeller.Domain.Models.StoredProcedure;
using static eTeller.Application.Contracts.Commons.IBaseSimpleRepository;

namespace eTeller.Application.Contracts.StoreProcedures.Vigilanza
{
    public interface IVigilanzaSpRepository : IBaseSimpleRepository<SpTransactionGiornaleAntiriciclagio>
    {
        Task<List<SpTransactionGiornaleAntiriciclagio>> GetTransactionsForGiornaleAntiriciclaggio(
            string trxCassa,
            string trxLocalita,
            DateTime trxDataDal,
            DateTime trxDataAl,
            bool? trxReverse,
            string trxCutId,
            string trxOptId,
            string trxDivope,
            decimal? trxImpopeDA,
            decimal? trxImpopeA,
            string arcAppName,
            bool? arcForced);
    }
}
