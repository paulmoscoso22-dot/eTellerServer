using MediatR;

namespace eTeller.Application.Features.Tabella.Commands.InsertTabellaServInt
{
    public class InsertTabellaServIntCommand : IRequest<bool>
    {
        public string NomeTabella { get; set; }
        public int Id { get; set; }
        public string Des { get; set; }
    }
}
