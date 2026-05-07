using MediatR;

namespace eTeller.Application.Features.Tabella.Commands.InsertOperationType
{
    public class InsertOperationTypeCommand : IRequest<bool>
    {
        public string OptId { get; set; }
        public string OptDes { get; set; }
        public string OptHoscod { get; set; }
        public string OptIscredit { get; set; }
        public string OptAptId { get; set; }
        public bool OptPrtdv { get; set; }
        public string? OptAdvId { get; set; }
        public string TraUser { get; set; }
        public string TraStation { get; set; }
    }
}
