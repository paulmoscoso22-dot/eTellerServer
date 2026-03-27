using eTeller.Application.Mappings.Vigilanza;
using MediatR;

namespace eTeller.Application.Features.StoreProcedures.Vigilanza.Queries.GetSpAntirecRulesByParameters
{
    public record GetSpAntirecRulesByParametersQuery(
        string? arlOpTypeId,
        string? arlCurTypeId,
        string? arlAcctId,
        string? arlAcctType
        ) : IRequest<IEnumerable<SpAntirecRulesVm>>;
}
