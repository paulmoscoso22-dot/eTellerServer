using MediatR;
using System.Collections.Generic;
using eTeller.Application.Mappings.Manager;

namespace eTeller.Application.Features.Manager.Queries.Functions.GetSysFunctions
{
    public class GetSysFunctionsQuery : IRequest<IEnumerable<SysFunctionsVm>>
    {
    }
}
