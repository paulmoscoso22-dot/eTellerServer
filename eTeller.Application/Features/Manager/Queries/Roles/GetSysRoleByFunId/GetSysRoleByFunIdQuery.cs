using MediatR;
using System.Collections.Generic;
using eTeller.Application.Mappings.Manager;

namespace eTeller.Application.Features.Manager.Queries.Roles.GetSysRoleByFunId
{
    public class GetSysRoleByFunIdQuery : IRequest<IEnumerable<SysRoleVm>>
    {
        public int FunId { get; set; }
    }
}
