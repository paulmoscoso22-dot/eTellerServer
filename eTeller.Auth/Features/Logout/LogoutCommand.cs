using eTeller.Auth.ViewModels;
using MediatR;

namespace eTeller.Auth.Features.Logout;

/// <summary>
/// Comando CQRS per il logout utente.
/// Il SessionId e l'UserId vengono estratti dai claims JWT nel controller.
/// </summary>
public record LogoutCommand(
    string SessionId,   // claim session_id dal JWT
    string UserId,      // claim sub dal JWT
    string TraStation   // IP del client (per trace)
) : IRequest<LogoutVm>;
