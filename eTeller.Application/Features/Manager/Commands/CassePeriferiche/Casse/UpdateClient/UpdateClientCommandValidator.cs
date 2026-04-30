using FluentValidation;

namespace eTeller.Application.Features.Manager.Commands.CassePeriferiche.Casse.UpdateClient
{
    public class UpdateClientCommandValidator : AbstractValidator<UpdateClientCommand>
    {
        public UpdateClientCommandValidator()
        {
            RuleFor(x => x.CliId)
                .NotEmpty().WithMessage("CLI_ID è obbligatorio.");

            RuleFor(x => x.CliIp)
                .NotEmpty().WithMessage("CLI_IP è obbligatorio.")
                .Matches(@"^((25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$")
                .WithMessage("CLI_IP non è un indirizzo IPv4 valido.");

            RuleFor(x => x.CliMac)
                .NotEmpty().WithMessage("CLI_MAC è obbligatorio.")
                .Matches(@"^[0-9A-F]{2}-[0-9A-F]{2}-[0-9A-F]{2}-[0-9A-F]{2}-[0-9A-F]{2}-[0-9A-F]{2}$")
                .WithMessage("CLI_MAC non è un indirizzo MAC valido (formato: XX-XX-XX-XX-XX-XX).");

            RuleFor(x => x.CliBraId)
                .NotEmpty().WithMessage("CLI_BRA_ID è obbligatorio.");

            RuleFor(x => x.CliStatus)
                .NotEmpty().WithMessage("CLI_STATUS è obbligatorio.");

            RuleFor(x => x.TraUser)
                .NotEmpty().WithMessage("TraUser è obbligatorio.");

            RuleFor(x => x.TraStation)
                .NotEmpty().WithMessage("TraStation è obbligatorio.");
        }
    }
}
