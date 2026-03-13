using eTeller.Domain.Models;

namespace eTeller.Application.Features.ContiCorrenti.Commands.Carica;

/// <summary>
/// Risultato del comando Carica per l'operazione su Conti Correnti.
/// Contiene la transazione salvata, eventuali warning e il flag benefondo.
/// </summary>
public sealed record CaricaContiCorrentiResult
{
    public Transaction Transaction { get; init; }
    public IReadOnlyList<string> Warnings { get; init; } = [];
    public bool BenefondoRichiesto { get; init; }

    private CaricaContiCorrentiResult() { }

    public static CaricaContiCorrentiResult Success(
        Transaction transaction,
        IEnumerable<string>? warnings = null,
        bool benefondoRichiesto = false) =>
        new()
        {
            Transaction = transaction,
            Warnings = warnings?.ToList().AsReadOnly() ?? (IReadOnlyList<string>)Array.Empty<string>(),
            BenefondoRichiesto = benefondoRichiesto
        };
}
