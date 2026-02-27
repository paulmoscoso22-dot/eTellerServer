using eTeller.Application.Features.ST_CurrencyType.Queries.GetCurrencyTypes;
using eTeller.Application.Features.ST_CurrencyType.Queries.GetCurrencyTypeByCutID;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace eTeller.Api.Controllers.CurrencyType
{
    [Route("api/[controller]")]
    [ApiController]
    public class CurrencyTypeController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CurrencyTypeController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        [Route("GetCurrencyTypes")]
        public async Task<IActionResult> GetCurrencyTypes()
        {
            var query = new GetCurrencyTypesQuery();
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpPost]
        [Route("GetCurrencyTypeByCutID")]
        public async Task<IActionResult> GetCurrencyTypeByCutID([FromBody] GetCurrencyTypeByCutIDQuery query)
        {
            var result = await _mediator.Send(query);
            if (result == null)
                return NotFound();
            return Ok(result);
        }
    }
}
