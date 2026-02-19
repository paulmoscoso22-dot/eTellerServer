using AutoMapper;
using eTeller.Application.Contracts;
using eTeller.Application.Mappings.ST_OperationType;
using StOptModel = eTeller.Domain.Models;
using MediatR;
using Microsoft.Extensions.Logging;

namespace eTeller.Application.Features.ST_Option.Queries.GetStOperations
{
    public class GetStOperationsQueryHandler : IRequestHandler<GetStOperationsQuery, IEnumerable<ST_OperationTypeVm>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<GetStOperationsQueryHandler> _logger;

        public GetStOperationsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, ILogger<GetStOperationsQueryHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<ST_OperationTypeVm>> Handle(GetStOperationsQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling {QueryName}", nameof(GetStOperationsQuery));
            
            var optionRepository = _unitOfWork.Repository<StOptModel.ST_OperationType>();
            var options = await optionRepository.GetAllAsync();
            
            var optionVms = _mapper.Map<IEnumerable<ST_OperationTypeVm>>(options);
            _logger.LogInformation("Handled {QueryName}, returned {Count} items", nameof(GetStOperationsQuery), optionVms?.Count() ?? 0);
            
            return optionVms;
        }
    }
}
