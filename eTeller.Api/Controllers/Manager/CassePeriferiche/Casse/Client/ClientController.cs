using eTeller.Application.Features.Manager.Commands.CassePeriferiche.Casse.DeleteClient;
using eTeller.Application.Features.Manager.Commands.CassePeriferiche.Casse.InsertClient;
using eTeller.Application.Features.Manager.Commands.CassePeriferiche.Casse.UpdateClient;
using eTeller.Application.Features.Manager.Queries.CassePeriferiche.Casse.GetClient;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace eTeller.Api.Controllers.Manager.CassePeriferiche.Casse.Client
{
    [Route("api/[controller]")]
    [Tags("Manager/CassePeriferiche")]
    [ApiController]
    public class ClientController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ClientController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        [Route("GetClient")]
        public async Task<IActionResult> GetClient()
        {
            var result = await _mediator.Send(new GetClientQuery());
            return Ok(result);
        }

        [HttpPost]
        [Route("InsertClient")]
        public async Task<IActionResult> InsertClient([FromBody] InsertClientCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpPut]
        [Route("UpdateClient")]
        public async Task<IActionResult> UpdateClient([FromBody] UpdateClientCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpDelete]
        [Route("DeleteClient")]
        public async Task<IActionResult> DeleteClient([FromBody] DeleteClientCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }
    }
}
