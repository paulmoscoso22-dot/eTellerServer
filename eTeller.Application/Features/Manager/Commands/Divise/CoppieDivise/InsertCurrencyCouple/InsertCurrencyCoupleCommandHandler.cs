using eTeller.Application.Contracts;
using eTeller.Application.Mappings.CurrencyCouple;
using MediatR;
using Microsoft.Extensions.Logging;

namespace eTeller.Application.Features.Manager.Commands.Divise.CoppieDivise.InsertCurrencyCouple
{
    public class InsertCurrencyCoupleCommandHandler : IRequestHandler<InsertCurrencyCoupleCommand, CurrencyCoupleVm>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<InsertCurrencyCoupleCommandHandler> _logger;

        public InsertCurrencyCoupleCommandHandler(IUnitOfWork unitOfWork, ILogger<InsertCurrencyCoupleCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<CurrencyCoupleVm> Handle(InsertCurrencyCoupleCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling {CommandName} for {Cur1}/{Cur2} by {User}",
                nameof(InsertCurrencyCoupleCommand), request.CucCur1, request.CucCur2, request.TraUser);

            var result = await _unitOfWork.CurrencyCoupleRepository.InsertAsync(
                request.CucCur1, request.CucCur2,
                request.CucLondes, request.CucShodes,
                request.CucSize, request.CucExcdir);

            await _unitOfWork.TraceRepository.InsertTrace(
                DateTime.UtcNow, request.TraUser, "CURRENCY_COUPLE", "INSERT",
                request.TraStation, "CURRENCY_COUPLE",
                $"{request.CucCur1}_{request.CucCur2}",
                string.Empty, $"Insert CurrencyCouple {request.CucCur1}/{request.CucCur2}",
                string.Empty, false);

            return result;
        }
    }
}
