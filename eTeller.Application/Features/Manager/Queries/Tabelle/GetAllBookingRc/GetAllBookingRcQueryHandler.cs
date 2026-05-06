using AutoMapper;
using eTeller.Application.Contracts;
using eTeller.Application.Mappings.BookingRc;
using eTeller.Domain.Models;
using MediatR;
using Microsoft.Extensions.Logging;

namespace eTeller.Application.Features.Manager.Queries.Tabelle.GetAllBookingRc
{
    public class GetAllBookingRcQueryHandler : IRequestHandler<GetAllBookingRcQuery, IEnumerable<BookingRcVm>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<GetAllBookingRcQueryHandler> _logger;

        public GetAllBookingRcQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, ILogger<GetAllBookingRcQueryHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<BookingRcVm>> Handle(GetAllBookingRcQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling {QueryName}", nameof(GetAllBookingRcQuery));

            var result = await _unitOfWork.Repository<StBookingRc>().GetAsync(
                x => (string.IsNullOrEmpty(request.BrcCutId) || x.BrcCutId == request.BrcCutId) &&
                     (string.IsNullOrEmpty(request.BrcOptId) || x.BrcOptId == request.BrcOptId) &&
                     (string.IsNullOrEmpty(request.BrcActId) || x.BrcActId == request.BrcActId));

            _logger.LogInformation("Handled {QueryName}, returned {Count} items", nameof(GetAllBookingRcQuery), result.Count());
            return _mapper.Map<IEnumerable<BookingRcVm>>(result);
        }
    }
}
