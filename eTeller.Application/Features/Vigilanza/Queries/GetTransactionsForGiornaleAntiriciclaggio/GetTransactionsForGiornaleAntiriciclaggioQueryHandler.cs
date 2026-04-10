using AutoMapper;
using eTeller.Application.Contracts;
using eTeller.Application.Mappings.Vigilanza;
using MediatR;
using Microsoft.Extensions.Logging;

namespace eTeller.Application.Features.StoreProcedures.Vigilanza.Queries.GetTransactionsForGiornaleAntiriciclaggio
{
    public class GetTransactionsForGiornaleAntiriciclaggioQueryHandler : IRequestHandler<GetTransactionsForGiornaleAntiriciclaggioQuery, IEnumerable<SpTransactionGiornaleAntiriciclagioVm>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<GetTransactionsForGiornaleAntiriciclaggioQueryHandler> _logger;

        public GetTransactionsForGiornaleAntiriciclaggioQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, ILogger<GetTransactionsForGiornaleAntiriciclaggioQueryHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<SpTransactionGiornaleAntiriciclagioVm>> Handle(GetTransactionsForGiornaleAntiriciclaggioQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling {QueryName} with parameters Cassa={Cassa}, Localita={Localita}, DataDal={DataDal}, DataAl={DataAl}",
                nameof(GetTransactionsForGiornaleAntiriciclaggioQuery), request.trxCassa, request.trxLocalita, request.trxDataDal, request.trxDataAl);

            var transactions = await _unitOfWork.VigilanzaRepository.GetTransactionsForGiornaleAntiriciclaggio(
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

            var transactionVms = _mapper.Map<IEnumerable<SpTransactionGiornaleAntiriciclagioVm>>(transactions);
            _logger.LogInformation("Handled {QueryName}, returned {Count} items", nameof(GetTransactionsForGiornaleAntiriciclaggioQuery), transactionVms?.Count() ?? 0);
            return transactionVms ?? Enumerable.Empty<SpTransactionGiornaleAntiriciclagioVm>();
        }
    }
}
