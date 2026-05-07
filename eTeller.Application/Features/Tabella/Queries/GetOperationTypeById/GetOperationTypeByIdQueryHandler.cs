using AutoMapper;
using eTeller.Application.Contracts;
using eTeller.Application.Mappings.ST_OperationType;
using eTeller.Domain.Models;
using MediatR;
using Microsoft.Extensions.Logging;

namespace eTeller.Application.Features.Tabella.Queries.GetOperationTypeById
{
    public class GetOperationTypeByIdQueryHandler : IRequestHandler<GetOperationTypeByIdQuery, ST_OperationTypeVm>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<GetOperationTypeByIdQueryHandler> _logger;

        public GetOperationTypeByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, ILogger<GetOperationTypeByIdQueryHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ST_OperationTypeVm> Handle(GetOperationTypeByIdQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling {QueryName} with OptId: {OptId}", nameof(GetOperationTypeByIdQuery), request.OptId);

            var result = await _unitOfWork.Repository<ST_OperationType>().GetAsync(x => x.OptId == request.OptId);
            var vm = _mapper.Map<ST_OperationTypeVm>(result?.FirstOrDefault());

            _logger.LogInformation("Handled {QueryName}", nameof(GetOperationTypeByIdQuery));
            return vm;
        }
    }
}
