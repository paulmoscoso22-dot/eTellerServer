using AutoMapper;
using eTeller.Application.Contracts;
using eTeller.Application.Mappings.Device;
using MediatR;
using Microsoft.Extensions.Logging;

namespace eTeller.Application.Features.Manager.Queries.CassePeriferiche.Casse.GetDevicesByBranch
{
    public class GetDevicesByBranchQueryHandler : IRequestHandler<GetDevicesByBranchQuery, IEnumerable<DeviceVm>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<GetDevicesByBranchQueryHandler> _logger;

        public GetDevicesByBranchQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, ILogger<GetDevicesByBranchQueryHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<DeviceVm>> Handle(GetDevicesByBranchQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling {QueryName} for BraId={BraId}", nameof(GetDevicesByBranchQuery), request.BraId);

            var devices = await _unitOfWork.Repository<Domain.Models.Device>()
                .GetAsync(d => d.DevBraId == request.BraId);

            var vms = _mapper.Map<IEnumerable<DeviceVm>>(devices);
            _logger.LogInformation("Handled {QueryName}, returned {Count} devices", nameof(GetDevicesByBranchQuery), vms?.Count() ?? 0);
            return vms;
        }
    }
}
