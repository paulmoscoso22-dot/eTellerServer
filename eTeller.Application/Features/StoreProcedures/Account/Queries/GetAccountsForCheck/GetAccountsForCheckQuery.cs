using MediatR;

namespace eTeller.Application.Features.StoreProcedures.Account.Queries.GetAccountsForCheck
{
    public record GetAccountsForCheckQuery(string iacCutId, string iacCurId, string iacActId, string iacCliCassa) : IRequest<int>;
}
