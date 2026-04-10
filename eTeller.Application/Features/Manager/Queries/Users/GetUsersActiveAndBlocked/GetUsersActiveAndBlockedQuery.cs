using MediatR;
using eTeller.Application.Mappings.User;

namespace eTeller.Application.Features.Manager.Queries.Users.GetUsersActiveAndBlocked
{
    public record GetUsersActiveAndBlockedQuery : IRequest<IEnumerable<SysUsersActiveAndBlockedVm>>;
}