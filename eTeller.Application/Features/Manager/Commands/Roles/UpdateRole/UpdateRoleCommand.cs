using MediatR;
using eTeller.Application.Mappings.Manager;

namespace eTeller.Application.Features.Manager.Commands.Roles.UpdateRole
{
    public record UpdateRoleCommand(
        int RoleId,
        string RoleName,
        string RoleDes
    ) : IRequest<SysRoleVm>;
}
