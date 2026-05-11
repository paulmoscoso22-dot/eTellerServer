---
name: jwt-conventions
description: >
  Convenzioni JWT per il progetto eTeller: claims standard, durata token, strategia di firma,
  refresh token e configurazione. Da usare come riferimento da tutti gli agenti che generano
  o validano token JWT nel backend ASP.NET 10.
applyTo: "eTellerServer/**"
---

# JWT Conventions — eTeller Backend

## Claims Standard del Token

Ogni access token JWT emesso dal sistema eTeller **deve** contenere i seguenti claims:

| Claim | Tipo | Sorgente | Esempio |
|---|---|---|---|
| `sub` | string | `UsrId` (chiave primaria) | `"USR001"` |
| `name` | string | Nome completo utente | `"Mario Rossi"` |
| `branch_id` | string | `UsrBraId` | `"BR01"` |
| `language` | string | `UsrLingua` | `"IT"` |
| `can_use_teller` | bool | Flag cassa | `true` |
| `cash_desk_id` | string? | Id cassa (nullable) | `"CD01"` |
| `ip_address` | string | IP al momento del login | `"192.168.1.100"` |
| `session_id` | string | GUID univoco sessione | `"a1b2c3..."` |
| `jti` | string | GUID univoco del token | `"f9e8d7..."` |
| `iat` | long (Unix) | Issued At (auto) | `1715425200` |
| `exp` | long (Unix) | Expiration (auto) | `1715468400` |

### Claims Standard JWT (RFC 7519)

```csharp
// ✅ Usa ClaimTypes standard dove disponibili
new Claim(JwtRegisteredClaimNames.Sub, userId),
new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
new Claim(JwtRegisteredClaimNames.Iat,
    DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(),
    ClaimValueTypes.Integer64),

// Claims custom eTeller
new Claim("branch_id", branchId),
new Claim("language", language),
new Claim("can_use_teller", canUseTeller.ToString().ToLower(), ClaimValueTypes.Boolean),
new Claim("ip_address", ipAddress),
new Claim("session_id", sessionId),
```

---

## Durata Token

| Token | Durata | Configurazione |
|---|---|---|
| Access Token | 8 ore (orario lavorativo) | `Jwt:AccessTokenExpirationMinutes: 480` |
| Sessione inattività | 30 minuti | Gestita lato server (non nel token) |

> **Nota:** eTeller non usa Refresh Token — alla scadenza l'utente deve riloggarsi.
> La sessione di 8 ore copre l'intera giornata lavorativa bancaria.

---

## Configurazione — appsettings

```json
{
  "Jwt": {
    "Issuer": "eTellerServer",
    "Audience": "eTellerClient",
    "SecretKey": "",
    "AccessTokenExpirationMinutes": 480
  }
}
```

> ⚠️ `SecretKey` **non deve mai essere committato nel repository**.  
> In sviluppo: usare `dotnet user-secrets set "Jwt:SecretKey" "chiave-locale-dev"`.  
> In produzione: usare variabile d'ambiente o Azure KeyVault.

---

## Firma del Token

```csharp
// ✅ OBBLIGATORIO — algoritmo minimo HS256, preferire RS256 in produzione
var key = new SymmetricSecurityKey(
    Encoding.UTF8.GetBytes(_config["Jwt:SecretKey"]!));

var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

// La chiave deve essere lunga almeno 32 caratteri (256 bit)
// ❌ MAI usare chiavi corte o predicibili come "secret" o "password"
```

---

## Implementazione ITokenService

```csharp
// eTeller.Application/Contracts/ITokenService.cs
public interface ITokenService
{
    string GenerateAccessToken(UserTokenClaims claims);
    ClaimsPrincipal? ValidateToken(string token);
    string? GetClaimValue(string token, string claimType);
}

// eTeller.Application/Contracts/UserTokenClaims.cs
public record UserTokenClaims(
    string UserId,
    string UserName,
    string BranchId,
    string Language,
    bool CanUseTeller,
    string? CashDeskId,
    string IpAddress,
    string SessionId
);
```

---

## Validazione Token in ASP.NET

```csharp
// eTeller.Api — Program.cs / ServiceRegistration
services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = config["Jwt:Issuer"],
            ValidAudience = config["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(config["Jwt:SecretKey"]!)),
            ClockSkew = TimeSpan.Zero   // ← nessuna tolleranza di scadenza
        };
    });
```

---

## Regole di Sicurezza JWT

| Regola | Dettaglio |
|---|---|
| ❌ Nessun dato sensibile nei claims | Non mettere password, hash password o dati bancari nei claims |
| ✅ Validare sempre `exp` | `ValidateLifetime = true` obbligatorio |
| ✅ Validare issuer e audience | Prevenire token da altri sistemi |
| ✅ `ClockSkew = TimeSpan.Zero` | Nessuna tolleranza — token scaduto = non valido |
| ❌ No token storage in localStorage | Il frontend deve usare HttpOnly cookie o memory — non localStorage |
| ✅ Token revocation lato server | Il logout deve segnare il `session_id` come invalido nel DB |
