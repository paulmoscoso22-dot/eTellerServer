using eTeller.Application.Features.Tabella.Commands.InsertTabellaServInt;
using eTeller.Application.Features.Tabella.Commands.UpdateTabellaServInt;
using eTeller.Application.Features.Tabella.Queries.GetTabellaServInt;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace eTeller.Api.Controllers.Manager
{
    [Route("api/[controller]")]
    [Tags("Manager")]
    [ApiController]
    public class ManagerController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ManagerController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>Ricerca elementi in una tabella con ID intero, con filtri opzionali.</summary>
        [HttpPost("GetTabellaInt")]
        public async Task<IActionResult> GetTabellaInt([FromBody] GetTabellaServIntQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>Inserisce un nuovo elemento in una tabella con ID intero.</summary>
        [HttpPost("InsertTabellaInt")]
        public async Task<IActionResult> InsertTabellaInt([FromBody] InsertTabellaServIntCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        /// <summary>Aggiorna la descrizione di un elemento in una tabella con ID intero.</summary>
        [HttpPost("UpdateTabellaInt")]
        public async Task<IActionResult> UpdateTabellaInt([FromBody] UpdateTabellaServIntCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }
    }
}
