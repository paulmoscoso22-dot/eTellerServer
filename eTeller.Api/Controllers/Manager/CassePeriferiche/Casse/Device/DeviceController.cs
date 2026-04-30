using eTeller.Application.Features.Manager.Queries.CassePeriferiche.Casse.GetClientDevices;
using eTeller.Application.Features.Manager.Queries.CassePeriferiche.Casse.GetDevicesByBranch;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace eTeller.Api.Controllers.Manager.CassePeriferiche.Casse.Device
{
    [Route("api/[controller]")]
    [Tags("Manager/CassePeriferiche")]
    [ApiController]
    public class DeviceController : ControllerBase
    {
        private readonly IMediator _mediator;

        public DeviceController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        [Route("GetDevicesByBranch")]
        public async Task<IActionResult> GetDevicesByBranch([FromQuery] string braId)
        {
            var result = await _mediator.Send(new GetDevicesByBranchQuery(braId));
            return Ok(result);
        }

        [HttpGet]
        [Route("GetClientDevices")]
        public async Task<IActionResult> GetClientDevices([FromQuery] string cliId)
        {
            var result = await _mediator.Send(new GetClientDevicesQuery(cliId));
            return Ok(result);
        }
    }
}
