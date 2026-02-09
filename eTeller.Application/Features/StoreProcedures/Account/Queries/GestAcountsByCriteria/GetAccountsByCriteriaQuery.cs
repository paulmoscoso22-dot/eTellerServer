using eTeller.Application.Mappings;
using MediatR;

namespace eTeller.Application.Features.StoreProcedures.Account.Queries.GestAcountsByCriteria
{
    public record GetAccountsByCriteriaQuery(string accType, string branch, string cliId, string currency, string currencyType): IRequest<IEnumerable<AccountVm>>;
}
