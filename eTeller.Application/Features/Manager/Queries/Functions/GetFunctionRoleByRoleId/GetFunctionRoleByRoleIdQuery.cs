using MediatR;
using eTeller.Application.Mappings.Manager;

namespace eTeller.Application.Features.Manager.Queries.Functions.GetFunctionRoleByRoleId
{
    public record GetFunctionRoleByRoleIdQuery(
        int RoleId,
        string? FunLikeName,
        string? FunLikeDes
    ) : IRequest<IEnumerable<FunctionRoleVm>>;
}
