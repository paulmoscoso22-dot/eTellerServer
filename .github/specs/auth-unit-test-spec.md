# SPEC — Unit Test Sicurezza & Autorizzazione — Modulo Auth Backend

> **⚠️ DOCUMENTO VIVO** — Deve essere aggiornato prima di qualsiasi modifica ai test del modulo di autenticazione.
> Nessuna issue relativa ai test auth può essere creata o lavorata senza che questo documento sia allineato.

| Campo | Valore |
|---|---|
| Versione | 1.0 — APPROVATO |
| Data creazione | 2026-05-11 |
| Ultimo aggiornamento | 2026-05-11 |
| Stato | 🟢 APPROVATO — procedi con la creazione delle issue |
| Spec correlata | `eTellerServer/.github/specs/login-migration-spec.md` |
| Responsabile revisione | Utente / Team |

---

## 1. Obiettivo

Verificare con unit test che il modulo `eTeller.Auth` rispetti:

1. **Parità funzionale** con il sistema legacy eTeller2022 — ogni step del flow login deve produrre lo stesso risultato del vecchio `etellerAuthmanager`.
2. **Sicurezza OWASP** — il sistema deve resistere a tutti gli attacchi principali su autenticazione e gestione sessioni.
3. **Nessuna regressione** — ogni modifica futura al modulo auth deve essere coperta prima di poter essere mergiata.

---

## 2. Progetto di Test

### 2.1 Progetto Target

I test vanno aggiunti al progetto **esistente**:
```
eTellerServer/eTeller.Application.UnitTests/
```

> ⚠️ Non creare un nuovo progetto separato.

### 2.2 Dipendenze da Aggiungere al `.csproj`

| Package / Reference | Motivo |
|---|---|
| `ProjectReference` → `eTeller.Auth.csproj` | Accesso agli handler e validator da testare |
| `BCrypt.Net-Next` v4.0.3 | Generare hash BCrypt nei test di verifica password |
| `FluentValidation` v11+ | Istanziare i validator nei test |

### 2.3 Struttura Cartelle da Creare

```
eTeller.Application.UnitTests/
  Feature/
    Manager/              ← già esiste — non toccare
    Auth/                 ← NUOVA
      Login/
        LoginCommandHandlerTests.cs
      ForceLogin/
        ForceLoginCommandHandlerTests.cs
      Logout/
        LogoutCommandHandlerTests.cs
      ChangePassword/
        ChangePasswordCommandHandlerTests.cs
      Validators/
        LoginCommandValidatorTests.cs
        ChangePasswordCommandValidatorTests.cs
```

---

## 3. Convenzioni dei Test

- **Framework:** xUnit + Moq (stesso stack esistente)
- **Naming:** `{Metodo}_{Condizione}_{RisultatoAtteso}` — es. `Handle_PasswordMd5Valida_RehashBCrypt`
- **Struttura:** AAA (Arrange / Act / Assert) con commenti separatori
- **Mock:** tutti i repository e i servizi esterni vengono mockati tramite `Mock<IUnitOfWork>`, `Mock<ITokenService>`, ecc.
- **Categoria:** ogni test è decorato con `[Trait("Category", "Security")]` per i test di attacco, `[Trait("Category", "FlowLegacy")]` per i test di parità con eTeller2022

---

## 4. Casi di Test per File

### 4.1 `LoginCommandHandlerTests` — 20 test

#### 4.1.1 Flusso Legacy (Parità eTeller2022)

| ID | Nome Test | Step Legacy | Risultato Atteso |
|---|---|---|---|
| L-01 | `Handle_UtenteNonTrovato_ReturnInvalidCredentials` | Step 1 | `ResultCode = "INVALID_CREDENTIALS"` |
| L-02 | `Handle_UtenteBloccato_ReturnUserBlocked` | Step 2 | `ResultCode = "USER_BLOCKED"` |
| L-03 | `Handle_UtenteNonAbilitato_ReturnUserDisabled` | Step 3 | `ResultCode = "USER_DISABLED"` |
| L-04 | `Handle_PasswordBCryptErrata_ReturnInvalidCredentials` | Step 4 | `ResultCode = "INVALID_CREDENTIALS"` |
| L-05 | `Handle_PasswordMd5Errata_ReturnInvalidCredentials` | Step 4 | `ResultCode = "INVALID_CREDENTIALS"` |
| L-06 | `Handle_PasswordMd5Valida_EsegueMigrazioneRehash` | Step 5 | `UpdatePasswordAsync` chiamato con hash `$2...` |
| L-07 | `Handle_PasswordBCryptValida_NessunRehash` | Step 5 | `UpdatePasswordAsync` NON chiamato |
| L-08 | `Handle_LoginOk_AzzeraContatoretentativi` | Step 6 | `ResetFailedAttemptsAsync` chiamato |
| L-09 | `Handle_UsrChgPasTrue_ReturnMustChangePassword` | Step 7 | `ResultCode = "MUST_CHANGE_PASSWORD"`, `RequiresPasswordChange = true` |
| L-10 | `Handle_SessioneGiaAttiva_ReturnUserAlreadyLogged` | Step 8 | `ResultCode = "USER_ALREADY_LOGGED"`, `UserAlreadyLogged = true` |
| L-11 | `Handle_LoginCompletoOk_ReturnTokenESessione` | Step 9–11 | `ResultCode = "OK"`, `AccessToken != null`, sessione creata |
| L-12 | `Handle_EccezioneInterna_ReturnErrorSenzaDettagli` | catch | `ResultCode = "ERROR"`, messaggio generico |

#### 4.1.2 Scenari di Attacco

| ID | Nome Test | Tipo Attacco | Risultato Atteso |
|---|---|---|---|
| S-01 | `Handle_UserNotFoundEInvalidCredentials_StessoMessaggioClient` | User Enumeration | `Message` identico nei due casi — client non distingue |
| S-02 | `Handle_SqlInjectionInUserId_ReturnInvalidCredentials` | SQL Injection | Input `'; DROP TABLE sys_USERS; --` → `INVALID_CREDENTIALS`, nessuna eccezione |
| S-03 | `Handle_SqlInjectionInPassword_ReturnInvalidCredentials` | SQL Injection | Input `' OR '1'='1` → `INVALID_CREDENTIALS` |
| S-04 | `Handle_UserIdLunghissimo_NonCrasha` | Buffer overflow / DoS | Input 1000 caratteri → `INVALID_CREDENTIALS` senza crash |
| S-05 | `Handle_BruteForce_SogliaMenoUno_UtenteAncoraAttivo` | Brute Force | N-1 tentativi → utente non bloccato |
| S-06 | `Handle_BruteForce_SogliaRaggiunta_UtenteBloccato` | Brute Force | N-esimo tentativo → `USER_BLOCKED` |
| S-07 | `Handle_UserIdSoloSpazi_NonBypassaAuth` | Input manipulation | Trim → utente non trovato → `INVALID_CREDENTIALS` |
| S-08 | `Handle_PasswordVuota_NonBypassaVerifica` | Empty bypass | Stringa vuota → `INVALID_CREDENTIALS`, verifica non saltata |

---

### 4.2 `ForceLoginCommandHandlerTests` — 7 test

#### 4.2.1 Flusso Normale

| ID | Nome Test | Risultato Atteso |
|---|---|---|
| L-01 | `Handle_CredenzialiCorrette_SessioneTerminataELoginOk` | `TerminateSessionAsync` chiamato, poi `LoginCommand` delegato, `ResultCode = "OK"` |
| L-02 | `Handle_CredenzialiErrate_ReturnInvalidCredentials` | `ResultCode = "INVALID_CREDENTIALS"` |
| L-03 | `Handle_UtenteNonTrovato_ReturnInvalidCredentials` | `ResultCode = "INVALID_CREDENTIALS"` |
| L-04 | `Handle_LoginCommandReturnBlocked_PropagaRisultato` | `ResultCode = "USER_BLOCKED"` propagato da `LoginCommand` |

#### 4.2.2 Scenari di Attacco

| ID | Nome Test | Tipo Attacco | Risultato Atteso |
|---|---|---|---|
| S-01 | `Handle_CredenzialiErrate_SessioneNonTerminata` | Session Hijacking | `TerminateSessionAsync` NON deve essere chiamato se credenziali errate |
| S-02 | `Handle_SqlInjectionInUserId_SessioneNonTerminata` | SQL Injection + Session Hijack | Nessun side-effect su sessioni altrui |
| S-03 | `Handle_UtenteBloccatoConCredenzialiCorrette_NessunaSessioneCreata` | Bypass blocco | `ResultCode = "USER_BLOCKED"`, `CreateSessionAsync` NON chiamato |

---

### 4.3 `LogoutCommandHandlerTests` — 4 test

#### 4.3.1 Flusso Normale

| ID | Nome Test | Risultato Atteso |
|---|---|---|
| L-01 | `Handle_LogoutOk_SessioneTerminataETraceInserita` | `TerminateSessionAsync` chiamato, trace inserita, `ResultCode = "OK"` |
| L-02 | `Handle_SessioneNonEsistente_IdempotenteSenzaErrore` | `TerminateSessionAsync` chiamato ugualmente, `ResultCode = "OK"` |
| L-03 | `Handle_EccezioneInterna_ReturnError` | `ResultCode = "ERROR"` |

#### 4.3.2 Scenari di Attacco

| ID | Nome Test | Tipo Attacco | Risultato Atteso |
|---|---|---|---|
| S-01 | `Handle_UserIdSanitizzato_ToUpperInvariantApplicato` | Input manipulation | UserId viene normalizzato prima di terminare la sessione |

---

### 4.4 `ChangePasswordCommandHandlerTests` — 10 test

#### 4.4.1 Flusso Normale

| ID | Nome Test | Risultato Atteso |
|---|---|---|
| L-01 | `Handle_CambioPasswordOk_HashBCryptSalvato` | `UpdatePasswordAsync` chiamato con hash `$2...`, `ResultCode = "OK"` |
| L-02 | `Handle_UtenteNonTrovato_ReturnError` | `ResultCode = "ERROR"` |
| L-03 | `Handle_PasswordCorrenteErrata_ReturnInvalidCurrentPassword` | `ResultCode = "INVALID_CURRENT_PASSWORD"` |
| L-04 | `Handle_NuovaPasswordUgualeCorrente_ReturnPolicyViolation` | `ResultCode = "POLICY_VIOLATION"` |
| L-05 | `Handle_NuovaPasswordInStorico_ReturnHistoryViolation` | `ResultCode = "HISTORY_VIOLATION"` |
| L-06 | `Handle_EccezioneInterna_ReturnError` | `ResultCode = "ERROR"` |

#### 4.4.2 Scenari di Attacco

| ID | Nome Test | Tipo Attacco | Risultato Atteso |
|---|---|---|---|
| S-01 | `Handle_PasswordNonAppareNeiLog_MockLoggerVerificato` | Credential leakage nei log | `Mock<ILogger>` verifica che nessuna call contenga la password |
| S-02 | `Handle_NuovaPasswordSalvataHashBCrypt_NonPlainText` | Credential storage insicuro | `UpdatePasswordAsync` chiamato con stringa che inizia con `$2` |
| S-03 | `Handle_LimiteStoricoEsatto_PasswordRifiutata` | Policy bypass | Al limite `historyLimit` la password è rifiutata |
| S-04 | `Handle_OltreStoricoLimite_PasswordAccettata` | Policy bypass | Oltre il limite la password riusata è accettata |

---

### 4.5 `LoginCommandValidatorTests` — 3 test

| ID | Nome Test | Risultato Atteso |
|---|---|---|
| V-01 | `Validate_UserIdVuoto_ErroreObbligatorio` | Errore su `UserId` |
| V-02 | `Validate_PasswordVuota_ErroreObbligatorio` | Errore su `Password` |
| V-03 | `Validate_DatiValidi_NessunErrore` | `IsValid = true` |

---

### 4.6 `ChangePasswordCommandValidatorTests` — 5 test

| ID | Nome Test | Caso | Risultato Atteso |
|---|---|---|---|
| V-01 | `Validate_PasswordTroppoCorta_Errore` | < 8 caratteri | Errore su `NewPassword` |
| V-02 | `Validate_PasswordSenzaMaiuscola_Errore` | Solo minuscole | Errore su `NewPassword` |
| V-03 | `Validate_PasswordSenzaCifra_Errore` | Solo lettere | Errore su `NewPassword` |
| V-04 | `Validate_PasswordSenzaCarattereSpeciale_Errore` | Alfanumerica | Errore su `NewPassword` |
| V-05 | `Validate_PasswordValida_NessunErrore` | Rispetta tutte le regole | `IsValid = true` |

---

## 5. Riepilogo Copertura

| File | Test Flusso Legacy | Test Attacchi | Totale |
|---|---|---|---|
| `LoginCommandHandlerTests` | 12 | 8 | **20** |
| `ForceLoginCommandHandlerTests` | 4 | 3 | **7** |
| `LogoutCommandHandlerTests` | 3 | 1 | **4** |
| `ChangePasswordCommandHandlerTests` | 6 | 4 | **10** |
| `LoginCommandValidatorTests` | 3 | — | **3** |
| `ChangePasswordCommandValidatorTests` | 5 | — | **5** |
| **Totale** | **33** | **16** | **49** |

---

## 6. Acceptance Criteria Globali

- [ ] Tutti i 49 test devono passare con `dotnet test`
- [ ] Nessun test di attacco deve produrre un'eccezione non gestita
- [ ] I test `[Trait("Category", "Security")]` devono essere eseguibili separatamente
- [ ] I messaggi client per `USER_NOT_FOUND` e `INVALID_CREDENTIALS` devono essere **identici** (anti user-enumeration)
- [ ] Nessuna password in plain text deve apparire nei mock del logger
- [ ] `UpdatePasswordAsync` deve sempre essere chiamato con hash BCrypt (`$2...`), mai plain text
- [ ] `TerminateSessionAsync` NON deve mai essere chiamato se le credenziali ForceLogin sono errate

---

## 7. Piano Issue GitHub

| # | Titolo Issue | File Target | Agenti | Stima |
|---|---|---|---|---|
| 1 | `[Test] Setup progetto — aggiungere dipendenze auth a eTeller.Application.UnitTests` | `.csproj` | `checkcode-agent` | XS (2h) |
| 2 | `[Test] LoginCommandHandler — flusso legacy (L-01 → L-12)` | `LoginCommandHandlerTests.cs` | `checkcode-agent` → `agent-query-security-review` | M (1gg) |
| 3 | `[Test] LoginCommandHandler — scenari attacco (S-01 → S-08)` | `LoginCommandHandlerTests.cs` | `checkcode-agent` → `agent-query-security-review` | M (1gg) |
| 4 | `[Test] ForceLoginCommandHandler — flusso legacy + session hijacking` | `ForceLoginCommandHandlerTests.cs` | `checkcode-agent` → `agent-query-security-review` | S (4h) |
| 5 | `[Test] LogoutCommandHandler — flusso normale + attacchi` | `LogoutCommandHandlerTests.cs` | `checkcode-agent` | S (2h) |
| 6 | `[Test] ChangePasswordCommandHandler — flusso legacy + credential security` | `ChangePasswordCommandHandlerTests.cs` | `checkcode-agent` → `agent-query-security-review` | M (1gg) |
| 7 | `[Test] Validator — LoginCommandValidator + ChangePasswordCommandValidator` | `Validators/` | `checkcode-agent` | XS (2h) |
