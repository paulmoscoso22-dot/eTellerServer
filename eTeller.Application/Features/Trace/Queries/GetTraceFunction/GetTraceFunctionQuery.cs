using eTeller.Application.Mappings.Trace;
using MediatR;

namespace eTeller.Application.Features.Trace.Queries.GetTraceFunction
{
    public class GetTraceFunctionQuery : IRequest<IEnumerable<ST_TRACE_FUNCTIONVm>>
    {
    }
}
