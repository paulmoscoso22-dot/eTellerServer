using eTeller.Application.Mappings.Client;
using MediatR;

namespace eTeller.Application.Features.Manager.Queries.CassePeriferiche.Casse.GetClient
{
    public class GetClientQuery : IRequest<IEnumerable<ClientVm>>
    {
    }
}
