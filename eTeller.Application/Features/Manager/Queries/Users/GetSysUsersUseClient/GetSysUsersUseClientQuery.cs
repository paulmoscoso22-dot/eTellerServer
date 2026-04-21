using MediatR;
using eTeller.Application.Mappings.User;
using System.Collections.Generic;

namespace eTeller.Application.Features.Manager.Queries.Users.GetSysUsersUseClient
{
    public class GetSysUsersUseClientQuery : IRequest<IEnumerable<SysUsersUseClientVm>>
    {
    }
}
