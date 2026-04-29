using eTeller.Application.Mappings.Client;
using MediatR;

namespace eTeller.Application.Features.Client.Queries.GetClient
{
    public record GetClientQuery : IRequest<IEnumerable<ClientVm>>;
}
