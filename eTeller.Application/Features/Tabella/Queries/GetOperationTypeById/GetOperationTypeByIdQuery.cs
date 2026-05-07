using eTeller.Application.Mappings.ST_OperationType;
using MediatR;

namespace eTeller.Application.Features.Tabella.Queries.GetOperationTypeById
{
    public class GetOperationTypeByIdQuery : IRequest<ST_OperationTypeVm>
    {
        public string OptId { get; set; }
    }
}
