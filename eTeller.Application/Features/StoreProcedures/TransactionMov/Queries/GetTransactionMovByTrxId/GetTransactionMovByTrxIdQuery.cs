using eTeller.Application.Mappings;
using MediatR;

namespace eTeller.Application.Features.StoreProcedures.TransactionMov.Queries.GetTransactionMovByTrxId
{
    public record GetTransactionMovByTrxIdQuery(int trxId) : IRequest<IEnumerable<TransactionMovVm>>;
}
