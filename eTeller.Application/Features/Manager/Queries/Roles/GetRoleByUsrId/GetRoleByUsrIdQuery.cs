using MediatR;
using eTeller.Application.Mappings.Manager;

namespace eTeller.Application.Features.Manager.Queries.Roles.GetRoleByUsrId
{
    public class GetRoleByUsrIdQuery : IRequest<IEnumerable<SysRoleVm>>
    {
        public string UsrId { get; set; }

        public GetRoleByUsrIdQuery(string usrId)
        {
            UsrId = usrId;
        }
    }
}
