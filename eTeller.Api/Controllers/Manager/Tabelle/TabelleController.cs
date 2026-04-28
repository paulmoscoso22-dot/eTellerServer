using eTeller.Application.Features.Manager.Queries.Tabelle.GetFunzioniSchedule;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace eTeller.Api.Controllers.Manager.Tabelle
{
    [Route("api/manager/[controller]")]
    [Tags("Manager/Tabelle")]
    [ApiController]
    public class TabelleController : ControllerBase
    {
        private readonly IMediator _mediator;

        public TabelleController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        [Route("GetFunzioniSchedule")]
        public async Task<IActionResult> GetFunzioniSchedule([FromBody] GetFunzioniScheduleQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }
    }
}
