using eTeller.Application.Features.User.Queries.GetActiveAndBlockedUsers;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace eTeller.Api.Controllers.User
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IMediator _mediator;

        public UserController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        [Route("GetActiveAndBlockedUsers")]
        public async Task<IActionResult> GetActiveAndBlockedUsers()
        {
            var result = await _mediator.Send(new GetActiveAndBlockedUsersQuery());
            return Ok(result);
        }
    }
}
