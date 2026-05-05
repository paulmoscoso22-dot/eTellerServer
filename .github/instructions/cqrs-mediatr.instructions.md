---
description: "Use when creating or modifying Commands, Queries, Handlers, or Validators with MediatR and FluentValidation. Covers CQRS folder structure, naming conventions, IRequest patterns, IRequestHandler, AbstractValidator, and IUnitOfWork usage for this project."
applyTo:
  eTeller.Application/Features/**/Commands/**/*
  eTeller.Application/Features/**/Queries/**/*
---

# CQRS / MediatR – Istruzioni per questo progetto

## Struttura cartelle (obbligatoria)

```
eTeller.Application/Features/<Area>/
  Commands/
    <Nome>/
      <Nome>Command.cs
      <Nome>CommandHandler.cs
      <Nome>CommandValidator.cs   ← solo se il command ha input da validare
  Queries/
    <Nome>/
      <Nome>Query.cs
      <Nome>QueryHandler.cs
```

## Command

```csharp
// Usa record con parametri posizionali. Tipo di ritorno: ViewModel o int.
public record InsertUserCommand(
    string UsrId,
    string UsrBraId,
    string UsrStatus,
    string UsrLingua,
    string TraUser,
    string TraStation
) : IRequest<InsertUserVm>;
```

- Usa `record` (immutabile, C# 9+).
- Implementa `IRequest<T>` dove `T` è un ViewModel o `int`.
- Namespace: `eTeller.Application.Features.<Area>.Commands.<Nome>`.

## CommandHandler

```csharp
public class InsertUserCommandHandler : IRequestHandler<InsertUserCommand, InsertUserVm>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<InsertUserCommandHandler> _logger;

    public InsertUserCommandHandler(IUnitOfWork unitOfWork, IMapper mapper,
        ILogger<InsertUserCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<InsertUserVm> Handle(InsertUserCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling {CommandName}", nameof(InsertUserCommand));

        // ... logica di business ...

        await _unitOfWork.Complete();  // MAI SaveChanges() direttamente

        _logger.LogInformation("Handled {CommandName}", nameof(InsertUserCommand));
        return new InsertUserVm { /* ... */ };
    }
}
```

- Costruttore: sempre `IUnitOfWork` + `IMapper` + `ILogger<THandler>`.
- Usa `await _unitOfWork.Complete()` per salvare — mai `DbContext.SaveChanges()`.
- Logga con `_logger.LogInformation` all'inizio e alla fine; `LogError` su eccezioni.
- Scrivi la trace su `_unitOfWork.TraceRepository.InsertTrace(...)` per operazioni di scrittura.
- Lancia `InvalidOperationException` per errori di business (gestita dal middleware globale).

## CommandValidator

```csharp
public class InsertUserCommandValidator : AbstractValidator<InsertUserCommand>
{
    public InsertUserCommandValidator()
    {
        RuleFor(x => x.UsrId)
            .NotEmpty().WithMessage("USR_ID is required");

        RuleFor(x => x.UsrBraId)
            .NotEmpty().WithMessage("USR_BRA_ID is required");
    }
}
```

- Eredita da `AbstractValidator<TCommand>`.
- Valida nel costruttore con `RuleFor(...).NotEmpty()`, `.MaximumLength()`, ecc.
- Registrata automaticamente dal pipeline `ValidationBehaviour` — nessuna azione extra.
- **Non duplicare** validazione nei controller o nell'handler.

## Query

```csharp
// Query senza parametri
public record GetAccountQuery() : IRequest<IEnumerable<AccountVm>>;

// Query con parametri
public record GetAccountsByIacIdQuery(int IacId) : IRequest<IEnumerable<AccountVm>>;
```

- Stesso pattern del Command: `record` + `IRequest<T>`.
- Le Query **non modificano** stato — nessuna chiamata a `Complete()`.

## QueryHandler

```csharp
  public class GetFuncAccTypeQueryHandler : IRequestHandler<GetFuncAccTypeQuery, IEnumerable<StFunAcctypVm>>
  {
      private readonly IUnitOfWork _unitOfWork;
      private readonly IMapper _mapper;
      private readonly ILogger<GetFuncAccTypeQueryHandler> _logger;

      public GetFuncAccTypeQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, ILogger<GetFuncAccTypeQueryHandler> logger)
      {
          _unitOfWork = unitOfWork;
          _mapper = mapper;
          _logger = logger;
      }

      public async Task<IEnumerable<StFunAcctypVm>> Handle(GetFuncAccTypeQuery request, CancellationToken cancellationToken)
      {
          _logger.LogInformation("Handling {QueryName}", nameof(GetFuncAccTypeQuery));
          var stFunAcctyp = await _unitOfWork.Repository<StFunAcctyp>().GetAllAsync();
          var result = _mapper.Map<IEnumerable<StFunAcctypVm>>(stFunAcctyp);
          if (result == null) {
              _logger.LogError("La ricerca no ha prodotto nessnun resultato");
              throw new Exception("La ricerca no ha prodotto nessnun resultato");
          }     
          _logger.LogInformation("Handled {QueryName}, returned {Count} items", nameof(GetFuncAccTypeQuery), result.Count());
          return result;
      }
  }
```

## Regole rapide

| ✅ Fai | ❌ Non fare |
|---|---|
| `record` per Command/Query | `class` mutabile |
| `IUnitOfWork` negli handler | `DbContext` diretto |
| `_mapper.Map<>()` per i ViewModel **nell'Handler** | mapping manuale nei controller o nel repository |
| Repository ritorna `Domain.Models` | Repository ritorna ViewModel o DTO |
| `FluentValidation` nel Validator | validazione nell'handler o nel controller |
| `_logger.LogInformation/LogError` | `Console.Write` o `Debug.Write` |
| `await _unitOfWork.Complete()` | `SaveChanges()` diretto |
| Logica di business nell'handler | Logica di business nel controller |
