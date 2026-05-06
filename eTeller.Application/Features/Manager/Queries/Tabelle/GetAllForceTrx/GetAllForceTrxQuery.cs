using eTeller.Application.Mappings.ForceTrx;
using MediatR;

namespace eTeller.Application.Features.Manager.Queries.Tabelle.GetAllForceTrx
{
    public record GetAllForceTrxQuery(string LanCode = "IT") : IRequest<IEnumerable<ForceTrxVm>>;
}
