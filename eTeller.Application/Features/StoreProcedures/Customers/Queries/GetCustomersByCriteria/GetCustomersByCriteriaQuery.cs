using eTeller.Application.Mappings;
using MediatR;

namespace eTeller.Application.Features.StoreProcedures.Customers.Queries.GetCustomersByCriteria
{
    public record GetCustomersByCriteriaQuery(
        string? CliId,
        string? Descrizione
    ) : IRequest<IEnumerable<CustomersVm>>;
}
