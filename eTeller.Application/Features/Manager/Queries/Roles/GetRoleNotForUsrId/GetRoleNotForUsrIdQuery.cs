using MediatR;
using eTeller.Application.Mappings.Manager;

namespace eTeller.Application.Features.Manager.Queries.Roles.GetRoleNotForUsrId
{
    public class GetRoleNotForUsrIdQuery : IRequest<IEnumerable<SysRoleVm>>
    {
        public string UsrId { get; set; }

        public GetRoleNotForUsrIdQuery(string usrId)
        {
            UsrId = usrId;
        }
    }
}
