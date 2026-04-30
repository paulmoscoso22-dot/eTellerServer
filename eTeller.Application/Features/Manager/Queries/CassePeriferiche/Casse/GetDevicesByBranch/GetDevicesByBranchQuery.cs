using eTeller.Application.Mappings.Device;
using MediatR;

namespace eTeller.Application.Features.Manager.Queries.CassePeriferiche.Casse.GetDevicesByBranch
{
    public record GetDevicesByBranchQuery(string BraId) : IRequest<IEnumerable<DeviceVm>>;
}
