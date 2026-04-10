using MediatR;
using eTeller.Application.Mappings.Manager;

namespace eTeller.Application.Features.Manager.Queries.Functions.GetFuncAccType
{
    public record GetFuncAccTypeQuery : IRequest<IEnumerable<StFunAcctypVm>>;
}