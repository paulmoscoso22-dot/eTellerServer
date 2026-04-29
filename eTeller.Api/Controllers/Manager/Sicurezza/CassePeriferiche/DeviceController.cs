using eTeller.Application.Features.CassePeriferiche.Queries.GetDeviceByBraIdNotCliId;
using eTeller.Application.Features.CassePeriferiche.Queries.GetDeviceByCliId;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using eTeller.Application.Features.CassePeriferiche.Commands;

namespace eTeller.Api.Controllers.Manager.Sicurezza.CassePeriferiche
{
    [Route("api/[controller]")]
    [ApiController]
    public class DeviceController : ControllerBase
    {
        private readonly IMediator _mediator;

        public DeviceController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        [Route("GetDeviceByBraIdNotCliId")]
        public async Task<IActionResult> GetDeviceByBraIdNotCliId([FromBody] GetDeviceByBraIdNotCliIdQuery request)
        {
            var result = await _mediator.Send(request);
            return Ok(result);
        }

        [HttpPost]
        [Route("GetDeviceByCliId")]
        public async Task<IActionResult> GetDeviceByCliId([FromBody] GetDeviceByCliIdQuery request)
        {
            var result = await _mediator.Send(request);
            return Ok(result);
        }

        [HttpPost]
        [Route("InsertClientDevice")]
        public async Task<IActionResult> InsertClientDevice([FromBody] InsertClientDeviceCommand request)
        {
            var result = await _mediator.Send(request);
            if (result)
                return NoContent();
            return BadRequest("Insert failed");
        }
    }
}
