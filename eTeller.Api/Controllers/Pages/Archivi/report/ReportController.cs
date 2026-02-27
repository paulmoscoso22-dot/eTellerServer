using eTeller.Application.Features.Archivi.Report.StoreProcedure.Queries.GetGiornaleCassa;
using eTeller.Application.Features.Archivi.Report.StoreProcedure.Queries.GetOperazioniAnnullate;
using eTeller.Application.Features.Archivi.Report.StoreProcedure.Queries.GetTotaliCassa;
using eTeller.Application.Features.Archivi.Report.StoreProcedure.Queries.GetTransactionWithFilters;
using eTeller.Application.Features.StoreProcedures.TransactionMov.Queries.GetTransactionMovByTrxId;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace eTeller.Api.Controllers.Pages.Archivi.report
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportController : ControllerBase
    {
        private readonly IMediator _mediator;
        public ReportController(IMediator mediator)
        {
            _mediator = mediator;
        }

        //Nota: pagine  
        //attesa - benefondo
        //giornale cassa
        //operazioni annullate

        [HttpPost]
        [Route("WithFilters")]
        public async Task<IActionResult> GetSpTransactionWithFilters([FromBody] GetSpTransactionWithFiltersQuery request)
        {
            var result = await _mediator.Send(request);
            return Ok(result);
        }

        [HttpPost]
        [Route("GetTotaliCassa")]
        public async Task<IActionResult> GetTotaliCassa([FromBody] GetSpTotaliCassaQuery request)
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
        [Route("GetSpGiornaleCassa")]
        public async Task<IActionResult> GetSpGiornaleCassa([FromBody] GetSpGiornaleCassaQuery request)
        {
            var result = await _mediator.Send(request);
            return Ok(result);
        }

        [HttpPost]
        [Route("GetSpOperazioniAnnullate")]
        public async Task<IActionResult> GetSpOperazioniAnnullate([FromBody] GetSpOperazioniAnnullateQuery request)
        {
            var result = await _mediator.Send(request);
            return Ok(result);
        }
    }
}
