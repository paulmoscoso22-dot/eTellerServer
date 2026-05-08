using eTeller.Application.Features.Tabella.Commands.InsertOperationType;
using eTeller.Application.Features.Tabella.Commands.InsertTabellaServInt;
using eTeller.Application.Features.Tabella.Commands.InsertTabellaServVarchar;
using eTeller.Application.Features.Tabella.Commands.UpdateOperationType;
using eTeller.Application.Features.Tabella.Commands.UpdateTabellaServInt;
using eTeller.Application.Features.Tabella.Commands.UpdateTabellaServVarchar;
using eTeller.Application.Features.Tabella.Queries.GetOperationTypeById;
using eTeller.Application.Features.Tabella.Queries.GetOperationTypes;
using eTeller.Application.Features.Tabella.Queries.GetTabellaServInt;
using eTeller.Application.Features.Tabella.Queries.GetTabellaServVarchar;
using eTeller.Application.Features.Tabella.Queries.GetTabellaServVarcharById;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace eTeller.Api.Controllers.Tabella
{
    [Route("api/[controller]")]
    [ApiController]
    public class TabellaController : ControllerBase
    {
        private readonly IMediator _mediator;

        public TabellaController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        [Route("GetTabellaServVarchar")]
        public async Task<IActionResult> GetTabellaServVarchar([FromBody] GetTabellaServVarcharQuery request)
        {
            var result = await _mediator.Send(request);
            return Ok(result);
        }

        [HttpPost]
        [Route("GetTabellaServVarcharById")]
        public async Task<IActionResult> GetTabellaServVarcharById([FromBody] GetTabellaServVarcharByIdQuery request)
        {
            var result = await _mediator.Send(request);
            return Ok(result);
        }

        [HttpPost]
        [Route("InsertTabellaServVarchar")]
        public async Task<IActionResult> InsertTabellaServVarchar([FromBody] InsertTabellaServVarcharCommand request)
        {
            var result = await _mediator.Send(request);
            return Ok(result);
        }

        [HttpPost]
        [Route("UpdateTabellaServVarchar")]
        public async Task<IActionResult> UpdateTabellaServVarchar([FromBody] UpdateTabellaServVarcharCommand request)
        {
            var result = await _mediator.Send(request);
            return Ok(result);
        }

        [HttpPost]
        [Route("GetTabellaServInt")]
        public async Task<IActionResult> GetTabellaServInt([FromBody] GetTabellaServIntQuery request)
        {
            var result = await _mediator.Send(request);
            return Ok(result);
        }

        [HttpPost]
        [Route("InsertTabellaServInt")]
        public async Task<IActionResult> InsertTabellaServInt([FromBody] InsertTabellaServIntCommand request)
        {
            var result = await _mediator.Send(request);
            return Ok(result);
        }

        [HttpPost]
        [Route("UpdateTabellaServInt")]
        public async Task<IActionResult> UpdateTabellaServInt([FromBody] UpdateTabellaServIntCommand request)
        {
            var result = await _mediator.Send(request);
            return Ok(result);
        }

        [HttpPost]
        [Route("GetOperationTypes")]
        public async Task<IActionResult> GetOperationTypes([FromBody] GetOperationTypesQuery request)
        {
            var result = await _mediator.Send(request);
            return Ok(result);
        }

        [HttpPost]
        [Route("GetOperationTypeById")]
        public async Task<IActionResult> GetOperationTypeById([FromBody] GetOperationTypeByIdQuery request)
        {
            var result = await _mediator.Send(request);
            return Ok(result);
        }

        [HttpPost]
        [Route("InsertOperationType")]
        public async Task<IActionResult> InsertOperationType([FromBody] InsertOperationTypeCommand request)
        {
            var result = await _mediator.Send(request);
            return Ok(result);
        }

        [HttpPost]
        [Route("UpdateOperationType")]
        public async Task<IActionResult> UpdateOperationType([FromBody] UpdateOperationTypeCommand request)
        {
            var result = await _mediator.Send(request);
            return Ok(result);
        }
    }
}

