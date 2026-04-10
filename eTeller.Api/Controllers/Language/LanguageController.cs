using eTeller.Application.Features.Language.Queries.GetAllLanguages;
using eTeller.Application.Features.Language.Queries.GetLanguagesById;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace eTeller.Api.Controllers.Language
{
    [Route("api/[controller]")]
    [Tags("Language")]
    [ApiController]
    public class LanguageController : ControllerBase
    {
        private readonly IMediator _mediator;

        public LanguageController(IMediator mediator)
        {
            _mediator = mediator;
        }
        [HttpPost]
        [Route("GetLanguageById")]
        public async Task<IActionResult> GetLanguageById([FromBody] GetLanguageByIIdQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }
        
        [HttpPost]
        [Route("GetLanguages")]
        public async Task<IActionResult> GetLanguages()
        {
            var query = new GetAllLanguagesQuery();
            var result = await _mediator.Send(query);
            return Ok(result);
        }
    }
}
