using eTeller.Application.Features.Manager.Commands.Tabelle.DeleteFunzioneSchedule;
using eTeller.Application.Features.Manager.Commands.Tabelle.InsertFunzioneSchedule;
using eTeller.Application.Features.Manager.Commands.Tabelle.ResetFunctionError;
using eTeller.Application.Features.Manager.Commands.Tabelle.ScheduleOneTimeTask;
using eTeller.Application.Features.Manager.Commands.Tabelle.UpdateFunzioneSchedule;
using eTeller.Application.Features.Manager.Queries.Tabelle.GetFunzioniSchedule;
using eTeller.Application.Features.Manager.Queries.Tabelle.GetFunzioniScheduleById;
using eTeller.Application.Features.Manager.Queries.Tabelle.GetPeriodTypes;
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

        /// <summary>Ricerca funzioni schedulate con filtri opzionali su nome e descrizione.</summary>
        [HttpPost]
        [Route("GetFunzioniSchedule")]
        public async Task<IActionResult> GetFunzioniSchedule([FromBody] GetFunzioniScheduleQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>Recupera una funzione schedulata per ID.</summary>
        [HttpPost]
        [Route("GetFunzioneScheduleById")]
        public async Task<IActionResult> GetFunzioneScheduleById([FromBody] GetFunzioniScheduleByIdQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>Inserisce una nuova funzione schedulata (auto o manuale).</summary>
        [HttpPost]
        [Route("InsertFunzioneSchedule")]
        public async Task<IActionResult> InsertFunzioneSchedule([FromBody] InsertFunzioneScheduleCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        /// <summary>Aggiorna una funzione schedulata esistente.</summary>
        [HttpPut]
        [Route("UpdateFunzioneSchedule")]
        public async Task<IActionResult> UpdateFunzioneSchedule([FromBody] UpdateFunzioneScheduleCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        /// <summary>Elimina una funzione schedulata.</summary>
        [HttpDelete]
        [Route("DeleteFunzioneSchedule")]
        public async Task<IActionResult> DeleteFunzioneSchedule([FromBody] DeleteFunzioneScheduleCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        /// <summary>Azzera il contatore errori di una funzione schedulata.</summary>
        [HttpPost]
        [Route("ResetFunctionError")]
        public async Task<IActionResult> ResetFunctionError([FromBody] ResetFunctionErrorCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        /// <summary>Forza l'esecuzione immediata (one-time run) di una funzione schedulata.</summary>
        [HttpPost]
        [Route("ScheduleOneTimeTask")]
        public async Task<IActionResult> ScheduleOneTimeTask([FromBody] ScheduleOneTimeTaskCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        /// <summary>Recupera i tipi di periodicità per le scheduled tasks.</summary>
        [HttpGet]
        [Route("GetPeriodTypes")]
        public async Task<IActionResult> GetPeriodTypes()
        {
            var result = await _mediator.Send(new GetPeriodTypesQuery());
            return Ok(result);
        }
    }
}
