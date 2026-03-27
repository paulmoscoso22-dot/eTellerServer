using eTeller.Application.Features.StoreProcedures.Manager.Queries.GetAllUsersByUsrId;
using eTeller.Application.Features.StoreProcedures.Manager.Queries.GetSysFunctionByFunId;
using eTeller.Application.Features.StoreProcedures.Manager.Commands.InsertSysFunction;
using eTeller.Application.Features.StoreProcedures.Manager.Commands.UpdateSysFunction;
using eTeller.Application.Features.StoreProcedures.Manager.Commands.DeleteSysFunction;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using eTeller.Application.Features.StoreProcedures.Manager.Queries.GetSysFunctions;

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
        public async Task<IActionResult> GetSysFunctions(GetSysFunctionsQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpPost]
        [Route("GetSysFunctionByFunId")]
        public async Task<IActionResult> GetSysFunctionByFunId(GetSysFunctionByFunIdQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpPost]
        [Route("InsertSysFunction")]
        public async Task<IActionResult> InsertSysFunction(InsertSysFunctionCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpPut]
        [Route("UpdateSysFunction")]
        public async Task<IActionResult> UpdateSysFunction(UpdateSysFunctionCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpDelete]
        [Route("DeleteSysFunction")]
        public async Task<IActionResult> DeleteSysFunction(DeleteSysFunctionCommand command)
        {
            var result = await _mediator.Send(command);
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
