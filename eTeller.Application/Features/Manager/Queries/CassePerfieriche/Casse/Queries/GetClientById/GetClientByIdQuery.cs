using eTeller.Application.Mappings.Client;
using MediatR;

namespace eTeller.Application.Features.Client.Queries.GetClientById
{
    public class GetClientByIdQuery : IRequest<IEnumerable<ClientVm>>
    {
        public string CliId { get; set; }

        public GetClientByIdQuery(string cliId)
        {
            CliId = cliId;
        }
    }
}
