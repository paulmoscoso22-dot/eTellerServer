using AutoMapper;
using eTeller.Application.Contracts;
using eTeller.Application.Mappings.User;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using eTeller.Domain.Models;

namespace eTeller.Application.Features.Manager.Queries.Users.GetSysUsersUseClient
{
    public class GetSysUsersUseClientQueryHandler : IRequestHandler<GetSysUsersUseClientQuery, IEnumerable<SysUsersUseClientVm>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<GetSysUsersUseClientQueryHandler> _logger;

        public GetSysUsersUseClientQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, ILogger<GetSysUsersUseClientQueryHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<SysUsersUseClientVm>> Handle(GetSysUsersUseClientQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling {QueryName}", nameof(GetSysUsersUseClientQuery));

            var repo = _unitOfWork.Repository<SysUsersUseClient>();
            var all = await repo.GetAllAsync();
            var filtered = all.Where(x => x.DataOut == null).OrderByDescending(x => x.UsrCliId).ToList();

            var vms = _mapper.Map<IEnumerable<SysUsersUseClientVm>>(filtered);
            _logger.LogInformation("Returned {Count} SysUsersUseClient records", vms?.Count() ?? 0);
            return vms;
        }
    }
}
