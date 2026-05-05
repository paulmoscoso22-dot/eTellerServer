using eTeller.Application.Features.Manager.Commands.Divise.CoppieDivise.DeleteCurrencyCouple;
using eTeller.Application.Features.Manager.Commands.Divise.CoppieDivise.InsertCurrencyCouple;
using eTeller.Application.Features.Manager.Commands.Divise.CoppieDivise.UpdateCurrencyCouple;
using eTeller.Application.Features.Manager.Queries.Divise.CoppieDivise.GetAllCurrencyCouples;
using eTeller.Application.Features.Manager.Queries.Divise.CoppieDivise.GetCurrenciesDV;
using eTeller.Application.Features.Manager.Queries.Divise.CoppieDivise.GetCurrencyCoupleByKey;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace eTeller.Api.Controllers.Manager.Divise.CurrencyCouple
{
    [Route("api/[controller]")]
    [ApiController]
    public class CurrencyCoupleController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CurrencyCoupleController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _mediator.Send(new GetAllCurrencyCouplesQuery());
            return Ok(result);
        }

        [HttpGet("GetByKey")]
        public async Task<IActionResult> GetByKey([FromQuery] string cur1, [FromQuery] string cur2)
        {
            var result = await _mediator.Send(new GetCurrencyCoupleByKeyQuery(cur1, cur2));
            if (result is null) return NotFound();
            return Ok(result);
        }

        [HttpPost("GetCurrenciesDV")]
        public async Task<IActionResult> GetCurrenciesDV()
        {
            var result = await _mediator.Send(new GetCurrenciesDVQuery());
            return Ok(result);
        }

        [HttpPost("Insert")]
        public async Task<IActionResult> Insert([FromBody] InsertCurrencyCoupleCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpPut("Update")]
        public async Task<IActionResult> Update([FromBody] UpdateCurrencyCoupleCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete([FromQuery] string cur1, [FromQuery] string cur2, [FromQuery] string traUser, [FromQuery] string traStation)
        {
            var result = await _mediator.Send(new DeleteCurrencyCoupleCommand(cur1, cur2, traUser, traStation));
            return Ok(result);
        }
    }
}
