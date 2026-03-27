using eTeller.Application.Mappings.Trace;
using MediatR;

namespace eTeller.Application.Features.Trace.Queries.GetTraceAll
{
    public class GetTraceAllQuery : IRequest<IEnumerable<TraceVm>>
    {
        public string? TraUser { get; set; }
        public string? TraFunCode { get; set; }
        public string? TraStation { get; set; }
        public string? TraTabNam { get; set; }
        public string? TraEntCode { get; set; }
        public bool? TraError { get; set; }
        public DateTime? DataFrom { get; set; }
        public DateTime? DataTo { get; set; }
    }
}
