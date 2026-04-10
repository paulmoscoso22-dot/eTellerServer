using MediatR;
using eTeller.Application.Mappings.Manager;

namespace eTeller.Application.Features.Manager.Commands.Users.UpdateUser
{
    public record UpdateUserCommand(
        string UsrId,
        string UsrHostId,
        string UsrBraId,
        string UsrStatus,
        string? UsrExtref,
        string UsrLingua,
        int[] addIdRoles,
        int[] delIdRoles
    ) : IRequest<SysUserByIdVm>;
}