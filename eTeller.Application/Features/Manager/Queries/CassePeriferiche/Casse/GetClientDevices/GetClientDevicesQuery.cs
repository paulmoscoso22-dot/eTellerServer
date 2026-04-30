using MediatR;

namespace eTeller.Application.Features.Manager.Queries.CassePeriferiche.Casse.GetClientDevices
{
    public record GetClientDevicesQuery(string CliId) : IRequest<IEnumerable<int>>;
}
