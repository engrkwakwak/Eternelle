# CLAUDE.md — Eternelle

Modular monolith backend in **.NET 10**, Clean Architecture + full DDD.
Database design: `docs/database-design.md`.
Module conventions (authoritative): `docs/module-conventions.md`.

## Reference Implementations (Milan Jovanovic)

Packed repomix snapshots — read these when you need pattern guidance:

| File | Repo | Use for |
|------|------|---------|
| `D:\packed-references\bookify-clean-architecture.md` | Bookify | Clean Architecture layers, repository pattern, domain events, value objects |
| `D:\packed-references\evently-modular-monolith.md` | Evently | Module boundaries, integration events, inbox/outbox, cross-module data flow |

**How to use in a prompt:**
> "Read `D:\packed-references\evently-modular-monolith.md` and show me how [specific pattern] is implemented."

- Architecture decisions (layers, aggregates, value objects) → read **bookify-clean-architecture**
- Module design (boundaries, integration events, module registration) → read **evently-modular-monolith**

---

## Project Layout

```text
src/
├── API/Eternelle.Api/              ← Host (Program.cs, config, middleware)
├── Common/                         ← Shared kernel (Domain, Application, Infrastructure, Presentation)
└── Modules/
    └── Weddings/
        ├── Eternelle.Modules.Weddings.Domain/
        ├── Eternelle.Modules.Weddings.Application/
        ├── Eternelle.Modules.Weddings.Infrastructure/
        ├── Eternelle.Modules.Weddings.Presentation/
        └── Eternelle.Modules.Weddings.IntegrationEvents/   ← integration event classes only
```

Future modules follow the same **five-project** structure: **Identity → Tenancy → RSVP → Guestbook → Catalog**.

---

## Module Status

| Module    | Domain | Application | Infrastructure | Presentation | IntegrationEvents |
|-----------|--------|-------------|----------------|--------------|-------------------|
| Weddings  | ✅      | ✅           | ✅              | ✅            | ✅                 |
| Identity  | ❌      | ❌           | ❌              | ❌            | ❌                 |
| Tenancy   | ❌      | ❌           | ❌              | ❌            | ❌                 |
| RSVP      | ❌      | ❌           | ❌              | ❌            | ❌                 |
| Guestbook | ❌      | ❌           | ❌              | ❌            | ❌                 |
| Catalog   | ❌      | ❌           | ❌              | ❌            | ❌                 |

---

## Cross-Module Rules

- **Only `Eternelle.Modules.{X}.IntegrationEvents`** may be referenced from outside module X.
  No module may reference another module's Domain or Application project.
- **Synchronous reads** use local data copies — each module duplicates the fields it needs in its own schema, kept consistent via integration events. No synchronous cross-module service calls.
- **Asynchronous communication** uses the Outbox/Inbox pipeline with MassTransit. Domain event → `DomainEventHandler` publishes `IntegrationEvent` directly from event payload via `IEventBus` → other module's Presentation subscribes via `IntegrationEventHandler<T>`.

---

## Architecture Conventions

### Domain Layer

**Aggregate roots** extend `Eternelle.Common.Domain.Entity`.

```csharp
// Strongly-typed ID — always a readonly record struct
public readonly record struct WeddingId(Guid Value)
{
    public static WeddingId New() => new(Guid.NewGuid());
    public static WeddingId Empty => new(Guid.Empty);
}

// Aggregate root
public sealed class Wedding : Entity
{
    private Wedding() { }  // EF constructor

    public static Wedding Create(...) { ... Raise(new WeddingCreatedDomainEvent(id)); }
}
```

- Folder + namespace per aggregate root: `Weddings/Wedding.cs` → `namespace Eternelle.Modules.Weddings.Domain.Weddings`
- Child entities use `internal static Create(...)` — only callable from their aggregate root
- Cross-module references are plain `Guid`, **never** value objects

**Domain events** use primary constructor syntax (Evently pattern):

```csharp
public sealed class WeddingCreatedDomainEvent(WeddingId weddingId) : DomainEvent
{
    public WeddingId WeddingId { get; init; } = weddingId;
}
```

**Value objects** — every domain-meaningful string is a sealed record VO:

```csharp
public sealed record PersonName
{
    public const int MaxLength = 150;   // const, not static readonly — monorepo, source refs only

    private PersonName(string value) { Value = value; }

    public string Value { get; }

    public static Result<PersonName> Create(string? raw)
    {
        if (string.IsNullOrWhiteSpace(raw)) return Result.Failure<PersonName>(PersonNameErrors.Empty);
        string trimmed = raw.Trim();
        if (trimmed.Length > MaxLength) return Result.Failure<PersonName>(PersonNameErrors.TooLong);
        return Result.Success(new PersonName(trimmed));
    }

    internal static PersonName FromPersistence(string value) => new(value);   // trusts DB, skips validation
}
```

- `Create()` is the only public construction path — all validation lives here.
- `FromPersistence()` is `internal` (called by EF Core value converters across the `InternalsVisibleTo` boundary).
- Every VO has a sibling `{Vo}Errors` static class.
- `const int MaxLength` is correct for this monorepo (all projects compile together from source). `static readonly` is only needed when shipping VO types as binary NuGet packages consumed by independently-deployed services.

**Repository interface** defined in domain: `I{Aggregate}Repository`.

**Errors:**

```csharp
public static class WeddingErrors
{
    public static Error NotFound(WeddingId id) =>
        Error.NotFound("Weddings.NotFound", $"Wedding '{id.Value}' was not found.");

    public static readonly Error SnapShareNotConfigured =
        Error.Problem("Weddings.SnapShareNotConfigured", "SnapShare is not configured.");
}
```

Use `Error.NotFound` / `Error.Problem` / `Error.Conflict` / `Error.Failure` from `Eternelle.Common.Domain`.

### Application Layer

**Commands and queries** implement `ICommand`, `ICommand<T>`, or `IQuery<T>` from `Eternelle.Common.Application.Messaging`.

**Every command parameter that identifies a resource must include a `WeddingId`** — required for ownership checks in the handler.

**Handlers** return `Result` or `Result<T>`. Never throw for business rule violations.

**Validators** use FluentValidation. Every ID parameter needs `RuleFor(c => c.TheId).NotEmpty()`. Every string field that maps to a VO needs `NotEmpty()` (required) or `.MaximumLength(Vo.MaxLength).When(c => c.Field is not null)` (optional) — validators reference the VO constant as the single source of truth for the limit.

```csharp
RuleFor(c => c.FirstName)
    .NotEmpty()
    .MaximumLength(PersonFirstName.MaxLength);   // early HTTP rejection; domain VO owns enforcement

RuleFor(c => c.Bio)
    .MaximumLength(Biography.MaxLength)
    .When(c => c.Bio is not null);
```

**IDOR guard pattern — load by child ID:**

```csharp
if (group is null
    || group.WeddingId != new WeddingId(command.WeddingId)
    || group.Id != new EntourageGroupId(command.EntourageGroupId))
{
    return Result.Failure(EntourageGroupErrors.MemberNotFound(memberId));
}
```

**IDOR guard pattern — load by parent ID:**

```csharp
if (group is null || group.WeddingId != new WeddingId(command.WeddingId))
{
    return Result.Failure(EntourageGroupErrors.NotFound(groupId));
}
```

Always return an **opaque** `NotFound` error — never leak existence.

**Domain event handlers** embed all integration-event payload in the domain event at raise-time, then publish directly — no repository reload:

```csharp
// Domain event carries creation-time data
public sealed class WeddingCreatedDomainEvent(
    WeddingId weddingId,
    Guid tenantId,
    DateOnly weddingDate) : DomainEvent
{
    public WeddingId WeddingId { get; init; } = weddingId;
    public Guid TenantId { get; init; } = tenantId;
    public DateOnly WeddingDate { get; init; } = weddingDate;
}

// Handler injects only IEventBus — no repository
internal sealed class WeddingCreatedDomainEventHandler(IEventBus eventBus)
    : DomainEventHandler<WeddingCreatedDomainEvent>
{
    public override async Task Handle(WeddingCreatedDomainEvent domainEvent, CancellationToken ct = default)
    {
        await eventBus.PublishAsync(
            new WeddingCreatedIntegrationEvent(domainEvent.Id, domainEvent.OccurredOnUtc,
                domainEvent.WeddingId.Value, domainEvent.TenantId, domainEvent.WeddingDate),
            ct);
    }
}
```

### Infrastructure Layer

- Repositories implement the domain interface and live under `Eternelle.Modules.Weddings.Infrastructure/{AggregatePluralName}/`
- EF entity configurations live alongside repositories
- EF value converters: `HasConversion(v => v.Value, v => Vo.FromPersistence(v))` + `HasMaxLength(Vo.MaxLength)`
- Module registered via `WeddingsModule.AddWeddingsModule(services, configuration)`

**Redis / Upload slot store fallback:**
`IConnectionMultiplexer` is only registered when Redis is reachable. `WeddingsModule` conditionally registers `RedisUploadSlotStore` (Redis available) or `StubUploadSlotStore` (in-memory `ConcurrentDictionary`, dev only) so the app starts without Redis locally. Check pattern:
```csharp
if (services.Any(d => d.ServiceType == typeof(IConnectionMultiplexer)))
    services.AddScoped<IUploadSlotStore, RedisUploadSlotStore>();
else
    services.AddSingleton<IUploadSlotStore, StubUploadSlotStore>();
```

**Concurrent insert serialization** — when enforcing a plan cap with `INSERT + enforce`, acquire a row-level lock first:
```csharp
await context.Database.ExecuteSqlAsync(
    $"SELECT 1 FROM wedding.profiles WHERE id = {weddingId.Value} FOR UPDATE", ct);
```

### Presentation Layer

Minimal-API endpoints implement `IEndpoint` and are grouped by aggregate under `{AggregatePluralName}/`:

```csharp
internal sealed class CreateWeddingEndpoint : IEndpoint
{
    internal sealed record Request(...);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("weddings/{weddingId}/...", async (Guid weddingId, Request request, ISender sender, CancellationToken ct) =>
        {
            var command = new CreateWeddingCommand(weddingId, ...);
            Result<Guid> result = await sender.Send(command, ct);
            return result.Match(
                id => Results.Created($"/weddings/{weddingId}/.../{id}", new { id }),
                ApiResults.Problem);
        })
        .WithTags(Tags.Weddings)
        .RequireAuthorization();
    }
}
```

- Tags are defined in `Tags.cs` — add a constant per aggregate
- Route parent IDs (`{weddingId}`, `{groupId}`, etc.) must be passed through to the command — never drop them
- Use `Results.Ok` for operations that return data without creating a resource; `Results.Created` only when creating a new resource with a `Location` header
- Public endpoints (e.g. SnapShare guest upload) use `.AllowAnonymous()` — pair with `.RequireRateLimiting()`

**Integration event handlers** live in Presentation and are registered via `AddIntegrationEventHandlers()` scanning the Presentation assembly.

---

## Next Module: Identity

Module name: **`Eternelle.Modules.Identity`** — not IAM.  
Schema constant: `public static class Schemas { public const string Identity = "identity"; }`

Rationale: matches `.NET` ecosystem convention (ASP.NET Core Identity), correct scope (auth + user profile), avoids acronym in namespace, already referenced as the example in `docs/module-conventions.md`.

Cross-module data flow: `WeddingCreatedIntegrationEvent` → Identity handler stores a local copy of `WeddingId`/`TenantId`/`WeddingDate` in the `identity` schema. Identity never queries the `wedding` schema directly.

---

## Config & Settings

Module-specific config lives in `src/API/Eternelle.Api/`:
- `modules.weddings.json` — base config (non-environment-specific defaults only; do NOT put empty strings for validated fields)
- `modules.weddings.Development.json` — local dev overrides (MinIO credentials, dev URLs)
- Environment-specific files for other environments are managed separately

`WeddingsModule.cs` validates `SnapShareOptions` and `PhotoStorageOptions` at startup via `ValidateOnStart`. All five `PhotoStorageOptions` fields and `SnapShare.UploadBaseUrl` are required non-empty strings — secrets (AccessKey, SecretKey) come from environment variables in non-dev environments (`Weddings__PhotoStorage__AccessKey`).

---

## Domain Invariants Worth Knowing

- **OverLimit photos** are NOT deleted — they are marked `GuestPhotoStatus.OverLimit` by `EnforcePhotoLimitAsync`. Pre-upload quota checks must use `CountActiveByWeddingIdAsync` (excludes OverLimit) not `CountByWeddingIdAsync` (counts all).
- **Upload slots** are Redis-backed, 15-minute TTL, single-use (`GETDEL` for single, Lua script for batch). Batch slot redemption is all-or-nothing — if any slot is missing, none are consumed. Falls back to `StubUploadSlotStore` (in-memory) when Redis is unavailable.
- **Duplicate slot IDs** in a batch must be rejected before calling `RedeemManyAsync` — a duplicate maps two GuestPhoto records to one CDN upload.
- **SnapShare** must be configured before `RegisterGuestPhotos` can proceed — check explicitly even though `GetByUploadTokenAsync` implies it.
- **Photo plan limits** (Free: 50, Pro: 250, Plus: unlimited) are stored as a `WeddingPlan` enum on `wedding.profiles`. `WeddingPlanLimits.PhotoLimit(plan)` returns `null` for unlimited.

---

## Workflow Rules

### Before Writing Code

1. Read the finding against current code first — changes from earlier in the session may have already resolved it
2. For any file you are about to edit, read it first — never patch from memory
3. If touching a handler, check the corresponding validator has a `RuleFor` for every ID parameter

### Making Changes

- **Minimal diff** — fix exactly what is broken; do not refactor surrounding code
- **One concern per edit** — don't mix a bug fix with a rename
- Every new `ICommand`/`IQuery` parameter that identifies a resource needs: (a) the command property, (b) a validator rule, (c) the endpoint passing it through, (d) the handler ownership check
- When adding a repository method to an interface, implement it in the concrete class before compiling

### Validating Changes

- Read back every edited file after a non-trivial change
- For config changes: trace the full load path (`AddModuleConfiguration` → `ValidateOnStart`) to confirm the value reaches the options class
- For IDOR fixes: confirm the guard returns an opaque `NotFound`, not a permission-specific error

### Common Mistakes to Avoid

- **Dropping route IDs** — binding `{weddingId}` from the route but not passing it to the command is the most common IDOR mistake. Every route segment that is also a command parameter must be wired through.
- **Counting all photos for quota checks** — always use `CountActiveByWeddingIdAsync`, not `CountByWeddingIdAsync`
- **Null-forgiving on SnapShare** — `wedding.SnapShare!` hides a real invariant. Use an explicit null guard and return `WeddingErrors.SnapShareNotConfigured`.
- **`Results.Created` on batch operations** — batch endpoints that return IDs but don't create a single addressable resource should return `Results.Ok`, not `Results.Created` with a self-referential `Location` header.
- **Empty validator** — a command with new ID fields and no corresponding `RuleFor` rules will silently accept zero GUIDs.
- **Config empty strings** — empty strings in the base `modules.weddings.json` are not "unset" — they are values that fail `IsNullOrWhiteSpace` validation at startup. Remove keys that must be environment-provided entirely from the base file.
- **Repository reload in domain event handler** — don't inject a repository into a domain event handler to reload the aggregate. Embed all integration-event payload in the domain event at raise-time and publish directly from `domainEvent` properties.
- **`static readonly` for VO MaxLength** — use `const int MaxLength` in all VOs. `static readonly` is only justified when shipping domain types as binary NuGet packages to independently-deployed consumers, which does not apply to this monorepo.
