using eTeller.Domain.Models.StoredProcedure;
using static eTeller.Application.Contracts.Commons.IBaseSimpleRepository;

namespace eTeller.Application.Contracts.StoreProcedures
{
    public interface IGiornaleAntiriciclaggioSpRepository : IBaseSimpleRepository<GiornaleAntiriciclaggio>
    {
        Task<List<GiornaleAntiriciclaggio>> GetTransactionWithFiltersForGiornaleAntiriciclaggio(
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
