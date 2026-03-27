# eTeller Server - Agent Guidelines

## Project Overview

This is a .NET 10.0 banking/teller application with a clean architecture pattern. The solution contains 4 projects:

- **eTeller.Api** - Web API layer (ASP.NET Core)
- **eTeller.Application** - Application services, handlers, validators, DTOs
- **eTeller.Domain** - Domain models, constants, interfaces
- **eTeller.Infrastructure** - EF Core DbContext, repositories, external services

## Build Commands

### Build Solution
```bash
dotnet build eTellerServer.sln
```

### Build Specific Project
```bash
dotnet build eTeller.Api/eTeller.Api.csproj
```

### Run Development Server
```bash
dotnet run --project eTeller.Api/eTeller.Api.csproj
```

The API runs on `http://localhost:5000` (or configured port). Swagger is available at `/swagger` in development mode.

## Test Commands

**Note:** There is currently no test project in the solution. To add tests:

```bash
# Create a test project
dotnet new xunit -n eTeller.UnitTests

# Add to solution
dotnet sln add eTeller.UnitTests/eTeller.UnitTests.csproj

# Add project reference
dotnet add eTeller.UnitTests/eTeller.UnitTests.csproj reference eTeller.Application/eTeller.Application.csproj

# Run all tests
dotnet test

# Run single test (example)
dotnet test --filter "FullyQualifiedName~Namespace.ClassName.MethodName"
```

## Code Style Guidelines

### Naming Conventions

- **Classes/Interfaces**: PascalCase (e.g., `UserRepository`, `IAuthenticationService`)
- **Methods**: PascalCase (e.g., `GetByIdAsync`, `LoginAsync`)
- **Properties**: PascalCase (e.g., `UsrId`, `UserName`)
- **Private fields**: `_camelCase` with underscore prefix (e.g., `_unitOfWork`, `_logger`)
- **Parameters**: camelCase (e.g., `request`, `userId`)
- **Constants**: PascalCase in static classes (e.g., `ErrorCodes.UserNotExists`)

### Architecture Pattern

The codebase follows a variation of **MediatR pattern** with CQRS-lite:

```
Controllers (API Layer)
    ↓
Handlers/CommandHandlers/QueryHandlers (Application Layer)
    ↓
Repositories (Infrastructure Layer)
    ↓
DbContext (Infrastructure Layer)
```

### Project Structure

```
eTeller.Application/Features/
    └── [FeatureName]/
        ├── Commands/
        │   ├── [Action]Command.cs
        │   └── [Action]CommandHandler.cs
        ├── Queries/
        │   ├── [Action]Query.cs
        │   └── [Action]QueryHandler.cs
        ├── DTOs/
        └── Mapping/

eTeller.Domain/Models/
    ├── [EntityName].cs
    ├── View/
    └── StoredProcedure/

eTeller.Infrastructure/Repositories/
    ├── [EntityName]Repository.cs
    └── StoreProcedures/
```

### Controller Guidelines

- Use `[ApiController]` attribute
- Use route pattern: `[Route("api/[controller]")]`
- Inject services via constructor
- Use `ActionResult<T>` return types
- Include XML documentation for endpoints
- Wrap business logic in try-catch with proper logging

Example:
```csharp
[ApiController]
[Route("api/[controller]")]
public class AuthenticationController : ControllerBase
{
    private readonly IAuthenticationService _authenticationService;
    private readonly ILogger<AuthenticationController> _logger;

    public AuthenticationController(
        IAuthenticationService authenticationService,
        ILogger<AuthenticationController> logger)
    {
        _authenticationService = authenticationService;
        _logger = logger;
    }

    /// <summary>
    /// Authenticate user with credentials
    /// </summary>
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<LoginResponseDto>> Login([FromBody] LoginRequestDto request)
    {
        try
        {
            // business logic
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during login");
            return StatusCode(500, new LoginResponseDto { IsSuccessful = false });
        }
    }
}
```

### Repository Pattern

- Create repository interfaces in `eTeller.Application/Contracts/`
- Implement in `eTeller.Infrastructure/Repositories/`
- Use `BaseSimpleRepository<T>` for basic CRUD
- Use Dapper for stored procedure queries (via `Sp` repositories)
- Follow naming: `IUserRepository` → `UserRepository`

Example:
```csharp
public interface IUserRepository
{
    Task<User?> GetByIdAsync(string usrId);
    Task<bool> Exists(string usrId);
}

public class UserRepository : BaseSimpleRepository<User>, IUserRepository
{
    public UserRepository(eTellerDbContext dbContext) : base(dbContext) { }

    public async Task<User?> GetByIdAsync(string usrId)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.UsrId == usrId);
    }
}
```

### Domain Models

- Use EF Core attributes: `[Table("TABLE_NAME")]`, `[Column("COLUMN_NAME")]`
- Use nullable reference types (`string?`) for optional fields
- Required properties use `required` keyword or `[Required]` attribute

Example:
```csharp
[Table("sys_USERS")]
public class User
{
    [Column("USR_ID")]
    public required string UsrId { get; set; }

    [Column("USR_PASS")]
    public required string UsrPass { get; set; }

    [Column("USR_MAIL")]
    public string? UsrMail { get; set; }
}
```

### DTOs

- Use suffix `Dto`, `RequestDto`, `ResponseDto`
- Use `record` or `class` based on immutability needs
- Include `IsSuccessful` and `ResultCode` properties for responses

### Validation

- Use **FluentValidation** for request validation
- Place validators in `eTeller.Application/Validators/`
- Use error codes matching legacy system (e.g., "1305", "9003")

Example:
```csharp
public class CaricaRequestValidator : AbstractValidator<PrelievoViewVm>
{
    public CaricaRequestValidator()
    {
        RuleFor(x => x.NumeroConto)
            .NotEmpty()
            .WithErrorCode("1305")
            .WithMessage("Il numero conto è obbligatorio.");
    }
}
```

### Error Handling

- Return structured responses with `IsSuccessful` flag
- Use string error codes (not numbers) for readability
- Log all errors with `_logger.LogError(ex, "context message")`
- Return appropriate HTTP status codes (200, 400, 401, 500)

### Constants

Place constants in `eTeller.Domain/Common/`:
- Static classes with `Constants` suffix
- Use const strings for values
- Group related constants (ErrorCodes, StatusConstants, etc.)

Example:
```csharp
public static class ErrorCodes
{
    public const string UserNotExists = "1320";
    public const string UserBlocked = "1306";
}
```

### Dependency Injection

- Register services in `eTeller.Infrastructure/InfrastructureServiceRegistration.cs`
- Use interface-based injection
- Scoped lifetime for repositories, Transient for handlers

### Logging

- Use `ILogger<T>` for structured logging
- Use log levels appropriately: `LogInformation`, `LogWarning`, `LogError`
- Include contextual information in log messages

### Async/Await

- Always use `async`/`await` for I/O operations
- Use `Task<T>` return types
- Do not block with `.Result` or `.Wait()`

### Nullable Reference Types

- Enabled via `<Nullable>enable</Nullable>` in csproj
- Use `?` for nullable types
- Use null checks or null-forgiving operator where appropriate

### Packages Used

- **MediatR** (14.0.0) - Request/command handling
- **FluentValidation** (12.1.1) - Input validation
- **FluentResults** (4.0.0) - Result handling
- **AutoMapper** (16.0.0) - Object mapping
- **Microsoft.EntityFrameworkCore.SqlServer** (10.0.2) - Database
- **Dapper** (2.1.15) - Raw SQL/stored procedures

### Database

- Uses SQL Server
- Entity Framework Core for entity management
- Dapper for high-performance stored procedure calls
- Connection string in `appsettings.json`

### XML Documentation

Include XML docs for:
- Public classes and interfaces
- Controller endpoints
- Important public methods

### Important Notes

1. **No tests currently exist** - add a test project when implementing new features
2. **Password handling** - currently uses plain text comparison (TODO: implement hashing)
3. **Legacy system compatibility** - some code maintains legacy error codes and patterns
4. **CORS** - configured for Angular (localhost:4200) and Vite (localhost:5253)
