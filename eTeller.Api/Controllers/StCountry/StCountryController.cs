using eTeller.Application.Features.StCountry.Queries.GetAllCountry;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace eTeller.Api.Controllers.StCountry
{
    [Route("api/[controller]")]
    [ApiController]
    public class StCountryController : ControllerBase
    {
        private readonly IMediator _mediator;

        public StCountryController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        [Route("GetAllCountry")]
        public async Task<IActionResult> GetAllCountry()
        {
            var query = new GetAllCountryQuery();
            var result = await _mediator.Send(query);
            return Ok(result);
        }
    }
}