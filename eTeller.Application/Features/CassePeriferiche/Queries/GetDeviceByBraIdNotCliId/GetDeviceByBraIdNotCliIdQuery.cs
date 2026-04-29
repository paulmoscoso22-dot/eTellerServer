using eTeller.Application.Mappings.CassePeriferiche;
using MediatR;

namespace eTeller.Application.Features.CassePeriferiche.Queries.GetDeviceByBraIdNotCliId
{
    public class GetDeviceByBraIdNotCliIdQuery : IRequest<List<DeviceVm>>
    {
        public string CliId { get; set; }
        public string BraId { get; set; }

        public GetDeviceByBraIdNotCliIdQuery(string cliId, string braId)
        {
            CliId = cliId;
            BraId = braId;
        }
    }
}
