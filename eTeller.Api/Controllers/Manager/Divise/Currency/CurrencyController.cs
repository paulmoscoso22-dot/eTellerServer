using eTeller.Application.Features.Currency.Commands.UpdateCurrency;
using eTeller.Application.Features.Currency.Queries.GetAllCurrencies;
using eTeller.Application.Features.Currency.Queries.GetCurrencyByKey;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace eTeller.Api.Controllers.Currency
{
    [Route("api/[controller]")]
    [ApiController]
    public class CurrencyController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CurrencyController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        [Route("GetAllCurrencies")]
        public async Task<IActionResult> GetAllCurrencies([FromBody] GetAllCurrenciesQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpGet]
        [Route("GetByKey")]
        public async Task<IActionResult> GetByKey([FromQuery] string curId, [FromQuery] string curCutId)
        {
            var result = await _mediator.Send(new GetCurrencyByKeyQuery(curId, curCutId));
            if (result is null)
                return NotFound();
            return Ok(result);
        }

        [HttpPut]
        [Route("UpdateCurrency")]
        public async Task<IActionResult> UpdateCurrency([FromBody] UpdateCurrencyCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }
    }
}
