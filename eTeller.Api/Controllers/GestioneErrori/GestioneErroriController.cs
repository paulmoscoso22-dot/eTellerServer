using eTeller.Application.Features.GestioneErrori.Commands.DeleteGestioneErrori;
using eTeller.Application.Features.GestioneErrori.Commands.InsertGestioneErrori;
using eTeller.Application.Features.GestioneErrori.Commands.UpdateGestioneErrori;
using eTeller.Application.Features.GestioneErrori.Queries.GetForceCodes;
using eTeller.Application.Features.GestioneErrori.Queries.GetGestioneErrori;
using eTeller.Application.Features.GestioneErrori.Queries.GetGestioneErroriById;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace eTeller.Api.Controllers.GestioneErrori;

[Route("api/[controller]")]
[ApiController]
public class GestioneErroriController : ControllerBase
{
    private readonly IMediator _mediator;

    public GestioneErroriController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [Route("GetGestioneErrori")]
    public async Task<IActionResult> GetGestioneErrori([FromBody] GetGestioneErroriQuery request)
    {
        var result = await _mediator.Send(request);
        return Ok(result);
    }

    [HttpPost]
    [Route("GetGestioneErroriById")]
    public async Task<IActionResult> GetGestioneErroriById([FromBody] GetGestioneErroriByIdQuery request)
    {
        var result = await _mediator.Send(request);
        if (result is null)
            return NotFound();
        return Ok(result);
    }

    [HttpGet]
    [Route("GetForceCodes")]
    public async Task<IActionResult> GetForceCodes()
    {
        var result = await _mediator.Send(new GetForceCodesQuery());
        return Ok(result);
    }

    [HttpPost]
    [Route("InsertGestioneErrori")]
    public async Task<IActionResult> InsertGestioneErrori([FromBody] InsertGestioneErroriCommand request)
    {
        await _mediator.Send(request);
        return Ok();
    }

    [HttpPost]
    [Route("UpdateGestioneErrori")]
    public async Task<IActionResult> UpdateGestioneErrori([FromBody] UpdateGestioneErroriCommand request)
    {
        var result = await _mediator.Send(request);
        return Ok(result);
    }

    [HttpPost]
    [Route("DeleteGestioneErrori")]
    public async Task<IActionResult> DeleteGestioneErrori([FromBody] DeleteGestioneErroriCommand request)
    {
        var result = await _mediator.Send(request);
        return Ok(result);
    }
}
