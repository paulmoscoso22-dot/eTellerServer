using eTeller.Application.Mappings.ST_OperationType;
using MediatR;

namespace eTeller.Application.Features.Tabella.Queries.GetOperationTypes
{
    public class GetOperationTypesQuery : IRequest<IEnumerable<ST_OperationTypeVm>>
    {
    }
}
