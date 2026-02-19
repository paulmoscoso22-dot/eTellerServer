using eTeller.Application.Features.StoreProcedures.Transaction.Queries.GetTransactionWaitingForBef;
using eTeller.Application.Features.StoreProcedures.Transaction.Queries.GetTransactionWithFiltersForGiornale;
using eTeller.Application.Features.StoreProcedures.Transaction.Queries.GetTransactionWithFilters;
using eTeller.Application.Features.StoreProcedures.Transaction.Queries.GetTransactionWithFiltersForGiornaleAntiriciclaggio;
using eTeller.Application.Features.StoreProcedures.TransactionMov.Queries.GetTransactionMovByTrxId;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace eTeller.Api.Controllers.Transaction
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionController : ControllerBase
    {
        private readonly IMediator _mediator;

        public TransactionController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        [Route("WaitingForBEF")]
        public async Task<IActionResult> GetTransactionWaitingForBEF([FromBody] GetTransactionWaitingForBefQuery request)
        {
            var result = await _mediator.Send(request);
            return Ok(result);
        }

        [HttpPost]
        [Route("WithFiltersForGiornale")]
        public async Task<IActionResult> GetTransactionWithFiltersForGiornale([FromBody] GetTransactionWithFiltersForGiornaleQuery request)
        {
            var result = await _mediator.Send(request);
            return Ok(result);
        }

        [HttpPost]
        [Route("WithFilters")]
        public async Task<IActionResult> GetTransactionWithFilters([FromBody] GetTransactionWithFiltersQuery request)
        {
            var result = await _mediator.Send(request);
            return Ok(result);
        }

        [HttpPost]
        [Route("MovByTrxId")]
        public async Task<IActionResult> GetTransactionMovByTrxId([FromBody] GetTransactionMovByTrxIdQuery request)
        {
            var result = await _mediator.Send(request);
            return Ok(result);
        }

        [HttpPost]
        [Route("WithFiltersForGiornaleAntiriciclaggio")]
        public async Task<IActionResult> GetTransactionWithFiltersForGiornaleAntiriciclaggio([FromBody] GetTransactionWithFiltersForGiornaleAntiriciclaggioQuery request)
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
