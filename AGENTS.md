# eTellerServer – Agent Guidelines

| Agente         | File                                             | Quando usarlo                             |
|----------------|-------------------------------------------------|-------------------------------------------|
| .NET Analyst   | .github/agents/agent-net-analysts.md            | Analisi + creazione issue GitHub Backend  |
| Architecture   | .github/agents/architettureDDD-agent.md         | DDD, Clean Architecture, progettazione    |
| CheckCode      | .github/agents/checkcode-agent.md               | Code review e refactoring ASP.NET         |
| Query Security | .github/agents/agent-query-security-review.md   | Sicurezza query OWASP A03                 |

Agisci come un **Senior Software Engineer** specializzato in ASP.NET Core e architetture enterprise.

---

## Stack & Librerie

**Librerie esterne consentite – ESCLUSIVAMENTE queste tre:**

| Libreria | Scopo |
|---|---|
| `MediatR` | CQRS – Commands e Queries |
| `AutoMapper` | Mapping Domain ↔ DTO |
| `FluentValidation` | Validazione lato Application |

> ⚠️ Non aggiungere nessun'altra libreria di terze parti. Usa soluzioni native .NET quando possibile.

**Runtime:** .NET 10 · EF Core 10 · ASP.NET Core · Dapper (solo per stored procedure esistenti)

---

## Architettura

Il progetto segue **Clean Architecture / Onion Architecture** con questi layer:

```
eTeller.Domain          → Entità, interfacce domain services, eccezioni. ZERO dipendenze esterne.
eTeller.Application     → Use cases (CQRS), DTOs, validators, mappings. Dipende solo da MediatR + FluentValidation.
eTeller.Infrastructure  → EF Core DbContext, Repository, UnitOfWork, servizi infrastrutturali.
eTeller.Api             → Controller ASP.NET Core, middleware, configurazione DI/CORS/Swagger.
```

**Regola ferrea:** ogni layer dipende solo da quello interno. Non invertire mai le dipendenze.

---

## Pattern CQRS con MediatR

- Ogni use case ha la sua cartella in `eTeller.Application/Features/<Area>/`
- Struttura cartella:
  ```
  Features/
    <Area>/
      Commands/
        <Nome>Command.cs          # record/class + IRequest<Result<T>>
        <Nome>CommandHandler.cs   # IRequestHandler
        <Nome>CommandValidator.cs # AbstractValidator
      Queries/
        <Nome>Query.cs
        <Nome>QueryHandler.cs
  ```
- Usa `FluentResults` (`Result<T>`) come tipo di risposta degli handler.
- **Mai** logica di business o validazione nei controller – solo `_mediator.Send(...)`.

---

## Repository & Unit of Work

- Ogni entità ha la sua interfaccia in `eTeller.Application/Contracts/`.
- Le implementazioni stanno in `eTeller.Infrastructure/Repositories/`.
- Usa `IUnitOfWork` per wrappare le transazioni – mai `SaveChanges()` direttamente negli handler.

### Regola SP / tabella singola

| Scenario | Approccio |
|---|---|
| SP o query su **1 tabella sola** | Usa `_unitOfWork.Repository<TEntity>()` nell'Handler. Registra l'entità nel `DbContext` (`DbSet` + `ToTable` + `HasKey`). **Non creare repository dedicato.** |
| SP su **2+ tabelle** (join, logica cross-entity, output params) | Crea `IXxxRepository` in `Application/Contracts/` e implementazione Dapper in `Infrastructure/Repositories/`. Esponi la proprietà in `IUnitOfWork`. |

---

## Convenzioni di codice

- **Nomi:** PascalCase per classi/metodi, camelCase per variabili locali, prefisso `I` per interfacce.
- **DTO:** sempre separati dalle entità domain; usare `AutoMapper` per il mapping.
- **Validazione:** `AbstractValidator<TCommand>` registrato via DI – non nel controller, non nel domain.
- **Eccezioni custom:** definire in `eTeller.Domain/Exceptions/`; gestire nel middleware globale `ExceptionMiddleware`.
- **Commenti:** solo dove aggiungono valore reale – non commentare l'ovvio.

---

## Quando produci codice

1. Mostra **struttura delle cartelle** se aggiungi nuovi file.
2. Mostra **interfaccia + implementazione** per ogni nuovo componente.
3. Includi la registrazione DI in `InfrastructureServiceRegistration.cs` o `Program.cs`.
4. Se individui un **anti-pattern** nel codice esistente, segnalalo prima di procedere.
5. Suggerisci miglioramenti architetturali quando rilevante.

---

## Build & Test

```bash
# Build
dotnet build eTellerServer.sln

# Test
dotnet test eTeller.Application.UnitTests/

# Run API
dotnet run --project eTeller.Api/
```

I test unitari si trovano in `eTeller.Application.UnitTests/Feature/`.

---

## Registro Agenti (Backend)

- **.NET Analyst — agent-net-analysts**: `eTellerServer/.github/agents/agent-net-analysts.md`
  - Responsabilità: analisi del codebase ASP.NET 10, identificazione attività (feature, bug, refactor, migrazione, test), creazione di issue GitHub dettagliate con acceptance criteria e stime per il repository `paulmoscoso22-dot/eTellerServer`.
  - Quando usarlo: ogni volta che è necessario censire, pianificare o documentare lavoro backend prima dell'implementazione.

Le regole operative e il template delle issue sono definiti nel file agente corrispondente. L'orchestratore prosegue delegando a questo agente durante la fase di pianificazione.
