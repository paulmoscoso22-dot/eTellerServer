using AutoMapper;
using eTeller.Application.Contracts;
using eTeller.Application.Exceptions;
using eTeller.Application.Mappings.Branch;
using BraModel = eTeller.Domain.Models;
using MediatR;
using Microsoft.Extensions.Logging;

namespace eTeller.Application.Features.Branch.Queries.GetBranchById
{
    public class GetBranchByIdQueryHandler : IRequestHandler<GetBranchByIdQuery, IEnumerable<BranchVm>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<GetBranchByIdQueryHandler> _logger;

        public GetBranchByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, ILogger<GetBranchByIdQueryHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<BranchVm>> Handle(GetBranchByIdQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling {QueryName} for BraId: {BraId}", nameof(GetBranchByIdQuery), request.BraId);

            var repo = _unitOfWork.Repository<BraModel.Branch>();
            var results = await repo.GetAsync(b => b.BraId == request.BraId);

            if (results == null || !results.Any())
            {
                _logger.LogWarning("No branch found for BraId: {BraId}", request.BraId);
                throw new NotFoundException(nameof(GetBranchByIdQuery), request.BraId);
            }

            var vms = _mapper.Map<IEnumerable<BranchVm>>(results);
            _logger.LogInformation("Retrieved {Count} branches for BraId: {BraId}", vms.Count(), request.BraId);

            return vms;
        }
    }
}
