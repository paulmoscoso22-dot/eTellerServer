using eTeller.Domain.Models;
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

        Task<List<SpAntirecRules>> GetAntirecRulesByParameters(
            string? arlOpTypeId,
            string? arlCurTypeId,
            string? arlAcctId,
            string? arlAcctType);

        Task<List<AppearerAll>> GetAppearerByParameters(
            string? nome1,
            string? nome2,
            string? nome3,
            string? nome4,
            DateTime? araBirthdate,
            bool? araRecComplete,
            DateTime? minRecdate);

        Task<List<AppearerAll>> GetAppearerByParametersWithExpiry(
            string araName,
            string? araBirthdate,
            bool araRecComplete,
            bool showExpiredRecords,
            int recordValidityDays = 365);

        Task<AppearerAll?> GetAppearerAllByAraId(int araId);

        Task<int> InsertHisAntirecAppearer(
            DateTime hisDate,
            int araId,
            DateTime araRecdate,
            string araName,
            DateTime? araBirthdate,
            string? araBirthplace,
            string? araNationality,
            string? araIddocnum,
            DateTime? araDocexpdate,
            bool araRecComplete,
            string? araRepresents,
            string? araAddress);

        Task<int> UpdateAntirecAppearer(
            int araId,
            DateTime araRecdate,
            string araName,
            DateTime? araBirthdate,
            string? araBirthplace,
            string? araNationality,
            string? araIddocnum,
            DateTime? araDocexpdate,
            string? araRepresents,
            string? araAddress,
            bool araRecComplete,
            bool araIsupdated);

        Task<int> InsertARA(
            string traUser,
            string traStation,
            DateTime araRecdate,
            string araName,
            string? araBirthdate,
            string? araBirthplace,
            string? araIddocnum,
            string? araNationality,
            string? araDocexpdate,
            string? araRepresents,
            string? araAddress,
            bool araRecComplete);

        Task<int> UpdateARA(
            string traUser,
            string traStation,
            int araId,
            string araName,
            string? araBirthdate,
            string? araBirthplace,
            string? araNationality,
            string? araIddocnum,
            string? araDocexpdate,
            string? araRepresents,
            string? araAddress,
            bool araRecComplete);

        Task<bool> DeleteARA(
            string traUser,
            string traStation,
            int araId);
    }
}
