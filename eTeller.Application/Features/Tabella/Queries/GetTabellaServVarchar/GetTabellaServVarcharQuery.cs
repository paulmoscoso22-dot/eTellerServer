using eTeller.Application.Mappings.Tabella;
using MediatR;

namespace eTeller.Application.Features.Tabella.Queries.GetTabellaServVarchar
{
    public class GetTabellaServVarcharQuery : IRequest<IEnumerable<TabellaServVarcharVm>>
    {
        public string NomeTabella { get; set; }
        public string? Id { get; set; }
        public string? DesLike { get; set; }
    }
}
