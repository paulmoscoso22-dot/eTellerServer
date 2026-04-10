using MediatR;
using eTeller.Application.Mappings.Language;

namespace eTeller.Application.Features.Language.Queries.GetAllLanguages
{
    public class GetAllLanguagesQuery : IRequest<IEnumerable<STLanguageVm>>
    {
    }
}
