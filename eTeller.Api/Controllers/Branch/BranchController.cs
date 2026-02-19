using eTeller.Application.Features.Branch.Queries.GetBranches;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace eTeller.Api.Controllers.Branch
{
    [Route("api/[controller]")]
    [ApiController]
    public class BranchController : ControllerBase
    {
        private readonly IMediator _mediator;

        public BranchController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        [Route("GetBranches")]
        public async Task<IActionResult> GetBranches()
        {
            var query = new GetBranchesQuery();
            var result = await _mediator.Send(query);
            return Ok(result);
        }
    }
}
