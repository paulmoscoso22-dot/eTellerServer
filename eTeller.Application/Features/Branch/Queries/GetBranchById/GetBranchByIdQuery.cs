using MediatR;
using eTeller.Application.Mappings.Branch;

namespace eTeller.Application.Features.Branch.Queries.GetBranchById
{
    public class GetBranchByIdQuery : IRequest<IEnumerable<BranchVm>>
    {
        public string BraId { get; set; }

        public GetBranchByIdQuery(string braId)
        {
            BraId = braId;
        }
    }
}
