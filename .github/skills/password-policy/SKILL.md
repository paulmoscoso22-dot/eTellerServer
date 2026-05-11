---
name: password-policy
description: >
  Regole di dominio per la gestione delle password nel sistema eTeller: lunghezza, complessità,
  scadenza, hashing e policy di cambio obbligatorio. Da usare come riferimento da tutti gli agenti
  che lavorano su autenticazione, cambio password o validazione credenziali.
applyTo: "eTellerServer/**"
---

# Password Policy — eTeller Backend

## Regole di Complessità

Una password è valida se rispetta **tutte** le seguenti regole:

| Regola | Valore minimo | Validator FluentValidation |
|---|---|---|
| Lunghezza minima | 8 caratteri | `MinimumLength(8)` |
| Lunghezza massima | 64 caratteri | `MaximumLength(64)` |
| Almeno 1 lettera maiuscola | 1 | `Matches(@"[A-Z]")` |
| Almeno 1 lettera minuscola | 1 | `Matches(@"[a-z]")` |
| Almeno 1 cifra | 1 | `Matches(@"[0-9]")` |
| Almeno 1 carattere speciale | 1 | `Matches(@"[^a-zA-Z0-9]")` |
| Non uguale a UserId | — | Logica nell'Handler |
| Non uguale alla password precedente | — | Logica nell'Handler (verifica hash) |

---

## Validator FluentValidation (riferimento)

```csharp
public class ChangePasswordCommandValidator : AbstractValidator<ChangePasswordCommand>
{
    public ChangePasswordCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("UserId è obbligatorio.");

        RuleFor(x => x.CurrentPassword)
            .NotEmpty().WithMessage("La password corrente è obbligatoria.");

        RuleFor(x => x.NewPassword)
            .NotEmpty().WithMessage("La nuova password è obbligatoria.")
            .MinimumLength(8).WithMessage("La password deve essere di almeno 8 caratteri.")
            .MaximumLength(64).WithMessage("La password non può superare 64 caratteri.")
            .Matches(@"[A-Z]").WithMessage("La password deve contenere almeno una lettera maiuscola.")
            .Matches(@"[a-z]").WithMessage("La password deve contenere almeno una lettera minuscola.")
            .Matches(@"[0-9]").WithMessage("La password deve contenere almeno una cifra.")
            .Matches(@"[^a-zA-Z0-9]").WithMessage("La password deve contenere almeno un carattere speciale.");

        RuleFor(x => x.ConfirmPassword)
            .NotEmpty().WithMessage("La conferma password è obbligatoria.")
            .Equal(x => x.NewPassword).WithMessage("La conferma password non corrisponde.");
    }
}
```

---

## Scadenza Password

| Scenario | Comportamento |
|---|---|
| Password mai cambiata dopo N giorni | `RequiresPasswordChange = true` nella risposta di login |
| Password scaduta (flag DB attivo) | Login consentito ma `RequiresPasswordChange = true` |
| Primo accesso dopo reset admin | `RequiresPasswordChange = true` obbligatorio |

> La durata della scadenza è configurabile in `appsettings.json`:
> ```json
> "PasswordPolicy": {
>   "ExpirationDays": 90,
>   "ForceChangeOnFirstLogin": true
> }
> ```

---

## Hashing delle Password

```csharp
// ✅ OBBLIGATORIO — usare BCrypt o PBKDF2
// MAI confrontare password in chiaro con == o String.Compare

// Esempio con BCrypt (libreria BCrypt.Net-Next)
string hash = BCrypt.Net.BCrypt.HashPassword(plainPassword, workFactor: 12);
bool isValid = BCrypt.Net.BCrypt.Verify(plainPassword, storedHash);

// ❌ VIETATO — algoritmi deboli
string hash = MD5.HashData(Encoding.UTF8.GetBytes(password));   // INSICURO
string hash = SHA1.HashData(Encoding.UTF8.GetBytes(password));  // INSICURO
```

---

## Blocco Account dopo Tentativi Falliti

```
Tentativi falliti consecutivi → Azione
─────────────────────────────────────────
1 - 2  → Solo log Warning, nessun blocco
3 - 4  → Log Warning + incrementa contatore in DB
5+     → Blocca account (USR_STATUS = 'B') + Log Error
```

Il contatore `FailedLoginAttempts` va:
- **Incrementato** ad ogni login fallito con credenziali errate
- **Azzerato** ad ogni login riuscito
- **Non toccato** quando il blocco è per altri motivi (utente disabilitato)

---

## Reset Password da Amministratore

Quando un amministratore resetta la password di un utente:

1. Genera una password temporanea sicura (min 12 caratteri, random)
2. Imposta il flag `ForcePasswordChange = true` nel DB
3. Azzera il contatore `FailedLoginAttempts`
4. Sblocca l'account se era bloccato (`USR_STATUS = 'A'`)
5. **Non** inviare la password temporanea via email in chiaro — usare canale sicuro

---

## Regole di Sicurezza Password

| Regola | Dettaglio |
|---|---|
| ❌ Mai loggare password | Nemmeno a livello `Debug` o `Trace` |
| ❌ Mai trasmettere password in chiaro | Solo HTTPS, mai in query string |
| ❌ Mai confrontare senza hashing | Usare sempre `BCrypt.Verify()` o equivalente |
| ❌ Mai mostrare password nei log di errore | Nei catch, escludere sempre il campo password |
| ✅ Risposta generica su credenziali errate | Messaggio unico: `"Credenziali non valide."` |
| ✅ Tempo di risposta costante | Usare confronto tempo-costante per prevenire timing attack |
