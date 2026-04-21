using MediatR;

namespace eTeller.Application.Features.Manager.Commands.Users.UpdateUserClientExit
{
    public record UpdateUserClientExitCommand(
        int UsrCliId
    ) : IRequest<int>;
}