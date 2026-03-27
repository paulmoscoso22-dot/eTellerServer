using eTeller.Application.Mappings.Vigilanza;
using MediatR;

namespace eTeller.Application.Features.StoreProcedures.Vigilanza.Queries.GetTransactionsForGiornaleAntiriciclaggio
{
    public record GetTransactionsForGiornaleAntiriciclaggioQuery(
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
        ) : IRequest<IEnumerable<SpTransactionGiornaleAntiriciclagioVm>>;
}
