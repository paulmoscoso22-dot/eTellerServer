using eTeller.Application.Mappings.ForceTrx;
using MediatR;

namespace eTeller.Application.Features.Manager.Queries.Tabelle.GetForceTrxById
{
    public record GetForceTrxByIdQuery(string LanCode, int TrfId) : IRequest<IEnumerable<ForceTrxVm>>;
}
