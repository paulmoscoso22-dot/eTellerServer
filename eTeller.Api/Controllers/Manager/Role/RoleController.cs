using eTeller.Application.Features.Manager.Commands.Roles.DeleteRole;
using eTeller.Application.Features.Manager.Commands.Roles.InsertRole;
using eTeller.Application.Features.Manager.Commands.Roles.UpdateRole;
using eTeller.Application.Features.Manager.Queries.Roles.GetAllRole;
using eTeller.Application.Features.Manager.Queries.Roles.GetRoleByName;
using eTeller.Application.Features.Manager.Queries.Roles.GetSysRoleByFunId;
using eTeller.Application.Features.Manager.Queries.Roles.GetUserByRole;
using eTeller.Application.Features.Manager.Queries.Users.GetUsersRoleFunId;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using eTeller.Application.Features.Manager.Queries.Roles.GetRoleByUsrId;
using eTeller.Application.Features.Manager.Queries.Roles.GetRoleNotForUsrId;

namespace eTeller.Api.Controllers.Manager.Role
{
    [Route("api/manager/[controller]")]
    [Tags("Manager/Role")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly IMediator _mediator;

        public RoleController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        [Route("GetAllRole")]
        public async Task<IActionResult> GetAllRole()
        {
            var query = new GetAllRoleQuery();
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpPost]
        [Route("InsertRole")]
        public async Task<IActionResult> InsertRole([FromBody] InsertRoleCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpPut]
        [Route("UpdateRole")]
        public async Task<IActionResult> UpdateRole([FromBody] UpdateRoleCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpDelete]
        [Route("DeleteRole")]
        public async Task<IActionResult> DeleteRole([FromBody] DeleteRoleCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpPost]
        [Route("GetRoleByName")]
        public async Task<IActionResult> GetRoleByName([FromBody] GetRoleByNameQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpPost]
        [Route("GetUserByRole")]
        public async Task<IActionResult> GetUserByRole([FromBody] GetUserByRoleQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpPost]
        [Route("GetSysRoleByFunId")]
        public async Task<IActionResult> GetSysRoleByFunId(GetSysRoleByFunIdQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpPost]
        [Route("GetUsersRoleFunId")]
        public async Task<IActionResult> GetUsersRoleFunId(GetUsersRoleFunIdQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpPost]
        [Route("GetRoleNotForUsrId")]
        public async Task<IActionResult> GetRoleNotForUsrId(GetRoleNotForUsrIdQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpPost]
        [Route("GetRoleByUsrId")]
        public async Task<IActionResult> GetRoleByUsrId(GetRoleByUsrIdQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }
    }
}
