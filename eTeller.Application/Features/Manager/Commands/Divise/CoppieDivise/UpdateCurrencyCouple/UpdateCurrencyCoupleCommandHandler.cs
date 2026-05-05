using eTeller.Application.Contracts;
using eTeller.Application.Mappings.CurrencyCouple;
using MediatR;
using Microsoft.Extensions.Logging;

namespace eTeller.Application.Features.Manager.Commands.Divise.CoppieDivise.UpdateCurrencyCouple
{
    public class UpdateCurrencyCoupleCommandHandler : IRequestHandler<UpdateCurrencyCoupleCommand, CurrencyCoupleVm>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<UpdateCurrencyCoupleCommandHandler> _logger;

        public UpdateCurrencyCoupleCommandHandler(IUnitOfWork unitOfWork, ILogger<UpdateCurrencyCoupleCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<CurrencyCoupleVm> Handle(UpdateCurrencyCoupleCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling {CommandName} for {Cur1}/{Cur2} by {User}",
                nameof(UpdateCurrencyCoupleCommand), request.CucCur1, request.CucCur2, request.TraUser);

            var result = await _unitOfWork.CurrencyCoupleRepository.UpdateAsync(
                request.CucCur1, request.CucCur2,
                request.CucLondes, request.CucShodes,
                request.CucSize, request.CucExcdir);

            await _unitOfWork.TraceRepository.InsertTrace(
                DateTime.UtcNow, request.TraUser, "CURRENCY_COUPLE", "UPDATE",
                request.TraStation, "CURRENCY_COUPLE",
                $"{request.CucCur1}_{request.CucCur2}",
                string.Empty, $"Update CurrencyCouple {request.CucCur1}/{request.CucCur2}",
                string.Empty, false);

            return result;
        }
    }
}
