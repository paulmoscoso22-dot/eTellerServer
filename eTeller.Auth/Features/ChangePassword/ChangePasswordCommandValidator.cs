using FluentValidation;

namespace eTeller.Auth.Features.ChangePassword;

public class ChangePasswordCommandValidator : AbstractValidator<ChangePasswordCommand>
{
    public ChangePasswordCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("UserId è obbligatorio.");

        RuleFor(x => x.CurrentPassword)
            .NotEmpty().WithMessage("La password corrente è obbligatoria.");

        RuleFor(x => x.NewPassword)
            .NotEmpty().WithMessage("La nuova password è obbligatoria.")
            .MinimumLength(8).WithMessage("La password deve contenere almeno 8 caratteri.")
            .MaximumLength(64).WithMessage("La password non può superare 64 caratteri.")
            .Matches(@"[A-Z]").WithMessage("La password deve contenere almeno una lettera maiuscola.")
            .Matches(@"[a-z]").WithMessage("La password deve contenere almeno una lettera minuscola.")
            .Matches(@"[0-9]").WithMessage("La password deve contenere almeno una cifra.")
            .Matches(@"[^a-zA-Z0-9]").WithMessage("La password deve contenere almeno un carattere speciale.")
            .NotEqual(x => x.CurrentPassword).WithMessage("La nuova password deve essere diversa da quella corrente.");
    }
}
