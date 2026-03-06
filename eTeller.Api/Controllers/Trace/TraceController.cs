using eTeller.Application.Features.StoreProcedures.Trace.Commands.InsertTrace;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace eTeller.Api.Controllers.Trace
{
    [Route("api/[controller]")]
    [ApiController]
    public class TraceController : ControllerBase
    {
        private readonly IMediator _mediator;

        public TraceController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        [Route("InsertTrace")]
        public async Task<IActionResult> InsertTrace([FromBody] InsertTraceCommand request)
        {
            var result = await _mediator.Send(request);
            return Ok(new { Success = result > 0 });
        }
    }
}
