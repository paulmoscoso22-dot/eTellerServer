---
description: Queste sono alcune regole importanti da seguire quando si lavora con i repository e le unità di lavoro nel progetto. Assicurati di non commettere questi errori comuni per garantire un codice pulito e funzionante.
# applyTo: 'Describe when these instructions should be loaded by the agent based on task context' # when provided, instructions will automatically be added to the request context when the pattern matches an attached file
---

<!-- Tip: Use /create-instructions in chat to generate content with agent assistance -->

## ⚠️ Errori Comuni da NON Ripetere

### `GetAllAsync()` non accetta CancellationToken
`IBaseSimpleRepository<T>.GetAllAsync()` non ha overload con parametri.  
Anche `GetAsync(predicate)` non accetta `CancellationToken`.

```csharp
// ❌ SBAGLIATO — CS1501: no overload accepts 1 arguments
var results = await _unitOfWork.Repository<StForceCode>().GetAllAsync(cancellationToken);

// ✅ CORRETTO
var results = await _unitOfWork.Repository<StForceCode>().GetAllAsync();
```

Stessa regola per `GetAsync(predicate)`:
```csharp
// ❌ SBAGLIATO
var results = await _unitOfWork.Repository<ErrorCode>().GetAsync(e => e.ErrId == id, cancellationToken);

// ✅ CORRETTO
var results = await _unitOfWork.Repository<ErrorCode>().GetAsync(e => e.ErrId == id);
```