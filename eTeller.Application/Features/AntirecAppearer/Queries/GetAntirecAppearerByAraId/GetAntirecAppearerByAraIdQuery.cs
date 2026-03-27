using eTeller.Application.Features.StoreProcedures.AntirecAppearer.Mapping;
using MediatR;

namespace eTeller.Application.Features.StoreProcedures.AntirecAppearer.Queries.GetAntirecAppearerByAraId
{
    public class GetAntirecAppearerByAraIdQuery : IRequest<List<AntirecAppearerViewVm>>
    {
        public int AraId { get; set; }

        public GetAntirecAppearerByAraIdQuery(int araId)
        {
            AraId = araId;
        }
    }
}
