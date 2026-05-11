using eTeller.Auth.Features.Login;
using eTeller.Auth.Features.ForceLogin;
using eTeller.Auth.Features.Logout;
using eTeller.Auth.Features.ChangePassword;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace eTeller.Auth.Api.Controllers;

[Route("api/auth")]
[Tags("Auth")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Login utente. Restituisce JWT se le credenziali sono valide.
    /// ResultCode: OK, USER_ALREADY_LOGGED, PASSWORD_EXPIRED, MUST_CHANGE_PASSWORD,
    /// INVALID_CREDENTIALS, USER_BLOCKED, USER_DISABLED, CASH_DESK_BUSY.
    /// Nota: USER_NOT_FOUND e INVALID_CREDENTIALS restituiscono lo stesso messaggio generico (anti user-enumeration).
    /// </summary>
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Force-login: disconnette la sessione attiva e crea una nuova.
    /// Invocato quando /login ha restituito ResultCode = "USER_ALREADY_LOGGED".
    /// </summary>
    [HttpPost("force-login")]
    [AllowAnonymous]
    public async Task<IActionResult> ForceLogin([FromBody] ForceLoginCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Logout utente. Chiude la sessione in sys_USERSuseClient.
    /// SessionId e UserId vengono estratti dal token JWT.
    /// </summary>
    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        var sessionId = User.FindFirstValue("session_id") ?? string.Empty;
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
        var traStation = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "UNKNOWN";

        var command = new LogoutCommand(sessionId, userId, traStation);
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Cambio password (obbligatorio dopo scadenza o volontario).
    /// UserId viene estratto dal token JWT.
    /// </summary>
    [HttpPost("change-password")]
    [Authorize]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
        var traStation = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "UNKNOWN";

        var command = new ChangePasswordCommand(userId, request.CurrentPassword, request.NewPassword, traStation);
        var result = await _mediator.Send(command);
        return Ok(result);
    }
}

/// <summary>DTO body per /change-password (l'UserId viene dal JWT, non dal body).</summary>
public record ChangePasswordRequest(string CurrentPassword, string NewPassword);
