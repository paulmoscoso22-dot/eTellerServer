using eTeller.Application.Mappings.Vigilanza;
using MediatR;

namespace eTeller.Application.Features.StoreProcedures.Vigilanza.Queries.GetAppearerByParametersWithExpiry
{
    public record GetAppearerByParametersWithExpiryQuery(
        string AraName,
        string? AraBirthdate,
        bool AraRecComplete,
        bool ShowExpiredRecords,
        int RecordValidityDays = 365
    ) : IRequest<IEnumerable<AppearerAllVm>>;
}
