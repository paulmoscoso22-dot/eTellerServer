using MediatR;
using System.Collections.Generic;
using eTeller.Application.Mappings.Manager;

namespace eTeller.Application.Features.StoreProcedures.Manager.Queries.GetSysFunctions
{
    public class GetSysFunctionsQuery : IRequest<IEnumerable<SysFunctionsVm>>
    {
    }
}
