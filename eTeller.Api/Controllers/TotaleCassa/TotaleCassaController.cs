using eTeller.Application.Features.StoreProcedures.TotalicCassa.Queries.GetTotaliCassa;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace eTeller.Api.Controllers.TotaleCassa
{
    [Route("api/[controller]")]
    [ApiController]
    public class TotaleCassaController : ControllerBase
    {
        private readonly IMediator _mediator;

        public TotaleCassaController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        [Route("GetTotaliCassa")]
        public async Task<IActionResult> GetTotaliCassa([FromBody] GetTotaliCassaQuery request)
        {
            var result = await _mediator.Send(request);
            Console.WriteLine($"TotaliCassa result: {request}");
            return Ok(result);
        }
    }
}
