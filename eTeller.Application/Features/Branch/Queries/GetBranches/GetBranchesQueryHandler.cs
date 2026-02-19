using AutoMapper;
using eTeller.Application.Contracts;
using eTeller.Application.Mappings.Branch;
using BraModel = eTeller.Domain.Models;
using MediatR;
using Microsoft.Extensions.Logging;

namespace eTeller.Application.Features.Branch.Queries.GetBranches
{
    public class GetBranchesQueryHandler : IRequestHandler<GetBranchesQuery, IEnumerable<BranchVm>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<GetBranchesQueryHandler> _logger;

        public GetBranchesQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, ILogger<GetBranchesQueryHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<BranchVm>> Handle(GetBranchesQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling {QueryName}", nameof(GetBranchesQuery));
            
            var branchRepository = _unitOfWork.Repository<BraModel.Branch>();
            var branches = await branchRepository.GetAllAsync();
            
            var branchVms = _mapper.Map<IEnumerable<BranchVm>>(branches);
            _logger.LogInformation("Handled {QueryName}, returned {Count} items", nameof(GetBranchesQuery), branchVms?.Count() ?? 0);
            
            return branchVms;
        }
    }
}
