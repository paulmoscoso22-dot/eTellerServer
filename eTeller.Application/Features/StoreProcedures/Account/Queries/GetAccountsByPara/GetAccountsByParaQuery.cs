using eTeller.Application.Mappings;
using MediatR;

namespace eTeller.Application.Features.StoreProcedures.Account.Queries.GetAccountsByPara
{
    public record GetAccountsByParaQuery(
        string iacAccId,
        string iacCutId,
        string iacCurId,
        string iacDes,
        string iacActId,
        string iacCliCassa,
        string iacBraId
        ) : IRequest<IEnumerable<AccountVm>>;
}
