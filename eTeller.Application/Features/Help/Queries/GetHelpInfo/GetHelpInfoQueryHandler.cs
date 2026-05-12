using eTeller.Application.Contracts;
using eTeller.Application.Contracts.Help;
using eTeller.Application.Mappings.HelpInfoViewModels;
using eTeller.Domain.Models;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Linq;

namespace eTeller.Application.Features.Help.Queries.GetHelpInfo
{
    public class GetHelpInfoQueryHandler : IRequestHandler<GetHelpInfoQuery, HelpInfoVm>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHelpInfoRepository _helpInfoRepository;
        private readonly ILogger<GetHelpInfoQueryHandler> _logger;

        public GetHelpInfoQueryHandler(
            IUnitOfWork unitOfWork,
            IHelpInfoRepository helpInfoRepository,
            ILogger<GetHelpInfoQueryHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _helpInfoRepository = helpInfoRepository;
            _logger = logger;
        }

        public async Task<HelpInfoVm> Handle(GetHelpInfoQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling {QueryName} for CliId={CliId}", nameof(GetHelpInfoQuery), request.CliId);

            // Ultimo utente uscito dalla cassa (singola tabella)
            var lastUsersList = await _unitOfWork.Repository<SysUsersUseClient>().GetAsync(
                u => u.CliId == request.CliId && u.Logout);
            var lastUser = lastUsersList.OrderByDescending(u => u.DataOut).FirstOrDefault();

            // Lingua di sistema: CLI_LINGUA da sys_CLIENT → descrizione da ST_LANGUAGE (due tabelle separate)
            var clients = await _unitOfWork.Repository<Client>().GetAsync(c => c.CliId == request.CliId);
            var client = clients.FirstOrDefault();
            var systemLanguage = string.Empty;
            if (client?.CliLingua is not null)
            {
                var langs = await _unitOfWork.Repository<ST_LANGUAGE>().GetAsync(l => l.LanId == client.CliLingua);
                systemLanguage = langs.FirstOrDefault()?.LanDes ?? client.CliLingua;
            }

            // Stampante fiches e TwinSafe (JOIN sys_CLIENT_DEVICE + sys_DEVICE)
            var fichesPrinter = await _helpInfoRepository.GetFichesPrinterAsync(request.CliId);
            var isTwinSafeEnabled = await _helpInfoRepository.ExistsTwinSafeAsync(request.CliId);

            // Versione applicativo web da PERSONALISATION
            var pars = await _unitOfWork.Repository<Personalisation>().GetAsync(p => p.ParId == "WebApplicationVersion");
            var webApplicationVersion = pars.FirstOrDefault()?.ParValue ?? "-";

            _logger.LogInformation("Handled {QueryName} for CliId={CliId}", nameof(GetHelpInfoQuery), request.CliId);

            return new HelpInfoVm
            {
                LastUser = lastUser?.UsrId,
                LastLogoutDate = lastUser?.DataOut,
                SystemLanguage = systemLanguage,
                FichesPrinter = fichesPrinter,
                IsTwinSafeEnabled = isTwinSafeEnabled,
                IsCassaOperationsEnabled = request.CanUseTellerEnable,
                WebApplicationVersion = webApplicationVersion,
                HostUser = null,        // Servizio host legacy non ancora migrato
                HostVersion = null,     // Servizio host legacy non ancora migrato
                HostCompileDate = null  // Servizio host legacy non ancora migrato
            };
        }
    }
}
