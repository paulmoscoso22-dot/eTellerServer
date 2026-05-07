using FluentValidation;

namespace eTeller.Application.Features.Tabella.Commands.InsertOperationType
{
    public class InsertOperationTypeCommandValidator : AbstractValidator<InsertOperationTypeCommand>
    {
        public InsertOperationTypeCommandValidator()
        {
            RuleFor(x => x.OptId)
                .NotEmpty().WithMessage("Il codice tipo operazione è obbligatorio.")
                .MaximumLength(5).WithMessage("Il codice non può superare 5 caratteri.");

            RuleFor(x => x.OptDes)
                .NotEmpty().WithMessage("La descrizione è obbligatoria.")
                .MaximumLength(50).WithMessage("La descrizione non può superare 50 caratteri.");

            RuleFor(x => x.OptHoscod)
                .NotEmpty().WithMessage("Il codice host è obbligatorio.")
                .MaximumLength(25).WithMessage("Il codice host non può superare 25 caratteri.");

            RuleFor(x => x.OptIscredit)
                .NotEmpty().WithMessage("Il segno importo è obbligatorio.")
                .Must(x => x == "1" || x == "-1").WithMessage("Il segno importo deve essere '1' o '-1'.");

            RuleFor(x => x.OptAptId)
                .NotEmpty().WithMessage("L'applicazione è obbligatoria.")
                .MaximumLength(5).WithMessage("L'applicazione non può superare 5 caratteri.");

            RuleFor(x => x.OptAdvId)
                .MaximumLength(50).WithMessage("Il modello fiche non può superare 50 caratteri.")
                .When(x => x.OptAdvId != null);

            RuleFor(x => x.TraUser)
                .NotEmpty().WithMessage("L'utente è obbligatorio.");

            RuleFor(x => x.TraStation)
                .NotEmpty().WithMessage("La stazione è obbligatoria.");
        }
    }
}
