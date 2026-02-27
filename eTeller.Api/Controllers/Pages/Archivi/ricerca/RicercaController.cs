using eTeller.Application.Features.Archivi.Ricerca.StoreProcedure.Queries.GetTransactionWithFiltersForGiornaleAntiriciclaggio;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace eTeller.Api.Controllers.Pages.Archivi.ricerca
{
    [Route("api/[controller]")]
    [ApiController]
    public class RicercaController : ControllerBase
    {

        private readonly IMediator _mediator;
        public RicercaController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        [Route("WithFiltersForGiornaleAntiriciclaggio")]
        public async Task<IActionResult> GetTransactionWithFiltersForGiornaleAntiriciclaggio([FromBody] GetSpTransactionWithFiltersForGiornaleAntiriciclaggioQuery request)
        {
            var result = await _mediator.Send(request);
            return Ok(result);
        }
    }
}
