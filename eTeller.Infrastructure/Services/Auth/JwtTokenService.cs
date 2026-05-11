using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using eTeller.Application.Contracts.Auth;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace eTeller.Infrastructure.Services.Auth;

/// <summary>
/// Implementazione JWT di ITokenService.
/// Genera token firmati HS256, legge configurazione dalla sezione "Jwt" dell'appsettings.
/// </summary>
public class JwtTokenService : ITokenService
{
    private readonly IConfiguration _configuration;

    public JwtTokenService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string GenerateToken(UserTokenClaims claims)
    {
        var secretKey = _configuration["Jwt:SecretKey"];

        if (string.IsNullOrWhiteSpace(secretKey))
            throw new InvalidOperationException("Jwt:SecretKey non configurata o vuota. Usare dotnet user-secrets in sviluppo e una variabile d'ambiente sicura in produzione.");

        if (secretKey.Length < 32)
            throw new InvalidOperationException("Jwt:SecretKey deve essere di almeno 32 caratteri (256 bit) per garantire la sicurezza della firma HS256.");

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expiration = GetTokenExpiration();

        var jwtClaims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, claims.UserId),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Iat,
                DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(),
                ClaimValueTypes.Integer64),
            new Claim("name", claims.UserName),
            new Claim("branch_id", claims.BranchId),
            new Claim("language", claims.Language),
            new Claim("can_use_teller", claims.CanUseTeller.ToString().ToLower(), ClaimValueTypes.Boolean),
            new Claim("ip_address", claims.IpAddress),
            new Claim("session_id", claims.SessionId),
        };

        if (!string.IsNullOrEmpty(claims.CashDeskId))
            jwtClaims.Add(new Claim("cash_desk_id", claims.CashDeskId));

        if (!string.IsNullOrEmpty(claims.CashDeskBranchId))
            jwtClaims.Add(new Claim("cash_desk_branch_id", claims.CashDeskBranchId));

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: jwtClaims,
            expires: expiration,
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public DateTime GetTokenExpiration()
    {
        var raw = _configuration["Jwt:AccessTokenExpirationMinutes"];
        var minutes = int.TryParse(raw, out var parsed) ? parsed : 480;
        return DateTime.UtcNow.AddMinutes(minutes);
    }
}
