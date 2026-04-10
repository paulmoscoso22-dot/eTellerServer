using MediatR;
using eTeller.Application.Mappings.Manager;

namespace eTeller.Application.Features.Manager.Queries.Users.GetUserByHostId
{
    public class GetUserByHostIdQuery : IRequest<IEnumerable<SysUserByIdVm>>
    {
        public string UsrHostId { get; set; }

        public GetUserByHostIdQuery(string usrHostId)
        {
            UsrHostId = usrHostId;
        }
    }
}
