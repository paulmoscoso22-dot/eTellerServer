using eTeller.Application.Mappings.User;
using MediatR;

namespace eTeller.Application.Features.User.Queries.GetActiveAndBlockedUsers
{
    public class GetActiveAndBlockedUsersQuery : IRequest<IEnumerable<SysUsersActiveAndBlockedVm>>
    {
    }
}
