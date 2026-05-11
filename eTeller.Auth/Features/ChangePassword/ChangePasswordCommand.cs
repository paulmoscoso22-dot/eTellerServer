using eTeller.Auth.ViewModels;
using MediatR;

namespace eTeller.Auth.Features.ChangePassword;

/// <summary>
/// Comando CQRS per il cambio password (obbligatorio o volontario).
/// </summary>
public record ChangePasswordCommand(
    string UserId,          // dall'autenticazione JWT o da login con PASSWORD_EXPIRED
    string CurrentPassword, // password attuale da verificare
    string NewPassword,     // nuova password (validata da ChangePasswordCommandValidator)
    string TraStation       // IP del client (per trace)
) : IRequest<ChangePasswordVm>;
