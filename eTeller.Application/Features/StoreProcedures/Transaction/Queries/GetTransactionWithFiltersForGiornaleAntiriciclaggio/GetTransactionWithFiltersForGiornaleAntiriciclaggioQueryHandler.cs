using AutoMapper;
using eTeller.Application.Contracts;
using eTeller.Application.Mappings;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Linq;

namespace eTeller.Application.Features.StoreProcedures.Transaction.Queries.GetTransactionWithFiltersForGiornaleAntiriciclaggio
{
    public class GetTransactionWithFiltersForGiornaleAntiriciclaggioQueryHandler : IRequestHandler<GetTransactionWithFiltersForGiornaleAntiriciclaggioQuery, IEnumerable<GiornaleAntiriciclaggioVm>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<GetTransactionWithFiltersForGiornaleAntiriciclaggioQueryHandler> _logger;

        public GetTransactionWithFiltersForGiornaleAntiriciclaggioQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, ILogger<GetTransactionWithFiltersForGiornaleAntiriciclaggioQueryHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<GiornaleAntiriciclaggioVm>> Handle(GetTransactionWithFiltersForGiornaleAntiriciclaggioQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling {QueryName} with parameters Cassa={Cassa}, Localita={Localita}, DataDal={DataDal}, DataAl={DataAl}",
                nameof(GetTransactionWithFiltersForGiornaleAntiriciclaggioQuery), request.trxCassa, request.trxLocalita, request.trxDataDal, request.trxDataAl);

            var giornaleData = await _unitOfWork.GiornaleAntiriciclaggioSpRepository.GetTransactionWithFiltersForGiornaleAntiriciclaggio(
                request.trxCassa,
                request.trxLocalita,
                request.trxDataDal,
                request.trxDataAl,
                request.trxReverse,
                request.trxCutId,
                request.trxOptId,
                request.trxDivope,
                request.trxImpopeDA,
                request.trxImpopeA,
                request.arcAppName,
                request.arcForced);

            var giornaleVms = _mapper.Map<IEnumerable<GiornaleAntiriciclaggioVm>>(giornaleData);
            _logger.LogInformation("Handled {QueryName}, returned {Count} items", nameof(GetTransactionWithFiltersForGiornaleAntiriciclaggioQuery), giornaleVms?.Count() ?? 0);
            return giornaleVms;
        }
    }
}
