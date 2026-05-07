using eTeller.Application.Mappings.GestioneErrori;
using MediatR;

namespace eTeller.Application.Features.GestioneErrori.Queries.GetForceCodes;

public class GetForceCodesQuery : IRequest<IEnumerable<ForceCodeVm>>
{
}
