using AutoMapper;
using eTeller.Application.Mappings.CassePeriferiche;
using eTeller.Application.Features.CassePeriferiche.Queries.GetDeviceByCliId;
using eTeller.Application.Contracts;
using MediatR;

namespace eTeller.Application.Features.CassePeriferiche.Queries.GetDeviceByCliId
{
    public class GetDeviceByCliIdQueryHandler : IRequestHandler<GetDeviceByCliIdQuery, List<DeviceVm>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetDeviceByCliIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<List<DeviceVm>> Handle(GetDeviceByCliIdQuery request, CancellationToken cancellationToken)
        {
            var devices = await _unitOfWork.DeviceRepository.GetDeviceByCliIdAsync(request.CliId, cancellationToken);
            return _mapper.Map<List<DeviceVm>>(devices);
        }
    }
}