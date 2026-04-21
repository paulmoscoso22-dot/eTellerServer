using MediatR;

namespace eTeller.Application.Features.Manager.Commands.Users.ResetPassword
{
    public record ResetPasswordCommand(
        string UsrId,
        bool ChgPas,
        string UsrPass,
        string UsrPass2
    ) : IRequest<int>;
}