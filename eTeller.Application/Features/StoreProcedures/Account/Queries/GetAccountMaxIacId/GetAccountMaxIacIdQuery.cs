using MediatR;
using eTeller.Application.Mappings;

namespace eTeller.Application.Features.StoreProcedures.Account.Queries.GetAccountMaxIacId
{
    public record GetAccountMaxIacIdQuery() : IRequest<int>;
}
