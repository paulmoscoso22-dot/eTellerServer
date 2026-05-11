namespace eTeller.Auth.Contracts;

/// <summary>
/// Porta per la generazione di token JWT.
/// Implementazione in eTeller.Infrastructure.Auth.JwtTokenService.
/// </summary>
public interface ITokenService
{
    /// <summary>
    /// Genera un access token JWT firmato con HmacSha256.
    /// Durata: 8h (configurabile via Jwt:AccessTokenExpirationMinutes).
    /// </summary>
    string GenerateToken(UserTokenClaims claims);

    /// <summary>
    /// Restituisce la data di scadenza del token generato con i claims forniti.
    /// </summary>
    DateTime GetTokenExpiration();
}
