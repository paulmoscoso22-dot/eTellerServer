using AutoMapper;
using eTeller.Application.Contracts;
using eTeller.Application.Mappings.ST_OperationType;
using eTeller.Domain.Models;
using MediatR;
using Microsoft.Extensions.Logging;

namespace eTeller.Application.Features.Tabella.Queries.GetOperationTypes
{
    public class GetOperationTypesQueryHandler : IRequestHandler<GetOperationTypesQuery, IEnumerable<ST_OperationTypeVm>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<GetOperationTypesQueryHandler> _logger;

        public GetOperationTypesQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, ILogger<GetOperationTypesQueryHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<ST_OperationTypeVm>> Handle(GetOperationTypesQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling {QueryName}", nameof(GetOperationTypesQuery));

            var results = await _unitOfWork.Repository<ST_OperationType>().GetAllAsync();
            var vms = _mapper.Map<IEnumerable<ST_OperationTypeVm>>(results);

            _logger.LogInformation("Handled {QueryName}, returned {Count} items", nameof(GetOperationTypesQuery), vms?.Count() ?? 0);
            return vms;
        }
    }
}
