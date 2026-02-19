using eTeller.Application.Features.ST_Option.Queries.GetStOperations;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace eTeller.Api.Controllers.StOperation
{
    [Route("api/[controller]")]
    [ApiController]
    public class StOperationController : ControllerBase
    {
        private readonly IMediator _mediator;

        public StOperationController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        [Route("GetStOperations")]
        public async Task<IActionResult> GetStOperations()
        {
            var query = new GetStOperationsQuery();
            var result = await _mediator.Send(query);
            return Ok(result);
        }
    }
}
