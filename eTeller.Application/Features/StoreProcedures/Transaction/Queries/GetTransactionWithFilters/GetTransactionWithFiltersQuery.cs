using eTeller.Application.Mappings;
using MediatR;

namespace eTeller.Application.Features.StoreProcedures.Transaction.Queries.GetTransactionWithFilters
{
    public record GetTransactionWithFiltersQuery(
        string trxCassa,
        DateTime trxDataDal,
        DateTime trxDataAl,
        int trxStatus,
        string trxBraId
        ) : IRequest<IEnumerable<TransactionVm>>;
}
