using MediatR;
using System.Collections.Generic;
using eTeller.Application.Mappings.Manager;

namespace eTeller.Application.Features.StoreProcedures.Manager.Queries.GetSysRoleByFunId
{
    public class GetSysRoleByFunIdQuery : IRequest<IEnumerable<SysRoleVm>>
    {
        public int FunId { get; set; }
    }
}
