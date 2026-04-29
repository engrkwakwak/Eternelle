# Contributing to Eternelle

This document describes the architecture rules, coding conventions, and patterns used in this codebase. It is the primary reference for both human contributors and AI tools (CodeRabbit, Copilot, etc.) reviewing or generating code.

---

## Layer rules

Each module has four layers. The dependency rule is strict and enforced by project references.

### Domain

**Zero dependencies on Application, Infrastructure, or any framework** (no EF Core, MediatR, Dapper, ASP.NET).

- Aggregates expose behaviour through methods only — no public setters outside the aggregate root.
- Owned entities (e.g. `EntourageMember`, `Partner`) are created and mutated exclusively through their aggregate root. Their factory and mutation methods are `internal`.
- Value objects are `sealed record` or `sealed class` with a private constructor and a `static Create()` factory returning `Result<T>`.
- String length limits are defined as `public static readonly int` constants on the owning type, not as magic numbers:
  ```csharp
  // On EntourageMember:
  public static readonly int MaxNameLength = 150;
  public static readonly int MaxRoleLength = 100;
  ```
- Domain errors use the `Error` type from `Eternelle.Common.Domain`:
  ```csharp
  public static Error NotFound(WeddingId id) =>
      Error.NotFound("Weddings.NotFound", $"Wedding {id.Value} was not found");
  ```
- Domain events are raised via `Entity.Raise()` and are plain records implementing `IDomainEvent`.

### Application

**No references to Infrastructure or EF Core.**

- Commands implement `ICommand` (void) or `ICommand<TResponse>`.
- Queries implement `IQuery<TResponse>`.
- All handlers are `internal sealed class`.
- Every command must have a matching `AbstractValidator<TCommand>`. `MaximumLength` rules **must** reference domain constants:
  ```csharp
  RuleFor(c => c.Name)
      .NotEmpty()
      .MaximumLength(EntourageMember.MaxNameLength);
  ```
- Nullable fields are validated conditionally:
  ```csharp
  RuleFor(c => c.Subtitle)
      .MaximumLength(EntourageGroup.MaxSubtitleLength)
      .When(c => c.Subtitle is not null);
  ```
- **Query handlers use Dapper** via `IDbConnectionFactory` with `CommandDefinition` (always pass `cancellationToken`). Never use EF Core in a query handler.
- **Command handlers use domain repository interfaces** (`IWeddingRepository`, etc.) and `IUnitOfWork`. Call `entourageGroupRepository.Update(group)` after mutating owned entities, then `await unitOfWork.SaveChangesAsync(cancellationToken)`.
- Reorder handlers **must** validate list completeness before iterating using a three-condition HashSet check:
  ```csharp
  HashSet<Guid> providedIds = [.. command.StoryMomentIds];
  if (command.StoryMomentIds.Count != providedIds.Count ||   // duplicate check
      providedIds.Count != storyMoments.Count ||             // count mismatch
      !providedIds.SetEquals(storyMomentsById.Keys))         // wrong IDs
  {
      return Result.Failure(StoryMomentErrors.ReorderListMismatch());
  }
  ```

### Infrastructure

- EF Core entity configurations go in `IEntityTypeConfiguration<T>` classes, one per aggregate root.
- `UseSnakeCaseNamingConvention()` is active. Use explicit `HasColumnName()` only when the snake_case default is wrong.
- All strongly-typed ID value objects must have a `ValueConverter` registered globally in `ConfigureConventions`, not per-property:
  ```csharp
  builder.Properties<WeddingId>().HaveConversion<WeddingIdConverter>();
  ```
- String length constants from the domain must be referenced in `HasMaxLength()` — no magic numbers:
  ```csharp
  .HasMaxLength(EntourageMember.MaxNameLength)
  ```
- Repositories are `internal sealed class` implementing domain interfaces.
- All tables live in the `wedding` schema. Declare it via `modelBuilder.HasDefaultSchema(Schemas.Weddings)`.

### Presentation

- Endpoints implement `IEndpoint` and are discovered by `MapEndpoints()`.
- Endpoints must not contain business logic — map HTTP input to commands/queries and dispatch via `ISender`.
- Use the `TypedResults` / `Results` pattern for HTTP responses.
- No direct references to domain types in endpoint request/response models.

---

## Result pattern

All domain operations and command/query handlers return `Result` or `Result<T>` (never throw for expected failures).

```csharp
// Returning a failure
return Result.Failure<Guid>(WeddingErrors.NotFound(id));

// Returning a success value
return wedding.Id.Value;          // implicit conversion to Result<Guid>

// Checking in a handler
if (result.IsFailure) return result;
```

`ResultExtensions` in the Presentation layer maps `Result` to the correct HTTP status code via `ToResponse()`.

---

## ID types

Every aggregate and owned entity uses a strongly-typed ID record:

```csharp
public record WeddingId(Guid Value)
{
    public static WeddingId New() => new(Uuid7.NewUuid7().ToGuid());
}
```

IDs use **UUIDv7** (time-ordered) via `SourceFlow.Stores.EntityFramework`. This keeps database index locality while remaining globally unique.

---

## Domain events — Outbox pattern

1. Aggregate raises event: `Raise(new WeddingCreatedDomainEvent(Id, TenantId))`.
2. `InsertOutboxMessagesInterceptor` serialises domain events to `wedding.outbox_messages` inside the same `SaveChanges` transaction.
3. `ProcessOutboxJob` (Quartz, configurable interval) polls and dispatches to MediatR domain event handlers.
4. `IdempotentDomainEventHandler<T>` wraps every handler to prevent duplicate processing.

---

## Adding a new feature

### New command (e.g. `CreateFoo`)

1. **Domain** — ensure the aggregate root has the corresponding method and any required error constants.
2. **Application/Foos/CreateFoo/**
   - `CreateFooCommand.cs` — `public sealed record CreateFooCommand(...) : ICommand<Guid>;`
   - `CreateFooCommandValidator.cs` — `internal sealed class` + `AbstractValidator<CreateFooCommand>`. Reference domain constants for `MaximumLength`.
   - `CreateFooCommandHandler.cs` — `internal sealed class`. Load aggregate, call method, persist, return.
3. **Presentation** — add `CreateFooEndpoint.cs` implementing `IEndpoint`. Map to `POST /foos`.
4. **Infrastructure** — wire EF config and repository in a dedicated infrastructure issue.

### New query (e.g. `GetFoos`)

1. **Application/Foos/GetFoos/**
   - `GetFoosQuery.cs` — `public sealed record GetFoosQuery(...) : IQuery<IReadOnlyList<FooResponse>>;`
   - `FooResponse.cs` — `public sealed record FooResponse(...)` with `nameof()`-friendly property names that match SQL column aliases.
   - `GetFoosQueryHandler.cs` — `internal sealed class`. Use `IDbConnectionFactory`, build SQL with `nameof()` aliases, use `CommandDefinition` with `cancellationToken`.
2. **Presentation** — add endpoint.

### New aggregate

1. In `*.Domain/{AggregateName}/`:
   - Aggregate root (inherits `Entity`)
   - Strongly-typed ID record (`{Name}Id`)
   - Error class (`{Name}Errors`) with `NotFound`, etc.
   - Domain event(s) (`{Name}CreatedDomainEvent`)
   - Repository interface (`I{Name}Repository`)
2. Follow layer rules above for Application and Infrastructure.
3. Add an EF Core migration after infrastructure is wired.

---

## EF Core migrations

Migrations use the PMC (Package Manager Console) or `dotnet ef` CLI.

**PMC (recommended for Visual Studio):**

```powershell
# Set Default project dropdown to: src/Modules/Weddings/Eternelle.Modules.Weddings.Infrastructure
Add-Migration MigrationName -StartupProject src/API/Eternelle.Api
```

**CLI:**

```bash
dotnet ef migrations add MigrationName \
  --project src/Modules/Weddings/Eternelle.Modules.Weddings.Infrastructure \
  --startup-project src/API/Eternelle.Api
```

**Remove last migration (not yet applied):**

```powershell
Remove-Migration -StartupProject src/API/Eternelle.Api
```

**Preflight guards** — any migration that adds `NOT NULL` columns to a populated table or narrows a column type **must** include a `DO $$ ... RAISE EXCEPTION` guard before the destructive `AlterColumn` call to abort the migration if existing rows would violate the new constraint:

```csharp
migrationBuilder.Sql("""
    DO $
    DECLARE violation_count INTEGER;
    BEGIN
        SELECT COUNT(*) INTO violation_count
        FROM wedding.some_table
        WHERE length(some_column) > 100;
        IF violation_count > 0 THEN
            RAISE EXCEPTION 'Migration blocked: % row(s) violate the new constraint', violation_count;
        END IF;
    END $;
    """);
```

---

## Coding conventions

- All handler classes are `internal sealed`.
- Command and query records are `public sealed record`.
- Endpoint classes are `internal sealed` implementing `IEndpoint`.
- String constants on domain types use `public static readonly int`, not `const int`, because `const` fields are inlined at compile time and can cause issues across assembly boundaries.
- Nullable reference types are enabled project-wide. Never suppress the nullable warning without a comment explaining why.
- `using` directives are auto-managed by the IDE (file-scoped namespaces, no redundant usings).
- Diagnostic suppressions (`NoWarn`, `#pragma warning disable`) go in `.editorconfig`, not in `.csproj` files or inline in source.

---

## Observability

- **Structured logging:** Serilog with Seq sink (local: `http://127.0.0.1:8081`). Request logging via `UseSerilogRequestLogging()`. Trace correlation added by `LogContextTraceLoggingMiddleware`.
- **Distributed tracing:** OpenTelemetry with OTLP exporter to Jaeger (local: `http://localhost:16686`). Instrumented: ASP.NET Core, HttpClient, Npgsql.
- **Health checks:** `/health` endpoint covers PostgreSQL and Redis. Consumed by Docker Compose `depends_on` conditions.
