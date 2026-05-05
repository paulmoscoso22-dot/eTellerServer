using FluentValidation;

namespace eTeller.Application.Features.Manager.Commands.Divise.CoppieDivise.DeleteCurrencyCouple
{
    public class DeleteCurrencyCoupleCommandValidator : AbstractValidator<DeleteCurrencyCoupleCommand>
    {
        public DeleteCurrencyCoupleCommandValidator()
        {
            RuleFor(x => x.CucCur1).NotEmpty().WithMessage("Divisa 1 è obbligatoria.");
            RuleFor(x => x.CucCur2).NotEmpty().WithMessage("Divisa 2 è obbligatoria.");
            //RuleFor(x => x.TraUser).NotEmpty().WithMessage("TraUser è obbligatorio.");
            //RuleFor(x => x.TraStation).NotEmpty().WithMessage("TraStation è obbligatoria.");
        }
    }
}
