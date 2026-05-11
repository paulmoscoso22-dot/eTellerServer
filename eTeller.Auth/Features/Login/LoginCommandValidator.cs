using FluentValidation;

namespace eTeller.Auth.Features.Login;

public class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("UserId è obbligatorio.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("La password è obbligatoria.");
    }
}
