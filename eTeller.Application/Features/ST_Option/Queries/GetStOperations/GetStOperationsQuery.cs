using eTeller.Application.Mappings.ST_OperationType;
using MediatR;

namespace eTeller.Application.Features.ST_Option.Queries.GetStOperations
{
    public class GetStOperationsQuery : IRequest<IEnumerable<ST_OperationTypeVm>>
    {
    }
}
