using MediatR;

namespace eTeller.Application.Features.Tabella.Commands.UpdateTabellaServInt
{
    public class UpdateTabellaServIntCommand : IRequest<bool>
    {
        public string NomeTabella { get; set; }
        public int Id { get; set; }
        public string Des { get; set; }
    }
}
