using eTeller.Application.Mappings.Currency;
using MediatR;

namespace eTeller.Application.Features.Currency.Commands.UpdateCurrency
{
    public record UpdateCurrencyCommand(
        string CurId,
        string CurCutId,
        decimal CurMinamn,
        string CurFinezza,
        decimal CurTolrat,
        string TraUser,
        string TraStation
    ) : IRequest<CurrencyVm>;
}
