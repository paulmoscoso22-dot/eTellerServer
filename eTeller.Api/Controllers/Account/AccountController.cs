using eTeller.Application.Features.Account.Queries;
using eTeller.Application.Features.Account.Queries.GetAccountByAccId;
using eTeller.Application.Features.Account.Queries.GetAccountByAccountIdAndType;
using eTeller.Application.Features.StoreProcedures.Account.Queries.GetAccountsByIacId;
using eTeller.Application.Features.StoreProcedures.Account.Queries.GetAccountsByPara;
using eTeller.Application.Features.StoreProcedures.Account.Queries.GetAccountsForBalance;
using eTeller.Application.Features.StoreProcedures.Account.Queries.GetAccountsForCheck;
using eTeller.Application.Features.StoreProcedures.Account.Queries.GetAccountMaxIacId;
using eTeller.Application.Features.StoreProcedures.Account.Queries;
using eTeller.Application.Features.StoreProcedures.Account.Queries.GestAcountsByCriteria;
using eTeller.Application.Features.StoreProcedures.Account.Commands.UpdateAccount;
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
        [Route("IacId")]
        public async Task<IActionResult> GetAccountsByIacId([FromBody] int iacId)
        {
            var query = new GetAccountsByIacIdQuery(iacId);
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpPost]
        [Route("Para")]
        public async Task<IActionResult> GetAccountsByPara([FromBody] GetAccountsByParaQuery request)
        {
            var result = await _mediator.Send(request);
            return Ok(result);
        }

        [HttpPost]
        [Route("ForBalance")]
        public async Task<IActionResult> GetAccountsForBalance([FromBody] GetAccountsForBalanceQuery request)
        {
            var result = await _mediator.Send(request);
            return Ok(result);
        }

        [HttpPost]
        [Route("ForCheck")]
        public async Task<IActionResult> GetAccountsForCheck([FromBody] GetAccountsForCheckQuery request)
        {
            var result = await _mediator.Send(request);
            return Ok(result);
        }

        [HttpPost]
        [Route("MaxIacId")]
        public async Task<IActionResult> GetAccountMax()
        {
            var result = await _mediator.Send(new GetAccountMaxIacIdQuery());
            return Ok(result);
        }

        [HttpPost]
        [Route("Update")]
        public async Task<IActionResult> UpdateAccount([FromBody] UpdateAccountCommand command)
        {
            var result = await _mediator.Send(command);
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
