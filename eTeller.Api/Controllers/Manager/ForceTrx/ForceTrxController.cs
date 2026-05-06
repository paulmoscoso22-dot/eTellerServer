using eTeller.Application.Features.Manager.Queries.Tabelle.GetAllForceTrx;
using eTeller.Application.Features.Manager.Queries.Tabelle.GetForceTrxById;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace eTeller.Api.Controllers.Manager.ForceTrx
{
    [Route("api/manager/[controller]")]
    [Tags("Manager/ForceTrx")]
    [ApiController]
    public class ForceTrxController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ForceTrxController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>Recupera tutte le transazioni forzate.</summary>
        [HttpPost]
        [Route("GetAllForceTrx")]
        public async Task<IActionResult> GetAllForceTrx([FromBody] GetAllForceTrxQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>Recupera una transazione forzata per ID.</summary>
        [HttpPost]
        [Route("GetForceTrxById")]
        public async Task<IActionResult> GetForceTrxById([FromBody] GetForceTrxByIdQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }
    }
}
