---
name: query-security
description: >
  Regole di sicurezza per tutte le query nel backend eTeller: EF Core LINQ, Dapper, raw SQL,
  stored procedure. Definisce pattern sicuri obbligatori, pattern vietati, e checklist OWASP A03.
  Da usare come riferimento primario da agent-query-security-review e da qualsiasi agente che
  scrive query, repository o accessi al database.
applyTo: "eTellerServer/**"
---

# Query Security — eTeller Backend

## Principio Fondamentale

> **Regola assoluta:** nessun input proveniente dall'esterno (utente, rete, file, configurazione)
> deve mai essere concatenato in una stringa SQL. Ogni valore variabile deve essere trasmesso
> al database come **parametro**, mai come testo inline nel comando SQL.

---

## 1. EF Core LINQ — Pattern Obbligatori

### ✅ SICURO — Predicato LINQ (parametri automatici)
```csharp
// EF Core traduce in: WHERE USR_ID = @p0
var user = await _context.Users
    .FirstOrDefaultAsync(u => u.UsrId == userId, cancellationToken);

// EF Core traduce in: WHERE TABLE_SCHEMA = @p0 AND TABLE_NAME = @p1
bool exists = await _context.InformationSchemaTables
    .AnyAsync(t => t.TABLE_SCHEMA == schema && t.TABLE_NAME == tableName, cancellationToken);
```

### ✅ SICURO — FromSqlInterpolated (parametri da interpolazione)
```csharp
// L'interpolazione $"..." con FromSqlInterpolated genera parametri — NON è concatenazione
var result = await _context.Users
    .FromSqlInterpolated($"EXEC sys_USERS_Exits @USR_ID = {userId}")
    .ToListAsync(cancellationToken);
```

### ❌ VIETATO — FromSqlRaw con input esterno concatenato
```csharp
// INJECTION — non fare mai così
var result = await _context.Users
    .FromSqlRaw($"SELECT * FROM Users WHERE USR_ID = '{userId}'")
    .ToListAsync();

// INJECTION — neanche con string.Format
var result = await _context.Users
    .FromSqlRaw("SELECT * FROM Users WHERE USR_ID = '" + userId + "'")
    .ToListAsync();
```

### ✅ SICURO — FromSqlRaw con SqlParameter espliciti
```csharp
// FromSqlRaw è accettabile SOLO con SqlParameter — mai con interpolazione o concatenazione
var param = new SqlParameter("@USR_ID", userId);
var result = await _context.Users
    .FromSqlRaw("EXEC sys_USERS_Exits @USR_ID", param)
    .ToListAsync(cancellationToken);
```

### ❌ VIETATO — ExecuteSqlRawAsync con input esterno
```csharp
// INJECTION
await _context.Database.ExecuteSqlRawAsync(
    $"UPDATE Users SET IS_ACTIVE = 0 WHERE USR_ID = '{userId}'");
```

### ✅ SICURO — ExecuteSqlInterpolatedAsync
```csharp
// Usa sempre la variante Interpolated per input variabili
await _context.Database.ExecuteSqlInterpolatedAsync(
    $"UPDATE Users SET IS_ACTIVE = 0 WHERE USR_ID = {userId}",
    cancellationToken);
```

---

## 2. Dapper — Pattern Obbligatori

### ✅ SICURO — Parametri anonimi (object param)
```csharp
var user = await connection.QuerySingleOrDefaultAsync<User>(
    "EXEC sys_USERS_Exits @USR_ID",
    new { USR_ID = userId });

// Con query inline: sempre parametri, mai interpolazione
var result = await connection.QueryAsync<Session>(
    "SELECT * FROM sys_USER_SESSIONS WHERE USR_ID = @UsrId AND IS_ACTIVE = @IsActive",
    new { UsrId = userId, IsActive = true });
```

### ✅ SICURO — DynamicParameters
```csharp
var parameters = new DynamicParameters();
parameters.Add("@USR_ID", userId, DbType.String, ParameterDirection.Input, 50);
parameters.Add("@PWD",    hashedPassword, DbType.String, ParameterDirection.Input, 255);

var result = await connection.ExecuteAsync("sys_USERS_VerifyCredentials", parameters,
    commandType: CommandType.StoredProcedure);
```

### ❌ VIETATO — Interpolazione di stringhe in Dapper
```csharp
// INJECTION
await connection.QueryAsync<User>($"SELECT * FROM Users WHERE USR_ID = '{userId}'");

// INJECTION — anche con string.Format
await connection.QueryAsync<User>(
    string.Format("SELECT * FROM Users WHERE USR_ID = '{0}'", userId));
```

---

## 3. DDL / Codice di Inizializzazione DB

Il DDL (CREATE TABLE, ALTER TABLE, CREATE INDEX) è accettabile come raw SQL **solo se**:

- Il testo SQL contiene **esclusivamente costanti hardcoded** (nessuna variabile)
- Non viene mai costruito tramite concatenazione o interpolazione di input esterno

### ✅ SICURO — DDL con sole costanti
```csharp
// Accettabile: nessuna variabile nel testo SQL
await context.Database.ExecuteSqlRawAsync("""
    CREATE TABLE [dbo].[sys_USER_SESSIONS] (
        [SESSION_ID] INT IDENTITY(1,1) NOT NULL,
        ...
    )
    """, cancellationToken);
```

### ❌ VIETATO — DDL con nomi tabella variabili
```csharp
// INJECTION — il nome tabella non deve mai venire dall'esterno
await context.Database.ExecuteSqlRawAsync(
    $"CREATE TABLE {tableName} ([ID] INT)", cancellationToken);
```

---

## 4. Stored Procedure

### Regola
Le stored procedure sono **il pattern preferito** per operazioni legacy.
Passare sempre i parametri — mai costruire la chiamata con concatenazione.

### ✅ SICURO
```csharp
// EF Core
var param = new SqlParameter("@USR_ID", SqlDbType.NVarChar, 50) { Value = userId };
await _context.Database.ExecuteSqlRawAsync("EXEC sys_USERS_IncrCnt @USR_ID", param, cancellationToken);

// Dapper
await conn.ExecuteAsync("sys_USERS_IncrCnt",
    new { USR_ID = userId },
    commandType: CommandType.StoredProcedure);
```

---

## 5. Input Sanitization (SanitizeInput)

Prima di qualsiasi accesso al DB, gli input stringa provenienti dall'esterno (API, header, body)
devono essere validati da FluentValidation. La pulizia dell'input non sostituisce la
parametrizzazione — entrambe devono essere presenti.

### Regole di Validazione Obbligatorie

| Campo | Regole FluentValidation |
|---|---|
| `UserId` | `NotEmpty()`, `MaximumLength(50)`, `Matches(@"^[a-zA-Z0-9_\-]+$")` |
| `IpAddress` | `NotEmpty()`, validazione formato IP (`Matches` o custom validator) |
| `CliId` | `MaximumLength(50)`, `Matches(@"^[a-zA-Z0-9_\-]*$")` quando presente |
| Qualsiasi string free-text | `MaximumLength(N)`, trim, `NotEmpty()` se obbligatorio |

---

## 6. Checklist OWASP A03 — Injection

Prima di approvare qualsiasi file repository o query, verificare:

- [ ] **Nessuna concatenazione di stringhe** nel testo SQL (operatori `+`, `$"..."` inline in raw SQL)
- [ ] **Nessun `string.Format`** che costruisce query SQL
- [ ] **`FromSqlRaw`** usato solo con `SqlParameter` espliciti — mai con input esterno interpolato
- [ ] **`ExecuteSqlRawAsync`** usato solo per DDL con costanti — mai per DML con variabili
- [ ] **`ExecuteSqlInterpolatedAsync`** usato per DML con variabili (EF parametrizza automaticamente)
- [ ] **Dapper**: tutti i valori passati come parametri anonimi o `DynamicParameters`
- [ ] **FluentValidation** presente su ogni Command/Query che porta input esterno al DB
- [ ] **Nomi colonna/tabella** non provengono mai da input esterno (whitelist o hardcoded)
- [ ] **Nessun input esterno** nei costruttori di `DbCommand` o `SqlCommand` senza parametri

---

## 7. Riferimenti Rapidi — Quale API Usare

| Scenario | API Corretta | API Vietata |
|---|---|---|
| Query con filtro su input | `LINQ AnyAsync / FirstOrDefaultAsync` | `FromSqlRaw($"...{input}...")` |
| Chiamata stored procedure | `FromSqlInterpolated` o `Dapper + SP` | `FromSqlRaw("... '" + val + "'")` |
| Aggiornamento con input | `ExecuteSqlInterpolatedAsync` | `ExecuteSqlRawAsync($"...{input}...")` |
| DDL di inizializzazione | `ExecuteSqlRawAsync` (costanti only) | `ExecuteSqlRawAsync($"...{var}...")` |
| Join / proiezioni complesse | `LINQ` su DbSet mappati | Query inline con concatenazione |
| Controllo esistenza tabella | `LINQ AnyAsync su InformationSchemaTables` | `SqlQuery<int>($"...{name}...")` |
