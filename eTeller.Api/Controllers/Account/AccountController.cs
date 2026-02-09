using eTeller.Application.Features.Account.Queries;
using eTeller.Application.Features.Account.Queries.GetAccountByAccId;
using eTeller.Application.Features.Account.Queries.GetAccountByAccountIdAndType;
using eTeller.Application.Features.StoreProcedures.Account.Queries;
using eTeller.Application.Features.StoreProcedures.Account.Queries.GestAcountsByCriteria;
using MediatR;
using Microsoft.AspNetCore.Mvc;
namespace eTeller.Api.Controllers.Account
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IMediator _mediator;
        public AccountController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> GetAccounts()
        {
            var query = new GetAccountQuery();
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpPost]
        [Route("AccId")]
        public async Task<IActionResult> GetAccountsByAccId([FromBody] string accId)
        {
            var query = new GetAccountsByAccIdQuery(accId);
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpPost]
        [Route("Account_Type")]
        public async Task<IActionResult> GetAccountsByAccountIdAndType([FromBody] string iacAccId, string iacActId, string iacHostprefix)
        {
            var query = new GetAccountsByAccountIdAndTypeQuery(iacAccId, iacActId, iacHostprefix);
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpPost]
        [Route("Account_criteria")]
        public async Task<IActionResult> GetAccountsByCriteria([FromBody] string accType, string branch, string cliId, string currency, string currencyType)
        {
            var query = new GetAccountsByCriteriaQuery(accType, branch, cliId, currency, currencyType);
            var result = await _mediator.Send(query);
            return Ok(result);
        }
    }
}
