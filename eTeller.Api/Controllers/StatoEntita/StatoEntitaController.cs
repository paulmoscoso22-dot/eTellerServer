using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace eTeller.Api.Controllers.StatoEntita
{
    [Route("api/[controller]")]
    [Tags("StatoEntita")]
    [ApiController]
    public class StatoEntitaController : ControllerBase
    {
        private readonly IMediator _mediator;

        public StatoEntitaController(IMediator mediator)
        {
            _mediator = mediator;
        }
        [HttpPost]
        [Route("GetStatoEntitaById")]
        public async Task<IActionResult> GetStatoEntitaById([FromBody] eTeller.Application.Features.StatoEntita.GetStatoEntita.GetStatoEntitaQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpPost]
        [Route("GetAllStatiEntita")]
        public async Task<IActionResult> GetAllStatiEntita()
        {
            var query = new eTeller.Application.Features.StatoEntita.GetAllStatiEntita.GetAllStatiEntitaQuery();
            var result = await _mediator.Send(query);
            return Ok(result);
        }
    }
}
