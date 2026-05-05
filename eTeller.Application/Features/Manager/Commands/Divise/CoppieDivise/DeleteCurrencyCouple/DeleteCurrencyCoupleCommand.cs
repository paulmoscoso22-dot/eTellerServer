using MediatR;

namespace eTeller.Application.Features.Manager.Commands.Divise.CoppieDivise.DeleteCurrencyCouple
{
    public record DeleteCurrencyCoupleCommand(
        string CucCur1,
        string CucCur2,
        string TraUser,
        string TraStation
    ) : IRequest<bool>;
}
