using eTeller.Auth.ViewModels;
using MediatR;

namespace eTeller.Auth.Features.Login;

/// <summary>
/// Comando CQRS per il login utente.
/// Corrisponde alla logica di etellerAuthmanager.loginUser() del sistema legacy.
/// </summary>
public record LoginCommand(
    string UserId,
    string Password,
    string? CashDeskIp
) : IRequest<LoginVm>;
