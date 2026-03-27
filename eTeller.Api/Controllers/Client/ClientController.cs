using eTeller.Application.Features.Client.Queries.GetClient;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace eTeller.Api.Controllers.Client
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ClientController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        [Route("GetClient")]
        public async Task<IActionResult> GetClient()
        {
            var result = await _mediator.Send(new GetClientQuery());
            return Ok(result);
        }
    }
}
