using FluentValidation;

namespace eTeller.Application.Features.Manager.Commands.Tabelle.InsertServizio
{
    public class InsertServizioCommandValidator : AbstractValidator<InsertServizioCommand>
    {
        public InsertServizioCommandValidator()
        {
            RuleFor(x => x.SerId)
                .NotEmpty().WithMessage("Codice servizio è obbligatorio.")
                .MaximumLength(50).WithMessage("Codice servizio non può superare 50 caratteri.");

            RuleFor(x => x.SerDes)
                .NotEmpty().WithMessage("Descrizione è obbligatoria.")
                .MaximumLength(200).WithMessage("Descrizione non può superare 200 caratteri.");

            RuleFor(x => x.TraUser)
                .NotEmpty().WithMessage("TraUser è obbligatorio.");

            RuleFor(x => x.TraStation)
                .NotEmpty().WithMessage("TraStation è obbligatoria.");
        }
    }
}
