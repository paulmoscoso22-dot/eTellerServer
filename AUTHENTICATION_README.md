# eTeller Authentication System - DDD Implementation

## Panoramica
Questo documento descrive l'implementazione del sistema di autenticazione per eTeller seguendo i principi di Domain-Driven Design (DDD) e Clean Code.

## Architettura

### 1. Domain Layer (`eTeller.Domain`)
Contiene la logica di business core e le regole del dominio.

#### Models
- **User.cs**: Entità User mappata sulla tabella `sys_USERS`

#### Constants
- **AuthenticationConstants.cs**: Costanti per stati utente, risultati autenticazione e policy password
  - `UserStatusConstants`: ENABLED, DISABLED, BLOCKED, PENDING_ACTIVATION
  - `AuthenticationResultConstants`: SUCCESS, USER_LOGGED, CLI_OKKUPATO, etc.
  - `PasswordPolicyConstants`: Regole per lunghezza minima, cifre minime, massimi tentativi

#### Exceptions
- **AuthenticationExceptions.cs**: Eccezioni custom per il dominio autenticazione
  - `PasswordNotValidException`: Password già utilizzata
  - `PasswordTooShortException`: Password troppo corta
  - `PasswordMinDigitException`: Password con poche cifre
  - `UserBlockedException`: Utente bloccato
  - `UserNotFoundException`: Utente non trovato
  - `InvalidCredentialsException`: Credenziali non valide

### 2. Application Layer (`eTeller.Application`)
Contiene la logica applicativa e coordina il flusso tra presentazione e dominio.

#### DTOs
`Features/User/DTOs/AuthenticationDTOs.cs`:
- **LoginRequestDto**: Richiesta di login
- **LoginResponseDto**: Risposta di login con sessione utente
- **UserSessionDto**: Informazioni della sessione utente
- **ChangePasswordRequestDto**: Richiesta cambio password
- **ChangePasswordResponseDto**: Risposta cambio password
- **ValidateUserRequestDto**: Richiesta validazione utente
- **ValidateUserResponseDto**: Risposta validazione utente

#### Commands
`Features/User/Commands/AuthenticationCommands.cs`:
- **LoginCommand**: Comando per login
- **ChangePasswordCommand**: Comando per cambio password
- **ValidateUserQuery**: Query per validazione utente

#### Handlers
- **LoginCommandHandler.cs**: Gestisce il processo di login
  - Verifica esistenza utente
  - Controlla stato (bloccato/abilitato)
  - Valida credenziali
  - Gestisce tentativi falliti
  - Crea sessione utente
  
- **ChangePasswordCommandHandler.cs**: Gestisce il cambio password
  - Valida password corrente
  - Verifica policy password (lunghezza, cifre)
  - Controlla storico password
  - Aggiorna password nel database

#### Services
- **IAuthenticationService**: Interfaccia del servizio di autenticazione
- Metodi principali:
  - `LoginAsync()`: Login standard
  - `ForceLoginAsync()`: Login forzato (disconnette sessione precedente)
  - `ChangePasswordAsync()`: Cambio password
  - `ValidateUserAsync()`: Validazione credenziali

#### Repository Contracts
`Contracts/IUserRepository.cs`:
```csharp
Task<bool> Exists(string usrId);
Task<User?> GetByIdAsync(string usrId);
Task<bool> ValidateCredentialsAsync(string usrId, string password);
Task<int> IncrementFailedAttemptsAsync(string usrId);
Task ResetFailedAttemptsAsync(string usrId);
Task BlockUserAsync(string usrId);
Task UpdatePasswordAsync(string usrId, string newPassword);
Task<bool> IsPasswordInHistoryAsync(string usrId, string password, int historyCount);
Task<bool> IsUserBlockedAsync(string usrId);
Task<bool> IsUserEnabledAsync(string usrId);
```

### 3. Infrastructure Layer (`eTeller.Infrastructure`)
Implementa i dettagli tecnici e l'accesso ai dati.

#### Repositories
`Repositories/Archivi/UserRepository.cs`:
Implementa tutti i metodi dell'interfaccia `IUserRepository` usando Entity Framework Core.

#### Services
`Services/AuthenticationService.cs`:
Implementazione concreta del servizio di autenticazione che orchestra i vari handler e include:
- Sanitizzazione input per prevenire injection attacks
- Gestione errori centralizzata
- Logging delle operazioni

#### DbContext
`Context/eTellerDbContext.cs`:
Aggiunto `DbSet<User> Users` per l'accesso alla tabella `sys_USERS`.

### 4. API Layer (`eTeller.Api`)
Espone gli endpoint REST per l'autenticazione.

#### Controllers
`Controllers/Authentication/AuthenticationController.cs`:

**Endpoints:**

1. **POST /api/authentication/login**
   - Login standard
   - Input: `LoginRequestDto` (UserId, Password, IpAddress, etc.)
   - Output: `LoginResponseDto` con token/sessione

2. **POST /api/authentication/force-login**
   - Login forzato (disconnette sessione esistente)
   - Input: `LoginRequestDto`
   - Output: `LoginResponseDto`

3. **POST /api/authentication/change-password**
   - Cambio password (richiede autenticazione)
   - Input: `ChangePasswordRequestDto`
   - Output: `ChangePasswordResponseDto`

4. **POST /api/authentication/validate**
   - Validazione credenziali senza creare sessione
   - Input: `ValidateUserRequestDto`
   - Output: `ValidateUserResponseDto`

5. **POST /api/authentication/logout**
   - Logout utente (richiede autenticazione)
   - Output: Conferma logout

## Flusso di Login

```
1. Client → POST /api/authentication/login
2. Controller → AuthenticationService.LoginAsync()
3. Service → LoginCommandHandler.Handle()
4. Handler esegue:
   ✓ Verifica esistenza utente (UserRepository.Exists)
   ✓ Controlla se bloccato (UserRepository.IsUserBlockedAsync)
   ✓ Controlla se abilitato (UserRepository.IsUserEnabledAsync)
   ✓ Valida credenziali (UserRepository.ValidateCredentialsAsync)
   ✓ Se credenziali errate → Incrementa tentativi (UserRepository.IncrementFailedAttemptsAsync)
   ✓ Se credenziali OK → Reset tentativi (UserRepository.ResetFailedAttemptsAsync)
   ✓ Crea UserSessionDto
5. Response → LoginResponseDto con sessione utente
```

## Flusso di Cambio Password

```
1. Client → POST /api/authentication/change-password
2. Controller → AuthenticationService.ChangePasswordAsync()
3. Service → ChangePasswordCommandHandler.Handle()
4. Handler esegue:
   ✓ Valida conferma password
   ✓ Verifica password corrente (UserRepository.ValidateCredentialsAsync)
   ✓ Valida lunghezza minima (MIN = 8 caratteri)
   ✓ Valida cifre minime (MIN = 2 cifre)
   ✓ Controlla storico (UserRepository.IsPasswordInHistoryAsync)
   ✓ Aggiorna password (UserRepository.UpdatePasswordAsync)
5. Response → ChangePasswordResponseDto
```

## Sicurezza Implementata

### 1. Protezione da Injection
- Sanitizzazione input nel servizio (`SanitizeInput()`)
- Rimozione caratteri pericolosi: `'`, `"`, `<`, `>`, `&`, `;`, `--`, `/*`, `*/`

### 2. Gestione Tentativi Falliti
- Massimo 3 tentativi (configurabile in `PasswordPolicyConstants.MaxFailedAttempts`)
- Blocco automatico utente dopo 3 tentativi
- Reset tentativi dopo login riuscito

### 3. Policy Password
- Lunghezza minima: 8 caratteri
- Cifre minime: 2
- Controllo storico: ultime 5 password (TODO: implementare tabella storico)

### 4. Logging
- Log di tutti i tentativi di login (successo/fallimento)
- Log cambio password
- Log con IP address e UserId

### 5. Autorizzazione
- Endpoint login/validate: `[AllowAnonymous]`
- Endpoint change-password: `[Authorize]`
- Endpoint logout: `[Authorize]`

## Configurazione

### 1. Dependency Injection
Nel `Program.cs` o `Startup.cs`:

```csharp
// Registra DbContext
services.AddDbContext<eTellerDbContext>(options =>
    options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

// Registra UnitOfWork e Repositories
services.AddScoped<IUnitOfWork, UnitOfWork>();
services.AddScoped<IUserRepository, UserRepository>();

// Registra Authentication Service
services.AddScoped<IAuthenticationService, AuthenticationService>();
```

### 2. Database
Assicurarsi che la tabella `sys_USERS` esista con le seguenti colonne:
- USR_ID (varchar, PK)
- USR_HOST_ID (varchar, nullable)
- USR_BRA_ID (varchar)
- USR_STATUS (varchar)
- USR_EXTREF (varchar, nullable)
- USR_PASS (nvarchar)
- USR_CHG_PAS (bit)
- USR_LINGUA (varchar)
- USR_TENTATIVI (int)
- USR_MAIL (varchar, nullable)

## TODO / Miglioramenti Futuri

### Alta Priorità
1. **Password Hashing**: Implementare BCrypt o Argon2 per hashing password
2. **Password History Table**: Creare tabella per storico password
3. **Session Management**: Implementare gestione sessioni (Redis/Database)
4. **JWT Tokens**: Implementare generazione e validazione JWT per API

### Media Priorità
5. **Rate Limiting**: Limitare numero di tentativi login per IP
6. **2FA**: Implementare autenticazione a due fattori
7. **Password Reset**: Funzionalità di reset password via email
8. **Audit Log**: Tabella dedicata per audit delle operazioni di sicurezza

### Bassa Priorità
9. **Permissions System**: Sistema di permessi granulari
10. **Branch Validation**: Validazione filiale per postazioni cassa
11. **MAC Address Check**: Controllo MAC address per casse
12. **Session Timeout**: Configurazione timeout sessione dinamica

## Testing

### Unit Tests (da implementare)
```csharp
// LoginCommandHandlerTests.cs
- Test_LoginWithValidCredentials_ReturnsSuccess
- Test_LoginWithInvalidCredentials_ReturnsUnauthorized
- Test_LoginWithBlockedUser_ReturnsBlocked
- Test_LoginWithMaxFailedAttempts_BlocksUser

// ChangePasswordCommandHandlerTests.cs
- Test_ChangePasswordWithValidData_ReturnsSuccess
- Test_ChangePasswordTooShort_ReturnsError
- Test_ChangePasswordInsufficientDigits_ReturnsError
- Test_ChangePasswordInHistory_ReturnsError
```

### Integration Tests (da implementare)
```csharp
// AuthenticationControllerTests.cs
- Test_POST_Login_WithValidCredentials_Returns200
- Test_POST_Login_WithInvalidCredentials_Returns401
- Test_POST_ChangePassword_WithValidData_Returns200
- Test_POST_ChangePassword_Unauthorized_Returns401
```

## Esempio di Utilizzo

### Login
```http
POST /api/authentication/login
Content-Type: application/json

{
  "userId": "user001",
  "password": "Password123",
  "ipAddress": "192.168.1.100",
  "isCashDesk": false
}
```

**Risposta Successo:**
```json
{
  "isSuccessful": true,
  "resultCode": "OK",
  "message": "Login successful.",
  "userSession": {
    "userId": "user001",
    "userName": "Mario Rossi",
    "branchId": "BR001",
    "language": "IT",
    "canUseTeller": true,
    "cashDeskId": null,
    "sessionStartTime": "2026-02-27T10:30:00",
    "sessionExpirationTime": "2026-02-27T20:30:00"
  },
  "requiresPasswordChange": false
}
```

### Cambio Password
```http
POST /api/authentication/change-password
Content-Type: application/json
Authorization: Bearer <token>

{
  "userId": "user001",
  "currentPassword": "Password123",
  "newPassword": "NewPassword456",
  "confirmPassword": "NewPassword456"
}
```

**Risposta Successo:**
```json
{
  "isSuccessful": true,
  "message": "Password changed successfully.",
  "errors": null
}
```

## Migrazione dal Vecchio Sistema

### Differenze Principali
1. **Architettura**: Da monolitico (Login.aspx.cs) a layered DDD
2. **API**: Da Web Forms a REST API
3. **Dependency Injection**: Gestione dipendenze moderna
4. **Async/Await**: Operazioni database asincrone
5. **Logging**: Logging strutturato invece di messaggi UI

### Mapping Funzionalità

| Vecchio Sistema | Nuovo Sistema |
|----------------|---------------|
| `ButtonLogin_Click()` | `POST /api/authentication/login` |
| `ButtonForza_Click()` | `POST /api/authentication/force-login` |
| `btnChangePassword_Click()` | `POST /api/authentication/change-password` |
| `etellerAuthmanager.UserIdentify()` | `UserRepository.ValidateCredentialsAsync()` |
| `etellerAuthmanager.IncrCount()` | `UserRepository.IncrementFailedAttemptsAsync()` |
| `StartLogin()` | `LoginCommandHandler.Handle()` |
| Error codes from DB | `AuthenticationResultConstants` |

## Contatti e Supporto
Per domande o problemi con il sistema di autenticazione, contattare il team di sviluppo.

---
**Ultima modifica**: 27 Febbraio 2026  
**Versione**: 1.0.0
