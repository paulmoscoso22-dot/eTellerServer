using MediatR;
using Microsoft.AspNetCore.Mvc;
using eTeller.Application.Features.Manager.Commands.CassePeriferiche.Casse.UpdateClient;
using eTeller.Application.Features.Client.Queries.GetClientById;
using eTeller.Application.Features.Client.Queries.GetClient;

namespace eTeller.Api.Controllers.Client
{
    [Route("api/[controller]")]
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
        [Route("GetClientById")]
        public async Task<IActionResult> GetClientById([FromBody] GetClientByIdQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }
        [HttpPut]
        [Route("UpdateClient")]
        public async Task<IActionResult> UpdateClient([FromBody] UpdateClientCommand request)
        {
            var result = await _mediator.Send(request);
           
            if (result)
                return NoContent();
            return BadRequest("Update failed");
        }
    }
}

