using MediatR;
using eTeller.Application.Mappings.Manager;

namespace eTeller.Application.Features.Manager.Commands.Roles.DeleteRole
{
    public record DeleteRoleCommand(int RoleId) : IRequest<bool>;
}
