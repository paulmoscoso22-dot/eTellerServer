using eTeller.Application.Features.Tabella.Queries.GetTabellaServVarchar;
using eTeller.Application.Features.Tabella.Queries.GetTabellaServVarcharById;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace eTeller.Api.Controllers.Tabella
{
    [Route("api/[controller]")]
    [ApiController]
    public class TabellaController : ControllerBase
    {
        private readonly IMediator _mediator;

        public TabellaController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        [Route("GetTabellaServVarchar")]
        public async Task<IActionResult> GetTabellaServVarchar([FromBody] GetTabellaServVarcharQuery request)
        {
            var result = await _mediator.Send(request);
            return Ok(result);
        }

        [HttpPost]
        [Route("GetTabellaServVarcharById")]
        public async Task<IActionResult> GetTabellaServVarcharById([FromBody] GetTabellaServVarcharByIdQuery request)
        {
            var result = await _mediator.Send(request);
            return Ok(result);
        }
    }
}
