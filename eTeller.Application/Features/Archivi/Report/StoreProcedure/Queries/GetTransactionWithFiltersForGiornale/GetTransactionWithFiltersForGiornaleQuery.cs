using eTeller.Application.Mappings;
using MediatR;

namespace eTeller.Application.Features.Archivi.Report.StoreProcedure.Queries.GetTransactionWithFiltersForGiornale
{
    public record GetTransactionWithFiltersForGiornaleQuery(
        string trxCassa,
        DateTime trxDataDal,
        DateTime trxDataAl,
        int trxStatus,
        string trxBraId
        ) : IRequest<IEnumerable<TransactionVm>>;
}
