using MediatR;
using eTeller.Application.Mappings.Manager;

namespace eTeller.Application.Features.Manager.Commands.Roles.InsertRole
{
    public record InsertRoleCommand(
        string RoleName,
        string RoleDes,
        string traUser,
        string traStation, 
        string info

    ) : IRequest<SysRoleVm>;
}