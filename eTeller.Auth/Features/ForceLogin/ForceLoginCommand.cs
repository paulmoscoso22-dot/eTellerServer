using eTeller.Auth.ViewModels;
using MediatR;

namespace eTeller.Auth.Features.ForceLogin;

/// <summary>
/// Comando CQRS per il force-login: disconnette la sessione esistente e crea una nuova.
/// Invocato quando LoginCommand ritorna ResultCode = "USER_ALREADY_LOGGED".
/// </summary>
public record ForceLoginCommand(
    string UserId,
    string Password,
    string? CashDeskIp
) : IRequest<LoginVm>;
