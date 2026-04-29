using eTeller.Application.Mappings.CassePeriferiche;
using MediatR;

namespace eTeller.Application.Features.CassePeriferiche.Queries.GetDeviceByCliId
{
    public record GetDeviceByCliIdQuery(string CliId) : IRequest<List<DeviceVm>>;
}