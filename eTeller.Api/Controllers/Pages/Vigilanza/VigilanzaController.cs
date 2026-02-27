using eTeller.Application.Features.StoreProcedures.Vigilanza.Queries.GetTransactionsForGiornaleAntiriciclaggio;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace eTeller.Api.Controllers.Pages.Vigilanza
{
    [Route("api/[controller]")]
    [ApiController]
    public class VigilanzaController : ControllerBase
    {
        private readonly IMediator _mediator;

        public VigilanzaController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        [Route("TransactionsForGiornaleAntiriciclaggio")]
        public async Task<IActionResult> TransactionsForGiornaleAntiriciclaggio([FromBody] GetTransactionsForGiornaleAntiriciclaggioQuery request)
        {
            try
            {
                var result = await _mediator.Send(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                // Log the exception (you can use a logging framework like Serilog, NLog, etc.)
                Console.WriteLine($"An error occurred: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request.");
            }
        }
    }
}
