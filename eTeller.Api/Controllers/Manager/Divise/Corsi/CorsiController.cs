using eTeller.Application.Features.Manager.Queries.Divise.Corsi.GetAllCorsi;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace eTeller.Api.Controllers.Manager.Divise.Corsi
{
    [Route("api/[controller]")]
    [ApiController]
    public class CorsiController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CorsiController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("GetAll")]
        public async Task<IActionResult> GetAll([FromBody] GetAllCorsiQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }
    }
}
