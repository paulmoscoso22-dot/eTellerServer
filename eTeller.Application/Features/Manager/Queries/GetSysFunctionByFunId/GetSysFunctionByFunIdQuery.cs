using MediatR;
using System.Collections.Generic;
using eTeller.Application.Mappings.Manager;

namespace eTeller.Application.Features.StoreProcedures.Manager.Queries.GetSysFunctionByFunId
{
    public class GetSysFunctionByFunIdQuery : IRequest<IEnumerable<SysFunctionsVm>>
    {
        public int FunId { get; set; }
    }
}
