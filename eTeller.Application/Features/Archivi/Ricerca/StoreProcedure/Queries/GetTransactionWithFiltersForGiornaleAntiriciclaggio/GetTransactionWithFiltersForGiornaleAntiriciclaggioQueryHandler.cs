using AutoMapper;
using eTeller.Application.Contracts;
using eTeller.Application.Mappings;
using MediatR;
using Microsoft.Extensions.Logging;

namespace eTeller.Application.Features.Archivi.Ricerca.StoreProcedure.Queries.GetTransactionWithFiltersForGiornaleAntiriciclaggio
{
    public class GetTransactionWithFiltersForGiornaleAntiriciclaggioQueryHandler : IRequestHandler<GetSpTransactionWithFiltersForGiornaleAntiriciclaggioQuery, IEnumerable<GiornaleAntiriciclaggioVm>>
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

        public async Task<IEnumerable<GiornaleAntiriciclaggioVm>> Handle(GetSpTransactionWithFiltersForGiornaleAntiriciclaggioQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling {QueryName} with parameters Cassa={Cassa}, Localita={Localita}, DataDal={DataDal}, DataAl={DataAl}",
                nameof(GetSpTransactionWithFiltersForGiornaleAntiriciclaggioQuery), request.trxCassa, request.trxLocalita, request.trxDataDal, request.trxDataAl);

            var dateFrom = NormalizeDateFrom(request.trxDataDal);
            var dateTo = NormalizeDateTo(request.trxDataAl);

            var giornaleData = await _unitOfWork.GiornaleAntiriciclaggioRepository.GetSpTransactionWithFiltersForGiornaleAntiriciclaggio(
                request.trxCassa,
                request.trxLocalita,
                dateFrom,
                dateTo,
                request.trxReverse,
                request.trxCutId,
                request.trxOptId,
                request.trxDivope,
                request.trxImpopeDA,
                request.trxImpopeA,
                request.arcAppName,
                request.arcForced);

            var giornaleVms = _mapper.Map<IEnumerable<GiornaleAntiriciclaggioVm>>(giornaleData);
            _logger.LogInformation("Handled {QueryName}, returned {Count} items", nameof(GetSpTransactionWithFiltersForGiornaleAntiriciclaggioQuery), giornaleVms?.Count() ?? 0);
            return giornaleVms;
        }

        private static DateTime NormalizeDateFrom(DateTime? dateFrom)
        {
            if (dateFrom == DateTime.MinValue || dateFrom is null)
                return DateTime.Now.AddDays(-30000);

            return dateFrom.Value;
        }

        private static DateTime NormalizeDateTo(DateTime? dateTo)
        {
            if (dateTo == DateTime.MinValue || dateTo is null)
                return DateTime.MaxValue;

            return new DateTime(dateTo!.Value.Year, dateTo.Value.Month, dateTo.Value.Day, 23, 59, 59);
        }
    }
}
