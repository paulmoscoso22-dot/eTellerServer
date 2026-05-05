using AutoMapper;
using eTeller.Application.Contracts;
using eTeller.Application.Mappings.Manager;
using MediatR;
using Microsoft.Extensions.Logging;

namespace eTeller.Application.Features.Manager.Queries.Divise.Corsi.GetAllCorsi
{
    public class GetAllCorsiQueryHandler : IRequestHandler<GetAllCorsiQuery, IEnumerable<CorsiVm>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<GetAllCorsiQueryHandler> _logger;

        public GetAllCorsiQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, ILogger<GetAllCorsiQueryHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<CorsiVm>> Handle(GetAllCorsiQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling {QueryName}", nameof(GetAllCorsiQuery));
            var result = await _unitOfWork.CorsiRepository.GetAllAsync(
                request.CurId,
                request.CurLondes,
                request.CurCutId,
                request.DateFrom,
                request.DateTo);
            return _mapper.Map<IEnumerable<CorsiVm>>(result);
        }
    }
}
