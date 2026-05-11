---
name: auth-flow
description: >
  Documenta il flow completo di autenticazione del sistema eTeller: stati, sequenza operazioni,
  codici di risposta e gestione casi eccezionali (utente bloccato, sessione esistente, cambio password obbligatorio).
  Da usare come riferimento da tutti gli agenti che lavorano su login e sicurezza backend.
applyTo: "eTellerServer/**"
---

# Auth Flow — eTeller Backend

## Flow Principale: Login

```
Client                          API                          Application                  DB
  |                              |                                |                        |
  |-- POST /api/auth/login ----→|                                |                        |
  |                              |-- LoginCommand (MediatR) ----→|                        |
  |                              |                                |-- SanitizeInput        |
  |                              |                                |-- Exists(userId) -----→|
  |                              |                                |←-- bool ---------------| 
  |                              |                                |-- IsBlocked(userId) --→|
  |                              |                                |←-- bool ---------------|
  |                              |                                |-- IsEnabled(userId) --→|
  |                              |                                |←-- bool ---------------|
  |                              |                                |-- ValidateCredentials →|
  |                              |                                |←-- bool ---------------|
  |                              |                                |-- CheckPasswordExpiry →|
  |                              |                                |←-- bool ---------------|
  |                              |                                |-- WhoIsLogged(ip) ----→|
  |                              |                                |←-- userId? ------------|
  |                              |                                |-- GenerateToken()      |
  |                              |                                |-- InsertTrace()  -----→|
  |                              |                                |-- UnitOfWork.Complete→|
  |                              |←-- LoginVm --------------------|                        |
  |←-- 200 OK + LoginVm --------|                                |                        |
```

---

## Stati del Login

| Codice Risposta | ResultCode | Condizione |
|---|---|---|
| `200 OK` | `OK` | Login riuscito |
| `401 Unauthorized` | `USER_NOT_FOUND` | UserId inesistente — **usare messaggio generico** |
| `401 Unauthorized` | `INVALID_CREDENTIALS` | Password errata — **usare messaggio generico** |
| `401 Unauthorized` | `USER_BLOCKED` | Utente bloccato dopo N tentativi |
| `401 Unauthorized` | `USER_DISABLED` | Utente non abilitato |
| `200 OK` | `PASSWORD_EXPIRED` | Credenziali OK ma password scaduta → `RequiresPasswordChange = true` |
| `200 OK` | `USER_ALREADY_LOGGED` | Sessione già attiva da altro IP → `UserAlreadyLogged = true`, `ForceLogin` disponibile |
| `400 Bad Request` | `INVALID_REQUEST` | Input mancante o malformato |
| `500 Internal Server Error` | `ERROR` | Errore imprevisto — **mai esporre dettagli al client** |

> ⚠️ **Regola anti user-enumeration:** I codici `USER_NOT_FOUND` e `INVALID_CREDENTIALS` devono tornare
> lo stesso messaggio testuale generico al client: `"Credenziali non valide."` Il `ResultCode` differenziato
> è solo per log interni — **non inviarlo mai nella response al client**.

---

## Flow: Force Login

Attivato quando `UserAlreadyLogged = true` e il client invia `ForceLogin = true`.

```
1. Verifica credenziali (stesso flow principale)
2. Verifica che UserAlreadyLogged = true
3. Invalida la sessione esistente (Logout forzato dell'utente precedente)
4. Crea nuova sessione
5. Genera nuovo token JWT
6. InsertTrace con tipo "FORCE_LOGIN"
7. Ritorna LoginVm con IsSuccessful = true
```

---

## Flow: Logout

```
Client                          API                          Application                  DB
  |                              |                                |                        |
  |-- POST /api/auth/logout ---→|                                |                        |
  |   (userId nel token JWT)     |-- LogoutCommand (MediatR) ---→|                        |
  |                              |                                |-- Invalida sessione --→|
  |                              |                                |-- InsertTrace() ------→|
  |                              |                                |-- UnitOfWork.Complete→|
  |                              |←-- LogoutVm { Success=true }--|                        |
  |←-- 200 OK ------------------|                                |                        |
```

---

## Flow: Change Password

```
1. Valida che UserId = userId dal token JWT (non da input client — prevenzione privilege escalation)
2. Valida CurrentPassword (stesse regole del login)
3. Valida NewPassword vs policy (vedi skill password-policy)
4. Verifica che NewPassword != CurrentPassword
5. Verifica che NewPassword == ConfirmPassword
6. Aggiorna hash password nel DB
7. Resetta flag PasswordExpired
8. InsertTrace con tipo "CHANGE_PASSWORD"
9. Ritorna ChangePasswordVm { IsSuccessful = true }
```

---

## Flow: Validate User (per operazioni interne)

Usato dal frontend per verificare la sessione prima di operazioni critiche (es. conferma transazione).

```
1. Verifica validità token JWT (non scaduto, firma valida)
2. Estrai UserId dai claims
3. Verifica che l'utente esista e non sia bloccato
4. Ritorna ValidateVm { IsValid = true/false }
```

---

## Gestione Tentativi Falliti

| Tentativo | Azione |
|---|---|
| 1–2 fallimenti | Solo log Warning |
| 3 fallimenti | Log Warning + incrementa contatore |
| 5+ fallimenti | Blocca utente (`USR_STATUS = 'B'`) + Log Error + notifica |

> Il contatore va resettato ad ogni login riuscito.

---

## Trace Obbligatoria

Per ogni operazione di autenticazione va inserita una trace con `_unitOfWork.TraceRepository.InsertTrace(...)`:

| Operazione | Tipo Trace |
|---|---|
| Login riuscito | `"LOGIN"` |
| Login fallito | `"LOGIN_FAILED"` |
| Force Login | `"FORCE_LOGIN"` |
| Logout | `"LOGOUT"` |
| Change Password | `"CHANGE_PASSWORD"` |
| Utente bloccato | `"USER_BLOCKED"` |
