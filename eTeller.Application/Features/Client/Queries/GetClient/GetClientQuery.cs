using eTeller.Application.Mappings.Client;
using MediatR;

namespace eTeller.Application.Features.Client.Queries.GetClient
{
    public class GetClientQuery : IRequest<IEnumerable<ClientVm>>
    {
    }
}
