using FluentValidation;
using eTeller.Application.Mappings.Prelievo;

namespace eTeller.Application.Validators;

public class CaricaRequestValidator : AbstractValidator<PrelievoViewVm>
{
    public CaricaRequestValidator()
    {
        // -------------------------------------------------------
        // CONTO
        // -------------------------------------------------------

        RuleFor(x => x.NumeroConto)
            .NotEmpty()
            .WithErrorCode("1305")
            .WithMessage("Il numero conto è obbligatorio.");

        RuleFor(x => x.TipoConto)
            .NotEmpty()
            .WithMessage("Il tipo conto è obbligatorio.");

        RuleFor(x => x.DivisaConto)
            .NotEmpty()
            .WithErrorCode("2014")
            .WithMessage("La divisa del conto è obbligatoria.");

        // -------------------------------------------------------
        // DIVISA CONTANTE
        // -------------------------------------------------------

        RuleFor(x => x.DivisaContante)
            .NotEmpty()
            .WithErrorCode("2016")
            .WithMessage("La divisa delle banconote è obbligatoria.");

        // -------------------------------------------------------
        // IMPORTI — almeno uno dei due deve essere presente
        // -------------------------------------------------------

        RuleFor(x => x)
            .Must(x => x.ImportoConto.HasValue || x.ImportoContante.HasValue)
            .WithErrorCode("2015")
            .WithMessage("Inserire almeno un importo (conto o contante).")
            .OverridePropertyName("Importo");

        RuleFor(x => x.ImportoConto)
            .GreaterThan(0)
            .WithErrorCode("1325")
            .WithMessage("L'importo conto deve essere maggiore di zero.")
            .When(x => x.ImportoConto.HasValue);

        RuleFor(x => x.ImportoContante)
            .GreaterThan(0)
            .WithErrorCode("1325")
            .WithMessage("L'importo contante deve essere maggiore di zero.")
            .When(x => x.ImportoContante.HasValue);

        // -------------------------------------------------------
        // CAMBIO
        // -------------------------------------------------------

        RuleFor(x => x.TassoCambio)
            .GreaterThan(0)
            .WithErrorCode("1326")
            .WithMessage("Il tasso di cambio deve essere maggiore di zero.")
            .When(x => x.TassoCambio.HasValue);

        RuleFor(x => x.TassoControvalore)
            .GreaterThan(0)
            .WithErrorCode("1327")
            .WithMessage("Il tasso di controvalore deve essere maggiore di zero.")
            .When(x => x.TassoControvalore.HasValue);

        // -------------------------------------------------------
        // AGGIO — se presente, le divise devono essere uguali
        // -------------------------------------------------------

        RuleFor(x => x.Aggio)
            .GreaterThanOrEqualTo(0)
            .WithErrorCode("1328")
            .WithMessage("L'aggio non può essere negativo.")
            .When(x => x.Aggio.HasValue);

        RuleFor(x => x)
            .Must(x => x.DivisaConto.Substring(0, 3) == x.DivisaContante.Substring(0, 3))
            .WithErrorCode("2020")
            .WithMessage("L'aggio può essere applicato solo se le divise del conto e del contante sono uguali.")
            .When(x => x.Aggio.HasValue
                        && x.Aggio > 0
                        && !string.IsNullOrEmpty(x.DivisaConto)
                        && !string.IsNullOrEmpty(x.DivisaContante))
            .OverridePropertyName("Aggio");

        // -------------------------------------------------------
        // DATA VALUTA
        // -------------------------------------------------------

        RuleFor(x => x.DataValuta)
            .NotEmpty()
            .WithErrorCode("2021")
            .WithMessage("La data valuta è obbligatoria.");

        RuleFor(x => x.DataValuta)
            .Must(d => d != DateOnly.MinValue)
            .WithErrorCode("2021")
            .WithMessage("La data valuta non è valida.");

        // -------------------------------------------------------
        // CAMBIO FORZATO — richiede commento e nome/cognome
        // -------------------------------------------------------

        RuleFor(x => x.CommentoInterno)
            .NotEmpty()
            .WithErrorCode("9036")
            .WithMessage("Il commento interno è obbligatorio quando si forza il cambio.")
            .When(x => x.ForzaCambio);

        RuleFor(x => x.NomeCognome)
            .NotEmpty()
            .WithErrorCode("9036")
            .WithMessage("Il nome e cognome è obbligatorio quando si forza il cambio.")
            .When(x => x.ForzaCambio);
    }
}
