using eTeller.Application.Mappings;
using MediatR;

namespace eTeller.Application.Features.StoreProcedures.Transaction.Queries.GetTransactionWithFiltersForGiornaleAntiriciclaggio
{
    public record GetTransactionWithFiltersForGiornaleAntiriciclaggioQuery(
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
        bool? arcForced
        ) : IRequest<IEnumerable<GiornaleAntiriciclaggioVm>>;
}
