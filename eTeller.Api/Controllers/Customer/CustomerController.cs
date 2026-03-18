using eTeller.Application.Features.StoreProcedures.Customers.Queries.GetCustomersByCriteria;
using eTeller.Application.Features.StoreProcedures.CustomerAccounts.Queries.GetCustomerAccountsByCliId;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace eTeller.Api.Controllers.Customer
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CustomerController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        [Route("GetCustomerByCriteria")]
        public async Task<IActionResult> GetCustomerByCriteria([FromBody] GetCustomersByCriteriaQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpPost]
        [Route("GetCustomerAccountsByCliId")]
        public async Task<IActionResult> GetCustomerAccountsByCliId([FromBody] GetCustomerAccountsByCliIdQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }
    }
}
