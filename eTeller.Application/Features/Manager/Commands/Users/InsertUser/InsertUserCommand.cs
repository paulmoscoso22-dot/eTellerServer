using MediatR;
using eTeller.Application.Mappings.User;

namespace eTeller.Application.Features.Manager.Commands.Users.InsertUser
{
    public record InsertUserCommand(
        string UsrId,
        string UsrHostId,
        string UsrBraId,
        string UsrStatus,
        string? UsrExtref,
        string UsrLingua,
        string TraUser,
        string TraStation
    ) : IRequest<InsertUserVm>;
}