using MediatR;
using eTeller.Application.Mappings.Manager;

namespace eTeller.Application.Features.Manager.Queries.Roles.GetRoleByName
{
    public record GetRoleByNameQuery(string RoleName) : IRequest<SysRoleVm?>;
}