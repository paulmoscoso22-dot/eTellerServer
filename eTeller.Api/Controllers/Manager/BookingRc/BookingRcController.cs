using eTeller.Application.Features.Manager.Commands.Tabelle.InsertBookingRc;
using eTeller.Application.Features.Manager.Commands.Tabelle.UpdateBookingRc;
using eTeller.Application.Features.Manager.Queries.Tabelle.GetAccountTypes;
using eTeller.Application.Features.Manager.Queries.Tabelle.GetAllBookingRc;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace eTeller.Api.Controllers.Manager.BookingRc
{
    [Route("api/manager/[controller]")]
    [Tags("Manager/BookingRc")]
    [ApiController]
    public class BookingRcController : ControllerBase
    {
        private readonly IMediator _mediator;

        public BookingRcController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>Recupera i BookingRc con filtro per CutId, OptId, ActId.</summary>
        [HttpPost]
        [Route("GetBookingRc")]
        public async Task<IActionResult> GetBookingRc([FromBody] GetAllBookingRcQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>Recupera tutti gli AccountType.</summary>
        [HttpPost]
        [Route("GetAccountTypes")]
        public async Task<IActionResult> GetAccountTypes()
        {
            var result = await _mediator.Send(new GetAccountTypesQuery());
            return Ok(result);
        }

        /// <summary>Inserisce un nuovo BookingRc.</summary>
        [HttpPost]
        [Route("InsertBookingRc")]
        public async Task<IActionResult> InsertBookingRc([FromBody] InsertBookingRcCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        /// <summary>Aggiorna un BookingRc esistente.</summary>
        [HttpPost]
        [Route("UpdateBookingRc")]
        public async Task<IActionResult> UpdateBookingRc([FromBody] UpdateBookingRcCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }
    }
}
