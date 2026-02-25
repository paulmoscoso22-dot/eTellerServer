using eTeller.Application.Features.Currency.Queries.GetAllCurrencies;
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
        public async Task<IActionResult> GetAllCurrencies()
        {
            var query = new GetAllCurrenciesQuery();
            var result = await _mediator.Send(query);
            return Ok(result);
        }
    }
}
