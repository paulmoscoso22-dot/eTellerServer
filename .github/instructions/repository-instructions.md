---
description:  convenzioni C#, repository pattern, unit of work

# applyTo: 'Describe when these instructions should be loaded by the agent based on task context' # when provided, instructions will automatically be added to the request context when the pattern matches an attached file
---

<!-- Tip: Use /create-instructions in chat to generate content with agent assistance -->

---
description: Linee guida per la creazione di Repository in C# (eTeller Project) seguendo il pattern Unit of Work ed evitando logiche di mapping o business.
---

# C# Repository Guidelines (eTeller)

Segui queste istruzioni per creare o aggiornare le classi Repository. L'obiettivo è mantenere i repository "puri", limitandoli alle sole operazioni di accesso ai dati su entità di dominio.

## 1. Regole d'Oro
- **No Mapping:** Non utilizzare AutoMapper o logiche manuali per convertire entità in DTO/ViewModel. Il repository ritorna **esclusivamente** `Domain.Models` — mai ViewModel, mai DTO.
- **Interfaccia del repository:** Il tipo di ritorno di ogni metodo nell'interfaccia (`IXxxRepository`) deve essere un `Domain.Models` o una collezione di essi. Se vedi un ViewModel come tipo di ritorno nell'interfaccia, è un errore.
- **No Business Logic:** Non inserire validazioni o calcoli. Se serve logica, questa va nell'Handler dell'Application.
- **No SaveChanges:** Non chiamare mai `_context.SaveChangesAsync()`. Il salvataggio è responsabilità della **Unit of Work** richiamata dall'Handler.
- **Solo Accesso Dati:** Il metodo deve limitarsi a interrogare o preparare lo stato del database.
- **SP su tabella singola → NO repository dedicato:** Se una stored procedure (o query) coinvolge **una sola tabella**, non creare un repository dedicato. Usa direttamente `_unitOfWork.Repository<TEntity>()` nell'Handler. Aggiungi l'entità al `DbContext` con la configurazione EF Core appropriata (`ToTable`, `HasKey`, ecc.).

### Quando creare un repository dedicato (Dapper)

| Scenario | Approccio |
|---|---|
| SP su **1 tabella** — SELECT/INSERT/UPDATE/DELETE semplice | `_unitOfWork.Repository<T>()` nell'Handler — **nessun repository** |
| SP su **2+ tabelle** (join, logica cross-entity) | Repository dedicato con Dapper (`IXxxRepository` + implementazione) |
| SP con **output parameters** o logica complessa | Repository dedicato con Dapper |

> ⚠️ **Anti-pattern:** creare un `IXxxRepository` con metodi `GetAllAsync()`, `InsertAsync()`, `UpdateAsync()` che wrappano SP su tabella singola è ridondante. EF Core via `Repository<T>` è sufficiente e più coerente con il resto del progetto.

> ⚠️ **Anti-pattern da evitare:** Qualsiasi `using` su namespace `Mappings.*` o `Contracts.*` dentro un file Repository è un segnale di errore. Il mapping avviene nel **QueryHandler** tramite `_mapper.Map<>()`.

## 2. Esempio di Riferimento — Repository puro

Il repository ritorna l'entità di dominio pura. La mappatura a ViewModel avviene **nel QueryHandler** tramite `_mapper.Map<>()`.

```csharp
// ✅ CORRETTO — Repository ritorna Domain.Models
public interface ICorsiRepository
{
    Task<IEnumerable<CorsiResult>> GetAllAsync(...);  // ← Domain.Models, NON ViewModel
}

public class CorsiRepository : BaseSimpleRepository<CorsiResult>, ICorsiRepository
{
    public async Task<IEnumerable<CorsiResult>> GetAllAsync(...)
    {
        // Solo accesso dati — nessun Map, nessun Select(...=> new CorsiVm)
        var result = await connection.QueryAsync<CorsiResult>("dbo.sp_...", ...);
        return result;
    }
}

// ✅ CORRETTO — Il mapping avviene nel QueryHandler
public class GetAllCorsiQueryHandler : IRequestHandler<GetAllCorsiQuery, IEnumerable<CorsiVm>>
{
    public async Task<IEnumerable<CorsiVm>> Handle(GetAllCorsiQuery request, CancellationToken ct)
    {
        var entities = await _unitOfWork.CorsiRepository.GetAllAsync(...);
        return _mapper.Map<IEnumerable<CorsiVm>>(entities);  // ← mapping qui
    }
}
```

```csharp
// ❌ SBAGLIATO — Repository che fa mapping (viola le regole)
public async Task<IEnumerable<CorsiVm>> GetAllAsync(...)  // ← ViewModel nel repository
{
    var result = await connection.QueryAsync<CorsiResult>(...);
    return result.Select(c => new CorsiVm { ... });  // ← mapping nel repository
}
```