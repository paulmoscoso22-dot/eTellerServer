using AutoMapper;
using eTeller.Application.Contracts;
using eTeller.Application.Mappings.Tabella;
using MediatR;
using Microsoft.Extensions.Logging;

namespace eTeller.Application.Features.Tabella.Queries.GetTabellaServVarcharById
{
    public class GetTabellaServVarcharByIdQueryHandler : IRequestHandler<GetTabellaServVarcharByIdQuery, TabellaServVarcharVm>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<GetTabellaServVarcharByIdQueryHandler> _logger;

        public GetTabellaServVarcharByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, ILogger<GetTabellaServVarcharByIdQueryHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<TabellaServVarcharVm> Handle(GetTabellaServVarcharByIdQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling {QueryName} with Id: {Id}, NomeTabella: {NomeTabella}",
                nameof(GetTabellaServVarcharByIdQuery), request.Id, request.NomeTabella);

            var result = await _unitOfWork.TabellaRepository.GetTabellaServVarcharById(request.Id, request.NomeTabella);

            var vm = _mapper.Map<TabellaServVarcharVm>(result);
            _logger.LogInformation("Handled {QueryName}, found: {Found}", nameof(GetTabellaServVarcharByIdQuery), result != null);

            return vm;
        }
    }
}