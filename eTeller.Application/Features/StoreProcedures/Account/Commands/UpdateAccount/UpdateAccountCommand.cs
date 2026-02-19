using MediatR;

namespace eTeller.Application.Features.StoreProcedures.Account.Commands.UpdateAccount
{
    public record UpdateAccountCommand(
        int iacId,
        string iacAccId,
        string iacCutId,
        string iacCurId,
        string iacDes,
        string iacActId,
        string iacCliCassa,
        string iacBraId,
        string iacHostprefix
        ) : IRequest<int>;
}
