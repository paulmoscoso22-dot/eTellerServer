using AutoMapper;
using eTeller.Application.Contracts;
using eTeller.Application.Features.StoreProcedures.AntirecAppearer.Mapping;
using MediatR;

namespace eTeller.Application.Features.StoreProcedures.AntirecAppearer.Queries.GetAntirecAppearerByAraId
{
    public class GetAntirecAppearerByAraIdQueryHandle : IRequestHandler<GetAntirecAppearerByAraIdQuery, List<AntirecAppearerViewVm>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetAntirecAppearerByAraIdQueryHandle(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<List<AntirecAppearerViewVm>> Handle(GetAntirecAppearerByAraIdQuery request, CancellationToken cancellationToken)
        {
            var antirecAppearers = await _unitOfWork.AntirecAppearerSelectRepository.GetAntirecAppearerByAreaIdAsync(request.AraId);

            return _mapper.Map<List<AntirecAppearerViewVm>>(antirecAppearers);

        }
    }
}
