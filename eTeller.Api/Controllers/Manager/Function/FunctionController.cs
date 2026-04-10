using eTeller.Application.Features.Manager.Commands.Functions.DeleteSysFunction;
using eTeller.Application.Features.Manager.Commands.Functions.InsertSysFunction;
using eTeller.Application.Features.Manager.Commands.Functions.UpdateSysFunction;
using eTeller.Application.Features.Manager.Queries.Functions.GetFuncAccType;
using eTeller.Application.Features.Manager.Queries.Functions.GetFunctionRoleByRoleId;
using eTeller.Application.Features.Manager.Queries.Functions.GetSysFunctionByFunId;
using eTeller.Application.Features.Manager.Queries.Functions.GetSysFunctions;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace eTeller.Api.Controllers.Manager.Function
{
    [Route("api/manager/[controller]")]
    [Tags("Manager/Function")]

    [ApiController]
    public class FunctionController : ControllerBase
    {
        private readonly IMediator _mediator;
        // Constructor to inject the IMediator dependency

        public FunctionController(IMediator mediator) {
            _mediator = mediator;
        }

        [HttpPost]
        [Route("GetSysFunctions")]
        public async Task<IActionResult> GetSysFunctions(GetSysFunctionsQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }


        [HttpPost]
        [Route("GetFunctionRoleByRoleId")]
        public async Task<IActionResult> GetFunctionRoleByRoleId([FromBody] GetFunctionRoleByRoleIdQuery query)
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
        [Route("GetFuncAccType")]
        public async Task<IActionResult> GetFuncAccType()
        {
            var query = new GetFuncAccTypeQuery();
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
    }
}
