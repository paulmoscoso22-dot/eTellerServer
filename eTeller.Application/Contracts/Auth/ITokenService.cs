namespace eTeller.Application.Contracts.Auth;

/// <summary>
/// Porta per la generazione di token JWT.
/// Implementazione in eTeller.Infrastructure.Services.Auth.JwtTokenService.
/// Definita in Application per consentire a Infrastructure di implementarla
/// senza dipendenza circolare con eTeller.Auth.
/// </summary>
public interface ITokenService
{
    /// <summary>
    /// Genera un access token JWT firmato con HmacSha256.
    /// Durata configurabile via Jwt:AccessTokenExpirationMinutes (default 480 min = 8h).
    /// </summary>
    string GenerateToken(UserTokenClaims claims);

    /// <summary>
    /// Calcola la data di scadenza del prossimo token generato.
    /// </summary>
    DateTime GetTokenExpiration();
}
