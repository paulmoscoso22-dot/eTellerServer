using eTeller.Application.Features.Manager.Commands.Tabelle.DeleteServizio;
using eTeller.Application.Features.Manager.Commands.Tabelle.InsertServizio;
using eTeller.Application.Features.Manager.Commands.Tabelle.UpdateServizio;
using eTeller.Application.Features.Manager.Queries.Tabelle.GetServizi;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace eTeller.Api.Controllers.Manager.Tabelle
{
    [Route("api/manager/[controller]")]
    [Tags("Manager/Tabelle")]
    [ApiController]
    public class ServiziController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ServiziController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>Recupera tutti i servizi.</summary>
        [HttpGet]
        [Route("GetServizi")]
        public async Task<IActionResult> GetServizi()
        {
            var result = await _mediator.Send(new GetServiziQuery());
            return Ok(result);
        }

        /// <summary>Inserisce un nuovo servizio.</summary>
        [HttpPost]
        [Route("InsertServizio")]
        public async Task<IActionResult> InsertServizio([FromBody] InsertServizioCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        /// <summary>Aggiorna un servizio esistente.</summary>
        [HttpPut]
        [Route("UpdateServizio")]
        public async Task<IActionResult> UpdateServizio([FromBody] UpdateServizioCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        /// <summary>Elimina un servizio.</summary>
        [HttpDelete]
        [Route("DeleteServizio")]
        public async Task<IActionResult> DeleteServizio([FromBody] DeleteServizioCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }
    }
}
