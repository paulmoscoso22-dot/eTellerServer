using MediatR;
using eTeller.Application.Mappings.User;
using System.Collections.Generic;

namespace eTeller.Application.Features.User.Queries.GetUserUseClient
{
    public record GetUserUseClientQuery : IRequest<IEnumerable<SysUsersUseClientVm>>;
}
