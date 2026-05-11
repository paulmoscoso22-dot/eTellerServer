---
name: creating-gh-issues
description: >
  Crea issue GitHub per attività ASP.NET 10 / backend nel repository eTellerServer.
  Usa questa skill quando l'utente vuole registrare una nuova funzionalità,
  un bug, un refactoring o un'ottimizzazione relativi esclusivamente al layer
  backend ASP.NET (eTellerServer). NON usare per task frontend (Angular / eTellerClient).
applyTo: "eTellerServer/**"
---

## Contesto del Progetto

- **Repository:** `paulmoscoso22-dot/eTellerServer`
- **URL:** https://github.com/paulmoscoso22-dot/eTellerServer.git
- **Stack backend:** ASP.NET 10, CQRS, MediatR, Clean Architecture, DDD, Entity Framework Core

---

## Quando Usare questa Skill

Invoca questa skill quando l'utente chiede di:
- Aggiungere un nuovo Command, Query o Handler MediatR
- Creare o modificare un Controller / Endpoint API
- Aggiungere o modificare un Repository, UnitOfWork o DbContext
- Correggere un bug backend (logica, validazione, persistenza)
- Refactoring di codice ASP.NET (Clean Architecture, DDD, naming C#)
- Aggiungere test unitari o di integrazione per handler, repository, controller
- Migrare logica dal progetto legacy `eTeller2022`

---

## Protocollo di Creazione Issue

### 1. Definizione Input

- Genera un **titolo conciso** in italiano (es. `[Backend] Aggiungere paginazione all'endpoint GET /api/transactions`).
- Se la richiesta è vaga, fai **al massimo due domande** prima di procedere.
- Prefissa sempre il titolo con `[Backend]` per distinguere dai task frontend.

### 2. Struttura del Corpo Issue

Usa sempre questo template Markdown:

```markdown
## Descrizione
<!-- Cosa deve fare questa funzionalità / qual è il problema? -->

## Layer coinvolto
- [ ] Controller / Endpoint API
- [ ] Command (CQRS)
- [ ] Query (CQRS)
- [ ] Handler (MediatR)
- [ ] Repository / Unit of Work
- [ ] Domain (Entity, Value Object, Aggregate)
- [ ] Infrastructure (DbContext, EF Migrations)
- [ ] Validator (FluentValidation)
- [ ] Test (unit / integrazione)
- [ ] Migrazione da eTeller2022 legacy

## File / Percorsi rilevanti
<!-- es. eTeller.Application/Features/Transactions/Queries/GetTransactionsQuery.cs -->

## Contratto API (se applicabile)
- Metodo HTTP:
- Endpoint:
- Request body / query params:
- Response:

## Passi operativi
1. 
2. 
3. 

## Acceptance Criteria
- [ ] ...
- [ ] ...

## Note tecniche
<!-- CQRS, MediatR pipeline, naming C#, Entity Framework, FluentValidation, ecc. -->

## Stima
<!-- XS (<2h) / S (2-4h) / M (1gg) / L (2-3gg) / XL (>3gg) -->
```

### 3. Etichette (Labels)

| Scenario | Labels da applicare |
|---|---|
| Nuovo endpoint / feature | `enhancement`, `aspnet`, `backend` |
| Bug logica / persistenza | `bug`, `aspnet`, `backend` |
| Refactoring / clean code | `refactoring`, `aspnet` |
| CQRS / MediatR | `cqrs`, `aspnet`, `backend` |
| Domain / DDD | `domain`, `aspnet`, `backend` |
| EF Core / Migrations | `ef-core`, `aspnet`, `backend` |
| Performance / query DB | `performance`, `aspnet` |
| Test backend | `testing`, `aspnet` |
| Migrazione legacy | `migration`, `aspnet`, `backend` |

### 4. Creazione via MCP GitHub Tool

Chiama il tool MCP GitHub con:
- `owner`: `paulmoscoso22-dot`
- `repo`: `eTellerServer`
- `title`: il titolo generato
- `body`: il corpo compilato con il template sopra
- `labels`: array di label appropriate dalla tabella

### 5. Conferma e Tracciabilità

- Dopo la creazione, comunica all'utente l'**URL diretto** all'issue.
- Se l'utente sta lavorando su un file specifico, aggiungi un commento in cima al file con il link all'issue.
- Inserisci nel corpo dell'issue il percorso del file C# rilevante (es. `eTeller.Application/Features/...`).

---

## Esempi di Titoli per Tipo di Task

| Tipo | Esempio Titolo |
|---|---|
| Nuova pagina | `[Angular] Creare pagina lista ordini con DevExtreme DataGrid` |
| Bug componente | `[Angular] Fix: il componente currency-form non resetta i campi dopo il salvataggio` |
| Refactoring | `[Angular] Refactoring transactions-list con Signals e standalone component` |
| Performance | `[Angular] Ottimizzare change detection nella pagina dashboard` |
| Test | `[Angular] Aggiungere unit test per currency.service.ts` |

---

## Esempi di Titoli per Tipo di Task Backend

| Tipo | Esempio Titolo |
|---|---|
| Nuovo endpoint | `[Backend] Aggiungere endpoint GET /api/transactions con paginazione` |
| Command CQRS | `[Backend] Creare CreateTransactionCommand con handler e validator` |
| Query CQRS | `[Backend] Implementare GetCurrencyCouplesQuery con filtri e ordering` |
| Bug handler | `[Backend] Fix: UpdateCurrencyHandler non gestisce il caso di divisa non trovata` |
| Repository | `[Backend] Aggiungere metodo GetPagedAsync in TransactionRepository` |
| Migrazione legacy | `[Backend] Migrare logica di eTellerDAL/CurrencyBusiness.cs in Clean Architecture` |
| EF Migration | `[Backend] Aggiungere migration EF Core per indice su colonna Timestamp` |
| Test | `[Backend] Aggiungere unit test per GetCurrencyCouplesQueryHandler` |