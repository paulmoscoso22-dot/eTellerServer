using eTeller.Application.Features.Help.Queries.GetHelpInfo;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace eTeller.Api.Controllers.Help
{
    [Route("api/[controller]")]
    [Tags("Help")]
    [ApiController]
    [Authorize]
    public class HelpController : ControllerBase
    {
        private readonly IMediator _mediator;

        public HelpController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("info-base")]
        public async Task<IActionResult> GetInfoBase()
        {
            var cliId = User.FindFirst("cash_desk_id")?.Value ?? string.Empty;
            var canUseTeller = bool.TryParse(User.FindFirst("can_use_teller")?.Value, out var flag) && flag;

            var query = new GetHelpInfoQuery(cliId, canUseTeller);
            var result = await _mediator.Send(query);
            return Ok(result);
        }
    }
}
