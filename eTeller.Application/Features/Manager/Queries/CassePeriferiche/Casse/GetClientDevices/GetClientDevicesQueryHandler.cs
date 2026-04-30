using eTeller.Application.Contracts;
using MediatR;
using Microsoft.Extensions.Logging;

namespace eTeller.Application.Features.Manager.Queries.CassePeriferiche.Casse.GetClientDevices
{
    public class GetClientDevicesQueryHandler : IRequestHandler<GetClientDevicesQuery, IEnumerable<int>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<GetClientDevicesQueryHandler> _logger;

        public GetClientDevicesQueryHandler(IUnitOfWork unitOfWork, ILogger<GetClientDevicesQueryHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<IEnumerable<int>> Handle(GetClientDevicesQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling {QueryName} for CliId={CliId}", nameof(GetClientDevicesQuery), request.CliId);

            var assignments = await _unitOfWork.Repository<Domain.Models.ClientDevice>()
                .GetAsync(cd => cd.CliId == request.CliId);

            var devIds = assignments.Select(cd => cd.DevId).ToList();
            _logger.LogInformation("Handled {QueryName}, returned {Count} device assignments", nameof(GetClientDevicesQuery), devIds.Count);
            return devIds;
        }
    }
}
