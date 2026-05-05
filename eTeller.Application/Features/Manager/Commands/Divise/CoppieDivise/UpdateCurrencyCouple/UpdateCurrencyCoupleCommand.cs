using eTeller.Application.Mappings.CurrencyCouple;
using MediatR;

namespace eTeller.Application.Features.Manager.Commands.Divise.CoppieDivise.UpdateCurrencyCouple
{
    public record UpdateCurrencyCoupleCommand(
        string CucCur1,
        string CucCur2,
        string? CucLondes,
        string? CucShodes,
        decimal? CucSize,
        string? CucExcdir,
        string TraUser,
        string TraStation
    ) : IRequest<CurrencyCoupleVm>;
}
