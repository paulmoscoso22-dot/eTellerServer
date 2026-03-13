using Microsoft.AspNetCore.Mvc;
using FluentValidation;
using eTeller.Application.Mappings.Prelievo;
using eTeller.Application.Validators;

namespace eTeller.Api.Controllers.ContiCorrenti.Prelievo;

[ApiController]
[Route("api/[controller]")]
public class PrelievoController : ControllerBase
{
    private readonly IValidator<PrelievoViewVm> _validator;

    public PrelievoController(IValidator<PrelievoViewVm> validator)
    {
        _validator = validator;
    }

    /// <summary>
    /// Valida i dati di carica (prelievo/versamento) — Fase 1
    /// </summary>
    /// <param name="request">Dati di carica da validare</param>
    /// <returns>Errori di validazione oppure dati validati pronti per Fase 2</returns>
    [HttpPost("valida-carica")]
    [ProducesResponseType(typeof(ValidationErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(CaricaValidataResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> ValidaCarica([FromBody] PrelievoViewVm request)
    {
        if (request == null)
        {
            return BadRequest(new ValidationErrorResponse
            {
                Success = false,
                Errors = new List<ValidationError>
                {
                    new ValidationError
                    {
                        Field = "Request",
                        Message = "La richiesta non può essere vuota.",
                        ErrorCode = "9000"
                    }
                }
            });
        }

        var validationResult = await _validator.ValidateAsync(request);

        if (!validationResult.IsValid)
        {
            var errorResponse = new ValidationErrorResponse
            {
                Success = false,
                Errors = validationResult.Errors
                    .GroupBy(x => x.PropertyName)
                    .Select(g => new ValidationError
                    {
                        Field = g.Key,
                        Message = g.First().ErrorMessage,
                        ErrorCode = g.First().ErrorCode ?? "9999"
                    })
                    .ToList()
            };

            return BadRequest(errorResponse);
        }

        // Validazione riuscita — i dati sono pronti per la Fase 2
        var normalizedRequest = NormalizzaRequest(request);
        var successResponse = new CaricaValidataResponse
        {
            Success = true,
            DatiValidati = normalizedRequest,
            Avvisi = new List<string>()
        };

        return Ok(successResponse);
    }

    /// <summary>
    /// Normalizza e pulisce i dati di carica dopo la validazione
    /// </summary>
    private static PrelievoViewVm NormalizzaRequest(PrelievoViewVm request)
    {
        return new PrelievoViewVm
        {
            NumeroConto = request.NumeroConto.Trim().ToUpper(),
            TipoConto = request.TipoConto.Trim().ToUpper(),
            DivisaConto = request.DivisaConto.Trim().ToUpper()[..3],
            DivisaContante = request.DivisaContante.Trim().ToUpper()[..3],
            ImportoConto = request.ImportoConto,
            ImportoContante = request.ImportoContante,
            TassoCambio = request.TassoCambio,
            TassoControvalore = request.TassoControvalore,
            ForzaCambio = request.ForzaCambio,
            Aggio = request.Aggio,
            TipoAggio = request.TipoAggio,
            SegnoAggio = request.SegnoAggio,
            DataValuta = request.DataValuta,
            StampaSaldo = request.StampaSaldo,
            StampaAvviso = request.StampaAvviso,
            NomeCognome = request.NomeCognome?.Trim(),
            CommentoInterno = request.CommentoInterno?.Trim(),
            TestoLibero = request.TestoLibero?.Trim(),
            TipoOperazione = request.TipoOperazione,
            TransazioneId = request.TransazioneId,
            ArrotondaTaglioMinimo = request.ArrotondaTaglioMinimo
        };
    }
}
