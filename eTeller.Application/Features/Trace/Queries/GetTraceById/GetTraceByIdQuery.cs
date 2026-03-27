using eTeller.Application.Mappings.Trace;
using MediatR;

namespace eTeller.Application.Features.Trace.Queries.GetTraceById
{
    public class GetTraceByIdQuery : IRequest<TraceVm>
    {
        public int TraId { get; set; }
    }
}