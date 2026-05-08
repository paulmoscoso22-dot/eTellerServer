using eTeller.Application.Mappings.Tabella;
using MediatR;

namespace eTeller.Application.Features.Tabella.Queries.GetTabellaServInt
{
    public class GetTabellaServIntQuery : IRequest<IEnumerable<TabellaServIntVm>>
    {
        public string NomeTabella { get; set; }
        public int? Id { get; set; }
        public string? DesLike { get; set; }
    }
}
