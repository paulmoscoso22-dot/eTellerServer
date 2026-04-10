using MediatR;
using eTeller.Application.Mappings.Manager;

namespace eTeller.Application.Features.Manager.Queries.Roles.GetAllRole
{
    public record GetAllRoleQuery : IRequest<IEnumerable<SysRoleVm>>;
}
