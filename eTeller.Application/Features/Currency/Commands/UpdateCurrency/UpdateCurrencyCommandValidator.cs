using FluentValidation;

namespace eTeller.Application.Features.Currency.Commands.UpdateCurrency
{
    public class UpdateCurrencyCommandValidator : AbstractValidator<UpdateCurrencyCommand>
    {
        public UpdateCurrencyCommandValidator()
        {
            RuleFor(x => x.CurId)
                .NotEmpty().WithMessage("CUR_ID è obbligatorio.");

            RuleFor(x => x.CurCutId)
                .NotEmpty().WithMessage("CUR_CUT_ID è obbligatorio.");

            RuleFor(x => x.CurMinamn)
                .GreaterThanOrEqualTo(0).WithMessage("Il taglio minimo deve essere >= 0.")
                .LessThanOrEqualTo(10000000).WithMessage("Il taglio minimo non può superare 10.000.000.");

            RuleFor(x => x.CurTolrat)
                .GreaterThanOrEqualTo(0).WithMessage("La tolleranza deve essere >= 0.")
                .LessThanOrEqualTo(1000000).WithMessage("La tolleranza non può superare 1.000.000.");

            RuleFor(x => x.TraUser)
                .NotEmpty().WithMessage("TraUser è obbligatorio.");

            RuleFor(x => x.TraStation)
                .NotEmpty().WithMessage("TraStation è obbligatorio.");
        }
    }
}
