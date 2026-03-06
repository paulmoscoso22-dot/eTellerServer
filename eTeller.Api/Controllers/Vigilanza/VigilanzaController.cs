using eTeller.Application.Features.StoreProcedures.Vigilanza.Queries.GetSpAntirecRulesByParameters;
using eTeller.Application.Features.StoreProcedures.Vigilanza.Queries.GetSpAntirecAppearerByParameters;
using eTeller.Application.Features.StoreProcedures.Vigilanza.Queries.GetAppearerByParametersWithExpiry;
using eTeller.Application.Features.StoreProcedures.Vigilanza.Queries.GetAppearerAllByAraId;
using eTeller.Application.Features.StoreProcedures.Vigilanza.Commands.InsertSpAntirecAppearer;
using eTeller.Application.Features.StoreProcedures.Vigilanza.Commands.InsertHisAntirecAppearer;
using eTeller.Application.Features.StoreProcedures.Vigilanza.Commands.InsertARA;
using eTeller.Application.Features.StoreProcedures.Vigilanza.Commands.UpdateARA;
using eTeller.Application.Features.StoreProcedures.Vigilanza.Commands.DeleteARA;
using eTeller.Application.Features.StoreProcedures.Vigilanza.Commands.UpdateSpAntirecAppearer;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace eTeller.Api.Controllers.Vigilanza
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
        [Route("GetSpAntirecRulesParameters")]
        public async Task<IActionResult> GetSpAntirecRulesParameters([FromBody] GetSpAntirecRulesByParametersQuery request)
        {
            var result = await _mediator.Send(request);
            return Ok(result);
        }

        [HttpPost]
        [Route("GetSpAntirecAppearerByParameters")]
        public async Task<IActionResult> GetSpAntirecAppearerByParameters([FromBody] GetSpAntirecAppearerByParametersQuery request)
        {
            var result = await _mediator.Send(request);
            return Ok(result);
        }

        [HttpPost]
        [Route("GetByParameters")]
        public async Task<IActionResult> GetByParameters([FromBody] GetAppearerByParametersWithExpiryQuery request)
        {
            var result = await _mediator.Send(request);
            return Ok(result);
        }

        [HttpPost]
        [Route("GetByAraId")]
        public async Task<IActionResult> GetByAraId([FromBody] GetAppearerAllByAraIdQuery request)
        {
            var result = await _mediator.Send(request);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        [HttpPut]
        [Route("InsertSpAntirecAppearer")]
        public async Task<IActionResult> InsertSpAntirecAppearer([FromBody] InsertSpAntirecAppearerCommand request)
        {
            var result = await _mediator.Send(request);
            return Ok(result);
        }

        [HttpPost]
        [Route("InsertHisAntirecAppearer")]
        public async Task<IActionResult> InsertHisAntirecAppearer([FromBody] InsertHisAntirecAppearerCommand request)
        {
            var hisId = await _mediator.Send(request);
            return Ok(new { HisId = hisId });
        }

        [HttpPut]
        [Route("InsertARA")]
        public async Task<IActionResult> InsertARA([FromBody] InsertARACommand request)
        {
            var result = await _mediator.Send(request);
            return Ok(new { Success = result });
        }

        [HttpPut]
        [Route("UpdateARA")]
        public async Task<IActionResult> UpdateARA([FromBody] UpdateARACommand request)
        {
            var result = await _mediator.Send(request);
            return Ok(new { AraId = result });
        }

        [HttpDelete]
        [Route("DeleteARA")]
        public async Task<IActionResult> DeleteARA([FromBody] DeleteARACommand request)
        {
            var result = await _mediator.Send(request);
            return Ok(new { Success = result });
        }

        [HttpPut]
        [Route("UpdateSpAntirecAppearer")]
        public async Task<IActionResult> UpdateSpAntirecAppearer([FromBody] UpdateSpAntirecAppearerCommand request)
        {
            var result = await _mediator.Send(request);
            return Ok(result);
        }
    }
}
