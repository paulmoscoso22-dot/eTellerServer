# eTeller Server - Agent Guidelines

.NET 10.0 banking/teller app.

## Commands

```bash
# Build
dotnet build eTellerServer.sln

# Run dev server (localhost:5000/http, localhost:5001/https)
dotnet run --project eTeller.Api/eTeller.Api.csproj

# Tests
dotnet test
dotnet test --filter "FullyQualifiedName~Namespace.ClassName.MethodName"

# Docker
docker compose up --build -d
docker compose down
```

## Solution Structure

```
eTeller.Api/           → entrypoint
eTeller.Application/   → commands, queries, handlers, validators, DTOs
eTeller.Domain/         → entities, interfaces
eTeller.Infrastructure/  → repositories, DbContext, services
eTeller.Application.UnitTests/
```

## Architecture

`Controllers → Handlers (MediatR) → Repositories → DbContext`

- Features: `eTeller.Application/Features/[Feature]/`
- Contracts: `eTeller.Application/Contracts/`
- Repository impls: `eTeller.Infrastructure/Repositories/`

## Coding

- File-scoped namespaces: `namespace X.Y;`
- 4 spaces, max 120 chars
- `record` for DTOs, `class` for entities
- Error codes: strings (`"1305"`, `"9003"`)

## Critical: Repository Queries

```csharp
// ALWAYS use FirstOrDefault() for single entities
var entity = (await _unitOfWork.Repository<MyEntity>()
    .GetAsync(e => e.Id == id)).FirstOrDefault();
```

## Handler Pattern

```csharp
public async Task<Response> Handle(Command request, CancellationToken ct)
{
    try
    {
        // logic
        await _unitOfWork.Complete();
        return result;
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error handling {Name}", nameof(Command));
        await _unitOfWork.Rollback();
        throw;
    }
}
```

## Notes

- Passwords: plain text (TODO: implement hashing)
- CORS: Angular localhost:4200, Vite localhost:5253