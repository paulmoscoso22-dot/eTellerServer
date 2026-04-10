using eTeller.Application.Features.Manager.Commands.Pesonalisation.UpdatePersonalisation;
using eTeller.Application.Features.Manager.Queries.Personalisation.GetPersonalisation;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace eTeller.Api.Controllers.Manager.Personalisation
{
    [Route("api/[controller]")]
    [Tags("Manager/Personalisation")]
    [ApiController]
    public class PersonalisationController : ControllerBase
    {
        private readonly IMediator _mediator;

        public PersonalisationController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        [Route("GetPersonalisation")]
        public async Task<IActionResult> GetPersonalisation()
        {
            var query = new GetPersonalisationQuery();
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpPut]
        [Route("UpdatePersonalisation")]
        public async Task<IActionResult> UpdatePersonalisation([FromBody] UpdatePersonalisationCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }
    }
}
