using eTeller.Application.Mappings.StCountry;
using MediatR;

namespace eTeller.Application.Features.StCountry.Queries.GetAllCountry
{
    public class GetAllCountryQuery : IRequest<IEnumerable<StCountryVm>>
    {
    }
}