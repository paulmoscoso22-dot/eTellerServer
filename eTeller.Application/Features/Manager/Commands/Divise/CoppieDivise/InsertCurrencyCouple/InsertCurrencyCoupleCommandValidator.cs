using FluentValidation;

namespace eTeller.Application.Features.Manager.Commands.Divise.CoppieDivise.InsertCurrencyCouple
{
    public class InsertCurrencyCoupleCommandValidator : AbstractValidator<InsertCurrencyCoupleCommand>
    {
        public InsertCurrencyCoupleCommandValidator()
        {
            RuleFor(x => x.CucCur1).NotEmpty().WithMessage("Divisa 1 è obbligatoria.");
            RuleFor(x => x.CucCur2).NotEmpty().WithMessage("Divisa 2 è obbligatoria.")
                .NotEqual(x => x.CucCur1).WithMessage("Divisa 1 e Divisa 2 devono essere diverse.");
            //RuleFor(x => x.TraUser).NotEmpty().WithMessage("TraUser è obbligatorio.");
            //RuleFor(x => x.TraStation).NotEmpty().WithMessage("TraStation è obbligatoria.");
        }
    }
}
