using MediatR;

namespace eTeller.Application.Features.Tabella.Commands.InsertTabellaServVarchar
{
    public class InsertTabellaServVarcharCommand : IRequest<bool>
    {
        public string NomeTabella { get; set; }
        public string Id { get; set; }
        public string Des { get; set; }
    }
}
