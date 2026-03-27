using eTeller.Application.Mappings.Tabella;
using MediatR;

namespace eTeller.Application.Features.Tabella.Queries.GetTabellaServVarcharById
{
    public class GetTabellaServVarcharByIdQuery : IRequest<TabellaServVarcharVm>
    {
        public string Id { get; set; }
        public string NomeTabella { get; set; }
    }
}