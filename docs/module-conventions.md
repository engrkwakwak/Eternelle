# Module Conventions

This document describes the conventions for all modules in Eternelle. Follow it when scaffolding a new module (e.g. `Eternelle.Modules.Identity`).

---

## 1. Project layout

A module is composed of five projects:

```text
src/Modules/{Name}/
  Eternelle.Modules.{Name}.Domain/           — Entities, VOs, domain events, repository interfaces
  Eternelle.Modules.{Name}.Application/      — Commands, queries, handlers, validators, domain event handlers
  Eternelle.Modules.{Name}.Infrastructure/   — EF Core, repositories, migrations, external clients
  Eternelle.Modules.{Name}.Presentation/     — Minimal API endpoints (IEndpoint), DI registration
  Eternelle.Modules.{Name}.IntegrationEvents/ — Integration event classes only; no logic
```

---

## 2. ProjectReference graph (hard rule)

| Project | May reference |
|---|---|
| Domain | *(nothing)* |
| Application | Domain, Common.Application, own IntegrationEvents |
| Infrastructure | Application, Common.Infrastructure |
| Presentation | Application, Common.Presentation |
| IntegrationEvents | Common.Application only |

Violating this graph creates circular dependencies and leaks module internals.

---

## 3. Cross-module rule

**Only `Eternelle.Modules.{X}.IntegrationEvents` may be referenced from outside module X.**

Integration event flow (Pattern B — load in handler):

```text
Domain event raised in aggregate
  → DomainEventHandler<TEvent> (in Application)
      → loads aggregate from repository to get full state
      → publishes IntegrationEvent via IEventBus
          → other module's Presentation subscribes via IntegrationEventHandler<TEvent>
```

No module may reference another module's Domain or Application project.

---

## 4. Naming

| Artifact | Convention |
|---|---|
| Command | `{Verb}{Noun}Command` + `Handler` + `Validator` in one folder per use-case |
| Query | `Get{Noun}Query` + `Handler` + response DTO |
| Domain event | `{Aggregate}{Verb}DomainEvent` |
| Integration event | `{Aggregate}{Verb}IntegrationEvent` |
| Domain errors | `{Aggregate}Errors` static class returning `Error` (from `Common.Domain`) |
| Repository interface | `I{Aggregate}Repository` in Domain |

---

## 5. Module registration template

Each module exposes one extension method in Presentation:

```csharp
public static IServiceCollection AddWeddingsModule(
    this IServiceCollection services,
    IConfiguration configuration)
{
    services.AddDomainEventHandlers();        // scan Application assembly
    services.AddIntegrationEventHandlers();   // scan Presentation assembly
    services.AddInfrastructure(configuration);
    services.AddEndpoints();                  // scan Presentation assembly
    return services;
}
```

Register in `src/API/Eternelle.Api/Program.cs`:

```csharp
builder.Services.AddWeddingsModule(builder.Configuration);
```

Each module owns a static `Schemas.{Module}` string constant (`"wedding"`, `"identity"`, …) used as the PostgreSQL schema prefix.

---

## 6. Value object discipline

Every domain-meaningful string is a sealed record VO:

```csharp
public sealed record PersonName
{
    public static readonly int MaxLength = 150;

    private PersonName(string value) { Value = value; }

    public string Value { get; }

    public static Result<PersonName> Create(string? raw) { /* trim, validate */ }

    internal static PersonName FromPersistence(string value) => new(value);
}
```

Rules:
- `Create()` is the only public construction path; it returns `Result<T>` and performs all validation.
- `FromPersistence()` is `internal` (called by EF Core value converters across the `InternalsVisibleTo` boundary) — it trusts the DB and skips validation.
- Every VO has a sibling `{Vo}Errors` class with `Error` constants for each failure mode.
- Aggregate methods accept VOs, not raw strings. Command handlers call `Create()` and short-circuit on failure before invoking the aggregate.
- EF Core value converters use `HasConversion(v => v.Value, v => Vo.FromPersistence(v))` + `HasMaxLength(Vo.MaxLength)`. Nullable pattern: `(v => v != null ? v.Value : null, v => v != null ? Vo.FromPersistence(v) : null)`.
- FluentValidation validators keep only `NotEmpty()` (fast HTTP-layer rejection); domain VOs own all length and format rules.

Shared VOs that are likely to be needed by multiple modules start in the module's `Shared/` folder and are lifted to `Common.Domain` when a second module actually needs them.
