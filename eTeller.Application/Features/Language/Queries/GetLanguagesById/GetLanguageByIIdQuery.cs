using MediatR;
using eTeller.Application.Mappings.Language;

namespace eTeller.Application.Features.Language.Queries.GetLanguagesById
{
    public class GetLanguageByIIdQuery : IRequest<IEnumerable<STLanguageVm>>
    {
        public string LanId { get; set; }

        public GetLanguageByIIdQuery(string lanId)
        {
            LanId = lanId;
        }
    }
}
