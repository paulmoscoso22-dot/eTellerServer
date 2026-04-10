using FluentValidation;

namespace eTeller.Application.Features.Manager.Commands.Users.InsertUser
{
    public class InsertUserCommandValidator : AbstractValidator<InsertUserCommand>
    {
        public InsertUserCommandValidator()
        {
            RuleFor(x => x.UsrId)
                .NotEmpty()
                .WithMessage("USR_ID is required");

            RuleFor(x => x.UsrHostId)
                .NotEmpty()
                .WithMessage("USR_HOST_ID is required");

            RuleFor(x => x.UsrBraId)
                .NotEmpty()
                .WithMessage("USR_BRA_ID is required");

            RuleFor(x => x.UsrStatus)
                .NotEmpty()
                .WithMessage("USR_STATUS is required");

            RuleFor(x => x.UsrLingua)
                .NotEmpty()
                .WithMessage("USR_LINGUA is required");
        }
    }
}