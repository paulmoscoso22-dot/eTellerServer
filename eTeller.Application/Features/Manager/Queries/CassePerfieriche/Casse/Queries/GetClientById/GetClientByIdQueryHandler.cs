using AutoMapper;
using eTeller.Application.Contracts;
using eTeller.Application.Exceptions;
using eTeller.Application.Mappings.Client;
using MediatR;
using Microsoft.Extensions.Logging;
using ClientModel = eTeller.Domain.Models;

namespace eTeller.Application.Features.Client.Queries.GetClientById
{
    public class GetClientByIdQueryHandler : IRequestHandler<GetClientByIdQuery, IEnumerable<ClientVm>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<GetClientByIdQueryHandler> _logger;

        public GetClientByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, ILogger<GetClientByIdQueryHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<ClientVm>> Handle(GetClientByIdQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling {QueryName} for CliId: {CliId}", nameof(GetClientByIdQuery), request.CliId);

            var repo = _unitOfWork.Repository<ClientModel.Client>();
            var results = await repo.GetAsync(c => c.CliId == request.CliId);

            if (results == null || !results.Any())
            {
                _logger.LogWarning("No client found for CliId: {CliId}", request.CliId);
                throw new NotFoundException(nameof(GetClientByIdQuery), request.CliId);
            }

            var vms = _mapper.Map<IEnumerable<ClientVm>>(results);
            _logger.LogInformation("Retrieved {Count} client(s) for CliId: {CliId}", vms.Count(), request.CliId);

            return vms;
        }
    }
}
