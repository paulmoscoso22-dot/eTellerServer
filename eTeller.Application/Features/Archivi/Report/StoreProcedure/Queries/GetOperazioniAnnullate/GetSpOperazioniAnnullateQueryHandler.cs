using AutoMapper;
using eTeller.Application.Contracts;
using eTeller.Application.Mappings.Transaction;
using MediatR;
using Microsoft.Extensions.Logging;

namespace eTeller.Application.Features.Archivi.Report.StoreProcedure.Queries.GetOperazioniAnnullate
{
    public class GetSpOperazioniAnnullateQueryHandler : IRequestHandler<GetSpOperazioniAnnullateQuery, IEnumerable<TransactionOperationAnnullateVm>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<GetSpOperazioniAnnullateQueryHandler> _logger;

        public GetSpOperazioniAnnullateQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, ILogger<GetSpOperazioniAnnullateQueryHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<TransactionOperationAnnullateVm>> Handle(GetSpOperazioniAnnullateQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling {QueryName} with Cassa={Cassa}, DataDal={DataDal}, DataAl={DataAl}, Status={Status}, BraId={BraId}",
                nameof(GetSpOperazioniAnnullateQuery), request.trxCassa, request.trxDataDal, request.trxDataAl, request.trxStatus, request.trxBraId);

            var transactions = await _unitOfWork.TransactionSpRepository.GetSpTransactionWithFilters(
                request.trxCassa,
                request.trxDataDal,
                request.trxDataAl,
                request.trxStatus,
                request.trxBraId);

            var result = _mapper.Map<IEnumerable<TransactionOperationAnnullateVm>>(transactions);

            _logger.LogInformation("Handled {QueryName}, returned {Count} items", nameof(GetSpOperazioniAnnullateQuery), result?.Count() ?? 0);

            return result;
        }
    }
}
