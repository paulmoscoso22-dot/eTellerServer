# SPEC — Migrazione Login eTeller2022 → eTeller2026

> **⚠️ DOCUMENTO VIVO** — Deve essere aggiornato prima di qualsiasi modifica al modulo di autenticazione.
> Nessuna issue relativa al login può essere creata o lavorata senza che questo documento sia allineato.

| Campo | Valore |
|---|---|
| Versione | 0.3 — APPROVATO |
| Data creazione | 2026-05-11 |
| Ultimo aggiornamento | 2026-05-12 |
| Stato | 🟢 APPROVATO — Backend completo, Frontend in esecuzione |
| Responsabile revisione | Utente / Team |

---

## 1. Obiettivo

Migrare la logica di autenticazione dal sistema legacy **eTeller2022** (ASP.NET WebForms + FormsAuthentication) al nuovo sistema **eTeller2026** (ASP.NET 10 + Angular 21), mantenendo la stessa logica di business del vecchio sistema e adeguando la sicurezza agli standard moderni.

Il login sarà implementato come **modulo autonomo e separato** sia lato backend che lato frontend, indipendente dal modulo applicativo principale.

---

## 2. Architettura del Modulo Auth — Decisioni Prese

### 2.1 Backend — Nuovo Progetto `eTeller.Auth`

Il login **non** viene collocato in `eTeller.Application/Features/Auth/`.
Viene creato un progetto separato nella solution `eTellerServer.sln`.

```
eTellerServer.sln
  eTeller.Domain/              ← condiviso (entità utente, sessione)
  eTeller.Application/         ← logica applicativa principale (invariata)
  eTeller.Auth/                ← NUOVO — modulo login autonomo
    Features/
      Login/
        LoginCommand + Handler + Validator
      ForceLogin/
        ForceLoginCommand + Handler
      Logout/
        LogoutCommand + Handler
      ChangePassword/
        ChangePasswordCommand + Handler + Validator
    Contracts/
      ITokenService.cs
      ISessionRepository.cs
    ViewModels/
      LoginVm.cs
      ChangePasswordVm.cs
      LogoutVm.cs
  eTeller.Infrastructure/      ← condiviso (repository, DbContext)
  eTeller.Api/                 ← API principale (invariata)
  eTeller.Auth.Api/            ← NUOVO — API endpoint login separati
    Controllers/
      AuthController.cs
    Program.cs / ServiceRegistration
```

**Motivazione:** Separazione netta delle responsabilità. Il token JWT viene emesso esclusivamente da `eTeller.Auth.Api`. L'API principale (`eTeller.Api`) valida il token ma non lo emette.

### 2.2 Frontend — Modulo Angular `auth` Separato

```
eTellerClient/src/app/
  auth/                        ← NUOVO — modulo standalone lazy-loaded
    login/
      login.component.ts
      login.service.ts
    change-password/
      change-password.component.ts
    force-login/
      force-login.component.ts
  main/                        ← applicazione principale (caricata dopo login)
```

**Routing:**
- `/auth/login` → modulo auth — nessun guard JWT
- `/auth/change-password` → modulo auth — nessun guard JWT
- `/**` → modulo main — guard JWT obbligatorio

---

## 3. Logica di Business — Flow Login (AS-IS da eTeller2022)

Questa è la sequenza **esatta** del vecchio sistema che deve essere replicata nel nuovo.

### 3.1 Sequenza Login Principale

| Step | Descrizione | Classe Legacy | SP Database |
|---|---|---|---|
| 1 | Rileva se il PC è una cassa (`isCassa`) via IP | `etellerAuthmanager.PCisCassa()` | `ClientDB.IP2ID()` |
| 2 | Se cassa: verifica stato cassa (`ENABLED`), MAC address valido, stampante fiche (1 sola) | `ClientDB` | `sys_CLIENTS_*` |
| 3 | Verifica che l'utente esista | `UserDB.Exits()` | `sys_USERS_Exits` |
| 4 | Verifica che l'utente non sia bloccato | `UserDB.ExitsAndNotBlocked()` | `sys_USERS_ExitsAndNotBlocked` |
| 5 | Verifica credenziali (password hashata **MD5**) | `UserDB.VerifyUserAndPassword()` | `sys_USERS_*` |
| 6 | Verifica scadenza password | `UserPasswordsDB.checkPasswordIsExpired()` | `sys_USER_PASSWD_Select_Last_User_Password` |
| 7 | Se password errata → incrementa contatore fallimenti | `UserDB.IncrCount()` | `sys_USERS_IncrCnt` |
| 8 | Se contatore ≥ soglia → blocca utente | `PersonalizationDB.GetValue("PASSWORD-TENTATIVI")` | `PERSONALISATION` |
| 9 | Se login OK → resetta contatore | `UserDB.ResetCount()` | `sys_USERS_ResetCnt` |
| 10 | Verifica stato utente (`ENABLED`) | `etellerAuthmanager.userStatus()` | `sys_USERS` |
| 11 | Verifica se deve cambiare password (flag DB) | `UserDB.MustChangePassword()` | `sys_USERS` |
| 12 | Verifica se utente già loggato dallo stesso IP | `USRuseClientDB.GetByPara(CLI_ID, USR_ID, true)` | `sys_USERSuseClient_SelectByUSR_CLI_LOG` |
| 13 | Registra sessione in DB | `USRuseClientDB.EnterUser2()` | `sys_USERSuseClient_Insert` |
| 14 | Recupera dati sessione utente | `userBranchID`, `userLingua`, `UsrID2Name`, `CLIID2DES`, `BraID2DES` | `sys_USERS`, `sys_CLIENTS`, `sys_BRANCH` |
| 15 | Genera token di sessione | `FormsAuthenticationTicket` (legacy) → **JWT** (nuovo) | — |

### 3.2 Stati Risultato Login

| Stato Legacy (`LOGGEDSTATUS`) | Codice Risposta Nuovo | Significato |
|---|---|---|
| `OK` | `200 OK` / `ResultCode: "OK"` | Login riuscito |
| `USER_LOGGED` | `200 OK` / `ResultCode: "USER_ALREADY_LOGGED"` | Utente già loggato da altro IP — proponi ForceLogin |
| `CLI_OKKUPATO` | `200 OK` / `ResultCode: "CASH_DESK_BUSY"` | Cassa già occupata da altro utente |
| `KO` | `500` / `ResultCode: "ERROR"` | Errore inserimento sessione |
| *(password errata)* | `401` / `ResultCode: "INVALID_CREDENTIALS"` | Credenziali errate — messaggio generico |
| *(utente bloccato)* | `401` / `ResultCode: "USER_BLOCKED"` | Utente bloccato |
| *(utente non abilitato)* | `401` / `ResultCode: "USER_DISABLED"` | Utente non abilitato |
| *(password scaduta)* | `200 OK` / `ResultCode: "PASSWORD_EXPIRED"` | Login OK ma password scaduta |
| *(must change pass)* | `200 OK` / `ResultCode: "MUST_CHANGE_PASSWORD"` | Login OK ma cambio password obbligatorio |

### 3.3 Dati nel Token Legacy (FormsAuthenticationTicket)

Il vecchio sistema codificava nel cookie questi dati concatenati con `#`:
```
canUseTeller # CLI_ID # BRA_ID # USR_BRA_ID # CLI_STATUS # CLI_LINGUA # USR_LINGUA # DataLogin # IP # USR_NOME # CLI_DES # CLI_BRA_EXT
```

Nel nuovo sistema questi dati diventano **claims JWT**:

| Dato Legacy | Claim JWT Nuovo | Tipo |
|---|---|---|
| `USR_ID` (nome ticket) | `sub` | string |
| `USR_NOME` | `name` | string |
| `USR_BRA_ID` | `branch_id` | string |
| `USR_LINGUA` | `language` | string |
| `canUseTeller` | `can_use_teller` | bool |
| `CLI_ID` | `cash_desk_id` | string? |
| `BRA_ID` | `cash_desk_branch_id` | string? |
| `CLI_STATUS` | `cash_desk_status` | string? |
| `CLI_LINGUA` | `cash_desk_language` | string? |
| `CLI_DES` | `cash_desk_description` | string? |
| `IP` | `ip_address` | string |
| `DataLogin` | `session_start` | datetime (unix) |
| *(nuovo)* | `session_id` | GUID |
| *(nuovo)* | `jti` | GUID |

---

## 4. Problemi di Sicurezza — Migrazione MD5 → BCrypt

### 4.1 Situazione Legacy
Le password nel database eTeller2022 sono hashate con **MD5** (`ClassCifrarePass.CifraPass()`).
MD5 è un algoritmo insicuro e non deve essere mantenuto nel nuovo sistema.

### 4.2 Strategia di Migrazione (da validare)

> ⚠️ **Questa sezione è in attesa di approvazione** — non implementare prima della revisione.

Opzione proposta: **migrazione lazy al primo login**
1. Al login, si tenta prima la verifica con BCrypt (nuovo hash)
2. Se fallisce, si tenta con MD5 (vecchio hash)
3. Se MD5 è valido → si ricalcola e salva il nuovo hash BCrypt → si elimina l'hash MD5
4. Dopo la prima migrazione, l'utente usa solo BCrypt

### 4.3 Parametri di Policy Password (da DB)

I parametri di policy vengono letti dalla tabella `PERSONALISATION` del database:

| Parametro DB | Valore di default | Significato |
|---|---|---|
| `PASSWORD-TENTATIVI` | 5 | Max tentativi prima del blocco |
| `PasswordExpirePeriods` | 90 | Giorni prima della scadenza |
| `PasswordHistoryLimit` | 5 | Numero password precedenti non riutilizzabili |

---

## 5. Stored Procedure Legacy da Mappare

| SP Legacy | Scopo | Approccio nel Nuovo Sistema |
|---|---|---|
| `sys_USERS_Exits` | Utente esiste? | `_unitOfWork.Repository<sys_USERS>().GetAsync(...)` |
| `sys_USERS_ExitsAndNotBlocked` | Esiste e non bloccato? | `_unitOfWork.UserRepository` |
| `sys_USERS_IncrCnt` | Incrementa contatore tentativi | `_unitOfWork.UserRepository.IncrCount()` |
| `sys_USERS_ResetCnt` | Azzera contatore | `_unitOfWork.UserRepository.ResetCount()` |
| `sys_USER_PASSWD_Select_Last_User_Password` | Data ultima password (scadenza) | Repository Dapper dedicato |
| `sys_USER_PASSWD_Select_Count_for_History` | Storico password | Repository Dapper dedicato |
| `sys_USER_PASSWD_Insert_Password` | Salva nuova password | Repository Dapper dedicato |
| `sys_USER_PASSWD_Clear_Password_History` | Pulisce storico oltre limite | Repository Dapper dedicato |
| `sys_USERSuseClient_SelectByUSR_CLI_LOG` | Sessione attiva per utente/cassa | Repository Dapper (join multi-tabella) |
| `sys_USERSuseClient_Insert` | Crea nuova sessione | Repository Dapper |
| `sys_USERSuseClient_UpdateExit` | Chiude sessione (logout) | Repository Dapper |
| `sys_USERSuseClient_SelectLastExitUserByCLI_ID` | Ultimo utente su questa cassa | Repository Dapper |

---

## 6. Piano Issue GitHub

> Queste issue devono essere create da `agent-net-analysts` (Backend) e `agent-angular-analysts` (Frontend) **dopo** l'approvazione di questo documento.

### Backend (repo: paulmoscoso22-dot/eTellerServer)

| # | Titolo Issue | URL | Agenti | Dipende da | Stima |
|---|---|---|---|---|---|
| 1 | `[Auth] Struttura progetto eTeller.Auth e eTeller.Auth.Api` | [#29](https://github.com/paulmoscoso22-dot/eTellerServer/issues/29) | `architettureDDD-agent` → `checkcode-agent` | — | M (1gg) |
| 2 | `[Auth] Architettura CQRS LoginCommand — contratto API e JWT` | [#30](https://github.com/paulmoscoso22-dot/eTellerServer/issues/30) | `agent-auth-architect` | #29 | M (1gg) |
| 3 | `[Auth] Implementazione LoginCommand, ForceLoginCommand e sessioni DB` | [#31](https://github.com/paulmoscoso22-dot/eTellerServer/issues/31) | `agent-auth-implementor` | #30 | L (2-3gg) |
| 4 | `[Auth] Implementazione ChangePasswordCommand e LogoutCommand` | [#32](https://github.com/paulmoscoso22-dot/eTellerServer/issues/32) | `agent-auth-implementor` | #30, #31 | M (1gg) |
| 5 | `[Auth] Security Review OWASP — migrazione MD5→BCrypt e checklist sicurezza` | [#33](https://github.com/paulmoscoso22-dot/eTellerServer/issues/33) | `agent-auth-security-review` | #31, #32 | M (1gg) |

### Frontend (repo: paulmoscoso22-dot/eTellerClient)

| # | Titolo Issue | URL | Agenti | Dipende da | Stima |
|---|---|---|---|---|---|
| AUTH-01 | `[AUTH-01] Setup modulo auth — struttura cartelle, routing lazy-loaded, app.config.ts` | [#44](https://github.com/paulmoscoso22-dot/eTellerClient/issues/44) | `agent-auth-angular` → `agent-refactor` → `agent-code-review` | — | XS (2h) |
| AUTH-02 | `[AUTH-02] LoginComponent — form DevExtreme, validazione, gestione ResultCode` | [#45](https://github.com/paulmoscoso22-dot/eTellerClient/issues/45) | `agent-auth-angular` → `agent-refactor` → `agent-code-review` → `agent-angular-tester` → `agent-frontend-security-review` | AUTH-01, AUTH-03, AUTH-09 | S (4h) |
| AUTH-03 | `[AUTH-03] AuthService + AuthStore — chiamate HTTP e Signals globali` | [#46](https://github.com/paulmoscoso22-dot/eTellerClient/issues/46) | `agent-auth-angular` → `agent-refactor` → `agent-code-review` → `agent-angular-tester` → `agent-frontend-security-review` | AUTH-01, AUTH-09 | S (4h) |
| AUTH-04 | `[AUTH-04] ForceLoginComponent — dialog conferma force login` | [#47](https://github.com/paulmoscoso22-dot/eTellerClient/issues/47) | `agent-auth-angular` → `agent-refactor` → `agent-code-review` → `agent-angular-tester` → `agent-frontend-security-review` | AUTH-02, AUTH-03 | XS (3h) |
| AUTH-05 | `[AUTH-05] ChangePasswordComponent — cambio password obbligatorio e volontario` | [#48](https://github.com/paulmoscoso22-dot/eTellerClient/issues/48) | `agent-auth-angular` → `agent-refactor` → `agent-code-review` → `agent-angular-tester` → `agent-frontend-security-review` | AUTH-01, AUTH-03 | S (4h) |
| AUTH-06 | `[AUTH-06] HTTP Interceptors — authInterceptor, logInterceptor, errorInterceptor` | [#49](https://github.com/paulmoscoso22-dot/eTellerClient/issues/49) | `agent-auth-angular` → `agent-refactor` → `agent-code-review` → `agent-angular-tester` → `agent-frontend-security-review` | AUTH-01, AUTH-03 | S (3h) |
| AUTH-07 | `[AUTH-07] AuthGuard e RoleGuard — protezione route con JWT` | [#50](https://github.com/paulmoscoso22-dot/eTellerClient/issues/50) | `agent-auth-angular` → `agent-refactor` → `agent-code-review` → `agent-angular-tester` → `agent-frontend-security-review` | AUTH-03 | XS (2h) |
| AUTH-08 | `[AUTH-08] ErrorHandlerService e GlobalErrorHandler — gestione centralizzata errori UI` | [#51](https://github.com/paulmoscoso22-dot/eTellerClient/issues/51) | `agent-auth-angular` → `agent-refactor` → `agent-code-review` → `agent-angular-tester` → `agent-frontend-security-review` | AUTH-01 | XS (2h) |
| AUTH-09 | `[AUTH-09] Contratti TypeScript — modelli auth condivisi` | [#52](https://github.com/paulmoscoso22-dot/eTellerClient/issues/52) | `agent-auth-angular` → `agent-code-review` | — | XS (1h) |
| AUTH-10 | `[AUTH-10] Logout — pulsante, chiamata backend e cleanup sessione` | [#53](https://github.com/paulmoscoso22-dot/eTellerClient/issues/53) | `agent-auth-angular` → `agent-refactor` → `agent-code-review` → `agent-angular-tester` → `agent-frontend-security-review` | AUTH-03 | XS (2h) |
| AUTH-11 | `[AUTH-11] Test Jest — copertura completa modulo auth Angular (9 file spec)` | [#54](https://github.com/paulmoscoso22-dot/eTellerClient/issues/54) | `agent-angular-tester` → `agent-frontend-security-review` | AUTH-02…AUTH-10 | M (8h) |

### Sequenza di Esecuzione

```
Issue 1 → Issue 2 → Issue 3 → Issue 4 → Issue 5  [COMPLETATI ✅]
                  ↘
                   AUTH-09 → AUTH-01 → AUTH-03 → AUTH-08
                                            ↓
                              AUTH-02 → AUTH-04 → AUTH-06 → AUTH-07 → AUTH-10 → AUTH-05
                                            ↓
                                        AUTH-11  (test — dipende da tutti)
```

---

## 7. Contratto API — Endpoint del Modulo Auth

**Base URL:** `eTeller.Auth.Api` (porta dedicata — risposta Q1 pendente)

| Metodo | Endpoint | Body | Risposta | Guard |
|---|---|---|---|---|
| POST | `/api/auth/login` | `LoginCommand` | `LoginVm` | `[AllowAnonymous]` |
| POST | `/api/auth/force-login` | `ForceLoginCommand` | `LoginVm` | `[AllowAnonymous]` |
| POST | `/api/auth/logout` | — (claims dal JWT) | `LogoutVm` | `[Authorize]` |
| POST | `/api/auth/change-password` | `ChangePasswordRequest` | `ChangePasswordVm` | `[Authorize]` |

### Request/Response DTO (definitivi da Issue #30)

**LoginCommand / ForceLoginCommand:**
```json
{ "userId": "string", "password": "string", "cashDeskIp": "string?" }
```

**LoginVm:**
```json
{
  "resultCode": "OK|USER_ALREADY_LOGGED|CASH_DESK_BUSY|PASSWORD_EXPIRED|MUST_CHANGE_PASSWORD|INVALID_CREDENTIALS|USER_BLOCKED|USER_DISABLED|ERROR",
  "message": "string?",
  "accessToken": "string?",
  "tokenExpiresAt": "datetime?",
  "requiresPasswordChange": false,
  "userAlreadyLogged": false,
  "cashDeskBusy": false,
  "userSession": {
    "userId": "string",
    "userName": "string",
    "branchId": "string",
    "language": "string",
    "canUseTeller": true,
    "cashDeskId": "string?",
    "cashDeskDescription": "string?",
    "cashDeskBranchId": "string?"
  }
}
```

**ChangePasswordRequest:**
```json
{ "currentPassword": "string", "newPassword": "string" }
```

**ChangePasswordVm:**
```json
{ "resultCode": "OK|INVALID_CURRENT_PASSWORD|POLICY_VIOLATION|HISTORY_VIOLATION|ERROR", "message": "string?" }
```

**LogoutVm:**
```json
{ "resultCode": "OK|ERROR", "message": "string?" }
```

---

## 8. Agenti Coinvolti

| Agente | File | Ruolo in questo spec |
|---|---|---|
| .NET Analyst | `eTellerServer/.github/agents/agent-net-analysts.md` | Crea issue Backend 1–5 |
| Angular Analyst | `eTellerClient/.github/agents/agent-angular-analysts.md` | Crea issue Frontend AUTH-01…AUTH-11 |
| Architecture Agent | `eTellerServer/.github/agents/architettureDDD-agent.md` | Issue 1 — struttura progetto |
| CheckCode Agent | `eTellerServer/.github/agents/checkcode-agent.md` | Issue 1 — review struttura |
| Auth Architect | `eTellerServer/.github/agents/agent-auth-architect.md` | Issue 2 — architettura CQRS |
| Auth Implementor | `eTellerServer/.github/agents/agent-auth-implementor.md` | Issue 3, 4 — implementazione |
| Auth Security Review | `eTellerServer/.github/agents/agent-auth-security-review.md` | Issue 5 — review OWASP Backend |
| Angular Expert | `eTellerClient/.github/agents/agent-angular-expert.md` | Riservato per moduli non-auth |
| **auth-angular** | `eTellerClient/.github/agents/agent-auth-angular.md` | **NUOVO** — implementa tutti i componenti auth Angular |
| Refactor | `eTellerClient/.github/agents/agent-refactor.md` | Ottimizzazione + verifica memory leak (skill-memory-management) |
| Code Review | `eTellerClient/.github/agents/agent-code-review.md` | Validazione standard Angular |
| **angular-tester** | `eTellerClient/.github/agents/agent-angular-tester.md` | **NUOVO** — scrive i test Jest per il modulo auth |
| **frontend-security-review** | `eTellerClient/.github/agents/agent-frontend-security-review.md` | **NUOVO** — step finale OBBLIGATORIO per ogni issue auth Angular, review OWASP |

---

## 9. Domande Aperte (da risolvere prima di procedere)

| # | Domanda | Impatto | Stato |
|---|---|---|---|
| Q1 | Il progetto `eTeller.Auth.Api` deve girare sulla stessa porta di `eTeller.Api` o su una porta dedicata? | Architettura deploy | 🔴 Aperta |
| Q2 | La strategia migrazione MD5→BCrypt (lazy al primo login) è approvata? | Sicurezza Issue 5 | 🔴 Aperta |
| Q3 | Il modulo Angular `auth/` deve essere un'applicazione Angular separata o un modulo lazy-loaded nella stessa app? | Struttura frontend Issue 6 | � Risolta — modulo lazy-loaded nella stessa app (`src/app/auth/`) |
| Q4 | La durata del token JWT (8h proposta) è corretta per l'orario lavorativo bancario? | Contratto JWT | 🔴 Aperta |
| Q5 | La verifica `PCisCassa` (cassa con MAC address + stampante fiche) deve essere mantenuta nel nuovo sistema? | Logica business Issue 3 | 🔴 Aperta |
| Q6 | La tabella `sys_USERSuseClient` (sessioni attive) rimane nel DB esistente o si crea una nuova struttura? | Repository Issue 3 | 🔴 Aperta |

---

## 11. Contratto TypeScript Frontend

> Allineato al §7 Contratto API. File di riferimento: `src/app/core/auth/auth.models.ts` (issue AUTH-09).

### Interfacce

| Interfaccia | File | Utilizzo |
|---|---|---|
| `ILoginRequest` | `auth.models.ts` | Body per POST `/api/auth/login` e `/api/auth/force-login` |
| `ILoginResponse` | `auth.models.ts` | Risposta da `AuthService.login()` e `forceLogin()` |
| `IUserSession` | `auth.models.ts` | Stato utente decodificato dal JWT, conservato in `AuthStore` |
| `IChangePasswordRequest` | `auth.models.ts` | Body per POST `/api/auth/change-password` |
| `IChangePasswordResponse` | `auth.models.ts` | Risposta da `AuthService.changePassword()` |

### Mapping Claims JWT → IUserSession

| Claim JWT (Backend) | Campo TypeScript | Note |
|---|---|---|
| `sub` | `userId` | |
| `name` | `name` | |
| `branch_id` | `branchId` | |
| `language` | `language` | |
| `can_use_teller` | `canUseTeller` | `boolean` — usato da `RoleGuard` |
| `cash_desk_id` | `cashDeskId` | opzionale |
| `session_id` | `sessionId` | GUID sessione |
| `exp` | `tokenExpiry` | Unix timestamp |

### Token JWT — Storage e Ciclo di Vita

| Aspetto | Decisione |
|---|---|
| Storage | In-memory — `AuthStore.token` Signal — **mai** `localStorage` (XSS risk) |
| Scadenza | Al 401 → `AuthStore.reset()` + redirect `/auth/login` |
| Refresh token | ❌ Non implementato — no rinnovo automatico |
| Logout | `AuthService.logout()` → backend invalida sessione → `AuthStore.reset()` sempre |

---

## 12. Storico Revisioni

| Data | Versione | Autore | Modifica |
|---|---|---|---|
| 2026-05-11 | 0.1 | Orchestratore | Creazione documento — bozza da piano di pianificazione |
| 2026-05-11 | 0.2 | Utente | Approvazione spec — autorizzazione creazione issue GitHub |
| 2026-05-12 | 0.3 | Orchestratore | Backend completato (issue #29–#33 + test #39–#45 chiuse). Aggiunta infrastruttura Frontend: §6 aggiornato con 11 issue AUTH-01…AUTH-11 (#44–#54), §8 aggiornato con 3 nuovi agenti Angular, §11 nuovo (contratto TypeScript), §9 Q3 risolta |
| 2026-05-11 | 0.3 | Orchestratore | Aggiornamento URL issue GitHub create (#29-#33 eTellerServer, #43 eTellerClient) |
