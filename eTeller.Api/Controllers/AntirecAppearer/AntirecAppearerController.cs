using eTeller.Application.Features.StoreProcedures.AntirecAppearer.Queries.GetAntirecAppearerByAraId;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace eTeller.Api.Controllers.AntirecAppearer
{
    [Route("api/[controller]")]
    [ApiController]
    public class AntirecAppearerController : ControllerBase
    {
        private readonly IMediator _mediator;
        public AntirecAppearerController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("by-area/{araId:int}")]
        public async Task<IActionResult> GetAppearerByAraId(int araId)
        {
            var query = new GetAntirecAppearerByAraIdQuery(araId);
            var result = await _mediator.Send(query);
            return Ok(result);
        }
    }
}
