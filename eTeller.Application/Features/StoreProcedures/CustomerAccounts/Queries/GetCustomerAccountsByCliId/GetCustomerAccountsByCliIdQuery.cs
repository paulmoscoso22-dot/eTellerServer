using eTeller.Application.Mappings.Account;
using MediatR;

namespace eTeller.Application.Features.StoreProcedures.CustomerAccounts.Queries.GetCustomerAccountsByCliId
{
    public record GetCustomerAccountsByCliIdQuery(
        string CliId
    ) : IRequest<IEnumerable<CustomerAccountVm>>;
}
