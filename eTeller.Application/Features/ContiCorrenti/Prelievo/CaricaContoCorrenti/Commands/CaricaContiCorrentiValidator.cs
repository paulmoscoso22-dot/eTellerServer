using eTeller.Application.Contracts;
using eTeller.Domain.Services;
using FluentValidation;
using IErrorCodeRepository = eTeller.Application.Contracts.StoreProcedures.Operazioni.ContoCorrenti.Prelievo.IErrorCodeRepository;

namespace eTeller.Application.Features.ContiCorrenti.Commands.Carica;

/// <summary>
/// Validatore FluentValidation per il comando CaricaContiCorrenti.
/// Implementa tutte le regole di validazione della Fase 2 del bottone "Carica",
/// incluse validazioni sincrone sui campi e validazioni asincrone contro il dominio
/// (taglio minimo, tolleranza cambio, IS107).
/// </summary>
public sealed class CaricaContiCorrentiValidator : AbstractValidator<CaricaContiCorrentiCommand>
{
    private const string CodiceErrContoObbligatorio       = "1305";
    private const string CodiceErrDivisaContoObbligatoria = "2014";
    private const string CodiceErrDivisaBBObbligatoria    = "2016";
    private const string CodiceErrImportoObbligatorio     = "2015";
    private const string CodiceErrImportoNonValido        = "1325";
    private const string CodiceErrCambioNonValido         = "1326";
    private const string CodiceErrCambioCtvNonValido      = "1327";
    private const string CodiceErrAggioNonValido          = "1328";
    private const string CodiceErrAggioSoloDiviseUguali   = "2020";
    private const string CodiceErrDataNonValida           = "2021";
    private const string CodiceErrTaglioMinimo            = "2013";
    private const string CodiceErrForzaCambioCommentoReq  = "9036";

    private readonly IForexDomainService _forexDomainService;
    private readonly IErrorCodeRepository _errorCodeRepository;

    public CaricaContiCorrentiValidator(
        IForexDomainService forexDomainService,
        IErrorCodeRepository errorCodeRepository)
    {
        _forexDomainService  = forexDomainService;
        _errorCodeRepository = errorCodeRepository;

        ApplicaRegolaContoObbligatorio();
        ApplicaRegoleDivise();
        ApplicaRegolaImportoObbligatorio();
        ApplicaRegolaImportiNonValidi();
        ApplicaRegolaCambioNonValido();
        ApplicaRegolaCambioCtvNonValido();
        ApplicaRegolaAggioNonValido();
        ApplicaRegolaAggioSoloDiviseUguali();
        ApplicaRegolaDateValide();
        ApplicaRegolaTaglioMinimo();
        ApplicaRegolaForzaCambioRichiedeCommento();
    }

    // ── Conto obbligatorio ────────────────────────────────────────────────────

    private void ApplicaRegolaContoObbligatorio()
    {
        RuleFor(x => x.NumeroConto)
            .NotEmpty()
            .WithErrorCode(CodiceErrContoObbligatorio)
            .WithMessage(_ => GetErrorMessage(CodiceErrContoObbligatorio));
    }

    // ── Divise obbligatorie ───────────────────────────────────────────────────

    private void ApplicaRegoleDivise()
    {
        RuleFor(x => x.DivisaConto)
            .NotEmpty()
            .WithErrorCode(CodiceErrDivisaContoObbligatoria)
            .WithMessage(_ => GetErrorMessage(CodiceErrDivisaContoObbligatoria));

        RuleFor(x => x.DivisaBanconote)
            .NotEmpty()
            .WithErrorCode(CodiceErrDivisaBBObbligatoria)
            .WithMessage(_ => GetErrorMessage(CodiceErrDivisaBBObbligatoria));
    }

    // ── Almeno un importo obbligatorio ────────────────────────────────────────

    private void ApplicaRegolaImportoObbligatorio()
    {
        RuleFor(x => x)
            .Must(x => x.ImportoConto.HasValue || x.ImportoBanconote.HasValue)
            .WithErrorCode(CodiceErrImportoObbligatorio)
            .WithMessage(_ => GetErrorMessage(CodiceErrImportoObbligatorio))
            .OverridePropertyName(nameof(CaricaContiCorrentiCommand.ImportoBanconote));
    }

    // ── Importi devono essere valori positivi ─────────────────────────────────

    private void ApplicaRegolaImportiNonValidi()
    {
        RuleFor(x => x.ImportoConto)
            .Must(v => !v.HasValue || v > 0)
            .WithErrorCode(CodiceErrImportoNonValido)
            .WithMessage(_ => GetErrorMessage(CodiceErrImportoNonValido))
            .When(x => x.ImportoConto.HasValue);

        RuleFor(x => x.ImportoBanconote)
            .Must(v => !v.HasValue || v > 0)
            .WithErrorCode(CodiceErrImportoNonValido)
            .WithMessage(_ => GetErrorMessage(CodiceErrImportoNonValido))
            .When(x => x.ImportoBanconote.HasValue);
    }

    // ── Tasso di cambio: positivo e con max 6 decimali ────────────────────────

    private void ApplicaRegolaCambioNonValido()
    {
        RuleFor(x => x.TassoCambio)
            .Must(BeValidExchangeRate)
            .WithErrorCode(CodiceErrCambioNonValido)
            .WithMessage(_ => GetErrorMessage(CodiceErrCambioNonValido))
            .When(x => x.TassoCambio.HasValue);
    }

    // ── Cambio CTV: positivo e con max 6 decimali ────────────────────────────

    private void ApplicaRegolaCambioCtvNonValido()
    {
        RuleFor(x => x.TassoCambioControvalore)
            .Must(BeValidExchangeRate)
            .WithErrorCode(CodiceErrCambioCtvNonValido)
            .WithMessage(_ => GetErrorMessage(CodiceErrCambioCtvNonValido))
            .When(x => x.TassoCambioControvalore.HasValue);
    }

    // ── Aggio: positivo, max 6 decimali ──────────────────────────────────────

    private void ApplicaRegolaAggioNonValido()
    {
        RuleFor(x => x.Aggio)
            .Must(v => !v.HasValue || v > 0)
            .WithErrorCode(CodiceErrAggioNonValido)
            .WithMessage(_ => GetErrorMessage(CodiceErrAggioNonValido))
            .When(x => x.Aggio.HasValue);
    }

    // ── Aggio applicabile solo se divise uguali ───────────────────────────────

    private void ApplicaRegolaAggioSoloDiviseUguali()
    {
        RuleFor(x => x)
            .Must(x => NormalizzaDivisa(x.DivisaBanconote) == NormalizzaDivisa(x.DivisaConto))
            .WithErrorCode(CodiceErrAggioSoloDiviseUguali)
            .WithMessage(_ => GetErrorMessage(CodiceErrAggioSoloDiviseUguali))
            .When(x => x.Aggio.HasValue && !string.IsNullOrEmpty(x.DivisaBanconote) && !string.IsNullOrEmpty(x.DivisaConto))
            .OverridePropertyName(nameof(CaricaContiCorrentiCommand.Aggio));
    }

    // ── Date devono essere parsabili e valide ─────────────────────────────────

    private void ApplicaRegolaDateValide()
    {
        RuleFor(x => x.DataValuta)
            .Must(d => d != default)
            .WithErrorCode(CodiceErrDataNonValida)
            .WithMessage(_ => GetErrorMessage(CodiceErrDataNonValida));

        RuleFor(x => x.DataOperazione)
            .Must(d => d != default)
            .WithErrorCode(CodiceErrDataNonValida)
            .WithMessage(_ => GetErrorMessage(CodiceErrDataNonValida));
    }

    // ── Taglio minimo banconote (validazione asincrona su dominio) ────────────

    private void ApplicaRegolaTaglioMinimo()
    {
        RuleFor(x => x)
            .MustAsync(async (cmd, ct) =>
            {
                if (!cmd.ImportoBanconote.HasValue || !cmd.ArrotondaTaglioMinimo)
                    return true;

                return await _forexDomainService.RispettaTaglioMinimoAsync(
                    cmd.DivisaBanconote,
                    cmd.ImportoBanconote.Value,
                    ct);
            })
            .WithErrorCode(CodiceErrTaglioMinimo)
            .WithMessage(cmd => BuildMessaggioTaglioMinimo(cmd))
            .When(x => x.ImportoBanconote.HasValue && !string.IsNullOrEmpty(x.DivisaBanconote))
            .OverridePropertyName(nameof(CaricaContiCorrentiCommand.ImportoBanconote));
    }

    // ── Forza cambio richiede commento + nome/cognome ─────────────────────────

    private void ApplicaRegolaForzaCambioRichiedeCommento()
    {
        When(x => x.ForzaCambio, () =>
        {
            RuleFor(x => x.CommentoInterno)
                .NotEmpty()
                .WithErrorCode(CodiceErrForzaCambioCommentoReq)
                .WithMessage(_ => GetErrorMessage(CodiceErrForzaCambioCommentoReq));

            RuleFor(x => x.NomeCognome)
                .NotEmpty()
                .WithErrorCode(CodiceErrForzaCambioCommentoReq)
                .WithMessage(_ => GetErrorMessage(CodiceErrForzaCambioCommentoReq));
        });
    }

    // ── Helpers privati ───────────────────────────────────────────────────────

    /// <summary>
    /// Un tasso di cambio valido è: positivo, non zero, con al massimo 6 decimali.
    /// </summary>
    private static bool BeValidExchangeRate(decimal? value)
    {
        if (!value.HasValue) return true;
        if (value <= 0) return false;

        var decimals = BitConverter.GetBytes(decimal.GetBits(value.Value)[3])[2];
        return decimals <= 6;
    }

    /// <summary>
    /// Normalizza la divisa prendendo solo i primi 3 caratteri (ISO 4217).
    /// Il DB può restituire "EUR " con trailing space, quindi serve trim.
    /// </summary>
    private static string NormalizzaDivisa(string divisa) =>
        divisa.Trim()[..Math.Min(3, divisa.Trim().Length)].ToUpperInvariant();

    /// <summary>Recupera la descrizione dell'errore dal repository in cultura base.</summary>
    private string GetErrorMessage(string errorCode) =>
        _errorCodeRepository.GetDescription(errorCode);

    /// <summary>Costruisce il messaggio di errore per taglio minimo con i dettagli della valuta.</summary>
    private string BuildMessaggioTaglioMinimo(CaricaContiCorrentiCommand cmd)
    {
        var taglioMinimo = _forexDomainService
            .GetTaglioMinimoAsync(cmd.DivisaBanconote, CancellationToken.None)
            .GetAwaiter()
            .GetResult();

        return string.Format(
            GetErrorMessage(CodiceErrTaglioMinimo),
            cmd.ImportoBanconote,
            cmd.DivisaBanconote,
            taglioMinimo);
    }
}
