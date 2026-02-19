using eTeller.Application.Mappings.Branch;
using MediatR;

namespace eTeller.Application.Features.Branch.Queries.GetBranches
{
    public class GetBranchesQuery : IRequest<IEnumerable<BranchVm>>
    {
    }
}
