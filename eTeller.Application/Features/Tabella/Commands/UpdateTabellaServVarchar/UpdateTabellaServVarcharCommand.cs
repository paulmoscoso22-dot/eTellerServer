using MediatR;

namespace eTeller.Application.Features.Tabella.Commands.UpdateTabellaServVarchar
{
    public class UpdateTabellaServVarcharCommand : IRequest<bool>
    {
        public string NomeTabella { get; set; }
        public string Id { get; set; }
        public string Des { get; set; }
    }
}
