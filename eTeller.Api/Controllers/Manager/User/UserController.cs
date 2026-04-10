using eTeller.Application.Features.Manager.Commands.Users.InsertUser;
using eTeller.Application.Features.Manager.Commands.Users.ResetPassword;
using eTeller.Application.Features.Manager.Commands.Users.UpdateUser;
using eTeller.Application.Features.Manager.Queries.Users.GetAllUsersByUsrId;
using eTeller.Application.Features.Manager.Queries.Users.GetUserByHostId;
using eTeller.Application.Features.Manager.Queries.Users.GetUsersActiveAndBlocked;
using eTeller.Application.Features.Manager.Queries.Users.GetUsersAllAccess;
using eTeller.Application.Features.Manager.Queries.Users.GetUsersByUserId;
using eTeller.Application.Features.User.Queries.GetActiveAndBlockedUsers;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace eTeller.Api.Controllers.Manager.User
{
    [Route("api/manager/[controller]")]
    [Tags("Manager/User")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IValidator<InsertUserCommand> _validator;

        public UserController(IMediator mediator, IValidator<InsertUserCommand> validator) {
            _mediator = mediator;
            _validator = validator;
        }

        [HttpPost]
        [Route("GetAllUsersByUsrId")]
        public async Task<IActionResult> GetAllUsersByUsrId(GetAllUsersByUsrIdQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpPost]
        [Route("GetUsersAllAccess")]
        public async Task<IActionResult> GetUsersAllAccess(GetUsersAllAccessQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpPost]
        [Route("GetUsersActiveBlocked")]
        public async Task<IActionResult> GetUsersActiveBlocked()
        {
            var query = new GetUsersActiveAndBlockedQuery();
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpPost]
        [Route("GetUsersByUserId")]
        public async Task<IActionResult> GetUsersByUserId(GetUsersByUserIdQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpPost]
        [Route("GetUserByHostId")]
        public async Task<IActionResult> GetUserByHostId(GetUserByHostIdQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }
        [HttpPost]
        [Route("GetActiveAndBlockedUsers")]
        public async Task<IActionResult> GetActiveAndBlockedUsers()
        {
            var result = await _mediator.Send(new GetActiveAndBlockedUsersQuery());
            return Ok(result);
        }

        [HttpPost]
        [Route("InsertUser")]
        public async Task<IActionResult> InsertUser([FromBody] InsertUserCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpPut]
        [Route("UpdateUser")]
        public async Task<IActionResult> UpdateUser([FromBody] UpdateUserCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpPost]
        [Route("ResetPassword")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }
    }
}
