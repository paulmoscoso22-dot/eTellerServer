using AutoMapper;
using eTeller.Application.Contracts;
using eTeller.Application.Mappings.User;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Linq;
 
using System.Threading;
using System.Threading.Tasks;
using eTeller.Domain.Models;
using System.Collections.Generic;

namespace eTeller.Application.Features.User.Queries.GetUserUseClient
{
    public class GetUserUseClientQueryHandler : IRequestHandler<GetUserUseClientQuery, IEnumerable<SysUsersUseClientVm>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<GetUserUseClientQueryHandler> _logger;

        public GetUserUseClientQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, ILogger<GetUserUseClientQueryHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<SysUsersUseClientVm>> Handle(GetUserUseClientQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling {QueryName}", nameof(GetUserUseClientQuery));

            // Retrieve all and apply in-memory filter to avoid EF Core references here
            var data = await _unitOfWork.Repository<SysUsersUseClient>().GetAllAsync();
            var filtered = data.Where(x => x.DataOut == null).OrderByDescending(x => x.UsrCliId).ToList();
            var vms = _mapper.Map<IEnumerable<SysUsersUseClientVm>>(filtered);
            _logger.LogInformation("Handled {QueryName}, returned {Count} items", nameof(GetUserUseClientQuery), vms?.AsEnumerable().Count() ?? 0);
            return vms;
        }
    }
}
