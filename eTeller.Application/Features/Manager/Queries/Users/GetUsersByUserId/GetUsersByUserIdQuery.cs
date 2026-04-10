using MediatR;
using eTeller.Application.Mappings.Manager;

namespace eTeller.Application.Features.Manager.Queries.Users.GetUsersByUserId
{
    public class GetUsersByUserIdQuery : IRequest<IEnumerable<SysUserByIdVm>>
    {
        public string UsrId { get; set; }

        public GetUsersByUserIdQuery(string usrId)
        {
            UsrId = usrId;
        }
    }
}
