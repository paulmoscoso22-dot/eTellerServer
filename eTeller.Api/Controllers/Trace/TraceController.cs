using eTeller.Application.Features.StoreProcedures.Trace.Commands.InsertTrace;
using eTeller.Application.Features.Trace.Queries.GetTraceAll;
using eTeller.Application.Features.Trace.Queries.GetTraceById;
using eTeller.Application.Features.Trace.Queries.GetTraceFunction;
using eTeller.Application.Features.Trace.Queries.GetTraceWithFunction;
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

        [HttpPost]
        [Route("GetTraceAll")]
        public async Task<IActionResult> GetTraceAll([FromBody] GetTraceAllQuery request)
        {
            var result = await _mediator.Send(request);
            return Ok(result);
        }

        [HttpPost]
        [Route("GetTraceFunction")]
        public async Task<IActionResult> GetTraceFunction()
        {
            var result = await _mediator.Send(new GetTraceFunctionQuery());
            return Ok(result);
        }

        [HttpPost]
        [Route("TraceById")]
        public async Task<IActionResult> TraceById([FromBody] GetTraceByIdQuery request)
        {
            var result = await _mediator.Send(request);
            return Ok(result);
        }

        //[HttpPost]
        //[Route("GetAllTracerDesFuction")]
        //public async Task<IActionResult> GetAllTracerDesFuction([FromBody] GetTraceWithFunctionQuery request)
        //{
        //    var result = await _mediator.Send(request);
        //    return Ok(result);
        //}
    }
}
