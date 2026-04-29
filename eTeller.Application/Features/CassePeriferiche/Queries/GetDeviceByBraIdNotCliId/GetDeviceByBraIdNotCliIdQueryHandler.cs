using AutoMapper;
using eTeller.Application.Mappings.CassePeriferiche;
using eTeller.Application.Features.CassePeriferiche.Queries.GetDeviceByBraIdNotCliId;
using eTeller.Application.Contracts;
using MediatR;

namespace eTeller.Application.Features.CassePeriferiche.Queries.GetDeviceByBraIdNotCliId
{
    public class GetDeviceByBraIdNotCliIdQueryHandler : IRequestHandler<GetDeviceByBraIdNotCliIdQuery, List<DeviceVm>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetDeviceByBraIdNotCliIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<List<DeviceVm>> Handle(GetDeviceByBraIdNotCliIdQuery request, CancellationToken cancellationToken)
        {
            var devices = await _unitOfWork.DeviceRepository.GetDevicesByBraIdNotCliIdAsync(request.CliId, request.BraId, cancellationToken);
            return _mapper.Map<List<DeviceVm>>(devices);
        }
    }
}
