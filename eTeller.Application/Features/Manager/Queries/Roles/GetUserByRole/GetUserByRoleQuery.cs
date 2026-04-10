using MediatR;
using eTeller.Application.Mappings.Manager;

namespace eTeller.Application.Features.Manager.Queries.Roles.GetUserByRole
{
    public record GetUserByRoleQuery(int RoleId) : IRequest<IEnumerable<UserSelectRoleVm>>;
}
