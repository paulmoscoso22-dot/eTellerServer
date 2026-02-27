using AutoMapper;
using eTeller.Application.Contracts;
using eTeller.Application.Mappings.Transaction;
using MediatR;
using Microsoft.Extensions.Logging;

namespace eTeller.Application.Features.Archivi.Report.StoreProcedure.Queries.GetGiornaleCassa
{
    public class GetSpGiornaleCassaQueryHandler : IRequestHandler<GetSpGiornaleCassaQuery, IEnumerable<TransactionGiornaleCassaVm>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<GetSpGiornaleCassaQueryHandler> _logger;

        public GetSpGiornaleCassaQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, ILogger<GetSpGiornaleCassaQueryHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<TransactionGiornaleCassaVm>> Handle(GetSpGiornaleCassaQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling {QueryName} with Cassa={Cassa}, DataDal={DataDal}, DataAl={DataAl}, Status={Status}, BraId={BraId}",
                nameof(GetSpGiornaleCassaQuery), request.trxCassa, request.trxDataDal, request.trxDataAl, request.trxStatus, request.trxBraId);

            var transactions = await _unitOfWork.TransactionSpRepository.GetSpTransactionWithFilters(
                request.trxCassa,
                request.trxDataDal,
                request.trxDataAl,
                request.trxStatus,
                request.trxBraId);

            var result = _mapper.Map<IEnumerable<TransactionGiornaleCassaVm>>(transactions);

            _logger.LogInformation("Handled {QueryName}, returned {Count} items", nameof(GetSpGiornaleCassaQuery), result?.Count() ?? 0);

            return result;
        }
    }
}
