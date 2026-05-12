using eTeller.Application.Mappings.HelpInfoViewModels;
using MediatR;

namespace eTeller.Application.Features.Help.Queries.GetHelpInfo
{
    public record GetHelpInfoQuery(string CliId, bool CanUseTellerEnable) : IRequest<HelpInfoVm>;
}
