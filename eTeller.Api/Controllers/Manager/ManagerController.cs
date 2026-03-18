using eTeller.Application.Features.StoreProcedures.Manager.Queries.GetAllUsersByUsrId;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace eTeller.Api.Controllers.Manager
{
    [Route("api/[controller]")]
    [ApiController]
    public class ManagerController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ManagerController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        [Route("GetAllUsersByUsrId")]
        public async Task<IActionResult> GetAllUsersByUsrId(GetAllUsersByUsrIdQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpPost]
        [Route("GetSysFunctions")]
        public async Task<IActionResult> GetSysFunctions(eTeller.Application.Features.StoreProcedures.Manager.Queries.GetSysFunctions.GetSysFunctionsQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpPost]
        [Route("GetSysRoleByFunId")]
        public async Task<IActionResult> GetSysRoleByFunId(eTeller.Application.Features.StoreProcedures.Manager.Queries.GetSysRoleByFunId.GetSysRoleByFunIdQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpPost]
        [Route("GetUsersRoleFunId")]
        public async Task<IActionResult> GetUsersRoleFunId(eTeller.Application.Features.StoreProcedures.Manager.Queries.GetUsersRoleFunId.GetUsersRoleFunIdQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpPost]
        [Route("GetUsersAllAccess")]
        public async Task<IActionResult> GetUsersAllAccess(eTeller.Application.Features.StoreProcedures.Manager.Queries.GetUsersAllAccess.GetUsersAllAccessQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }
    }
}
