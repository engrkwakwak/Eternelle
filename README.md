# Eternelle

Backend for **Eternelle** — a Philippine wedding platform that lets couples build a personalised wedding website. Built as a **.NET 10 modular monolith** following **Clean Architecture** and **Domain-Driven Design**.

---

## Architecture

```
┌────────────────────────────────────────────────────────────────┐
│  Eternelle.Api  (ASP.NET Core 10 — entry point)                │
│  Program.cs wires modules, middleware, health checks, OpenAPI  │
└───────────────────────┬────────────────────────────────────────┘
                        │ references only Infrastructure
          ┌─────────────▼─────────────┐
          │   Modules/Weddings        │  (one module today — more planned)
          │                           │
          │  .Presentation            │  Minimal API endpoints (IEndpoint)
          │       │                   │
          │  .Application             │  Commands, queries, validators (MediatR)
          │       │                   │
          │  .Infrastructure          │  EF Core, Dapper, repositories, jobs
          │       │                   │
          │  .Domain                  │  Aggregates, value objects, domain events
          └───────────────────────────┘
          ┌───────────────────────────┐
          │   Common/*                │  Shared abstractions (no business logic)
          │  .Domain                  │  Entity, Result, Error, IDomainEvent
          │  .Application             │  ICommand, IQuery, behaviors, IUnitOfWork
          │  .Infrastructure          │  Auth, caching, outbox/inbox, event bus
          │  .Presentation            │  IEndpoint, ResultExtensions
          └───────────────────────────┘
```

**Dependency rule:** Domain has no outward dependencies. Application depends only on Domain and Common abstractions. Infrastructure implements Application interfaces. Presentation dispatches to Application via MediatR `ISender`. The API project references only Infrastructure.

---

## Weddings module — domain model

The Weddings module owns the following aggregate roots:

| Aggregate | Owned entities | Notes |
|---|---|---|
| `Wedding` | `Partner` (≤ 2), `SnapShareConfig` | Core identity — tenanted by `TenantId` |
| `StoryMoment` | — | Ordered timeline entries |
| `EntourageGroup` | `EntourageMember`, `EntourageCouple` | Couple pairings enforced by group type |
| `DressCodeConfig` | `DressCodeColor`, `DressCodeImage` | Palette + reference images |
| `CeremonyAct` | — | Program-of-events entries |
| `GalleryImage` | — | Photo gallery |
| `GiftOption` | — | Gift registry / monetary options |
| `Reminder` | — | Automated reminders |
| `VendorCredit` | — | Vendor attribution |

---

## Tech stack

| Concern | Library / tool |
|---|---|
| Runtime | .NET 10, ASP.NET Core 10 |
| ORM (writes) | EF Core 10 + Npgsql (PostgreSQL 17) |
| Queries | Dapper 2 via `IDbConnectionFactory` |
| Messaging | MediatR 14 (CQRS), MassTransit 9 (integration events) |
| Background jobs | Quartz.NET 3 (outbox/inbox processors) |
| Validation | FluentValidation 12 |
| Auth | Keycloak (JWT Bearer) |
| Caching | Redis via `IDistributedCache` / MassTransit.Redis |
| Observability | Serilog → Seq, OpenTelemetry → Jaeger |
| API docs | Scalar (OpenAPI) |
| Health checks | `AspNetCore.HealthChecks` (Postgres + Redis) |

---

## Local development

### Prerequisites

- .NET 10 SDK
- Docker Desktop

### Start infrastructure

```bash
docker compose up -d
```

This starts:

| Container | Purpose | Port |
|---|---|---|
| `Eternelle.Database` | PostgreSQL 17 | 5432 |
| `Eternelle.Redis` | Redis (cache + MassTransit saga store) | 6379 |
| `Eternelle.Seq` | Structured log viewer | 127.0.0.1:8081 |
| `Eternelle.Jaeger` | Distributed traces | 16686 (UI), 4317/4318 (OTLP) |

The API container is defined in `docker-compose.yml` but is typically run from the IDE or CLI rather than Docker during development.

### Run the API

```bash
cd src/API/Eternelle.Api
dotnet run
```

Migrations are applied automatically on startup in the `Development` environment (`ApplyMigrations()` in `Program.cs`).

The Scalar API reference is available at `https://localhost:{port}/scalar/v1`.

### Connection strings

Default values for local dev live in `appsettings.Development.json` and `modules.weddings.Development.json`. Override via user secrets or environment variables in production.

---

## Project structure

```
src/
  API/
    Eternelle.Api/               # Entry point — no business logic
  Common/
    Eternelle.Common.Domain/     # Entity, Result<T>, Error, IDomainEvent
    Eternelle.Common.Application/# ICommand, IQuery, IUnitOfWork, behaviors
    Eternelle.Common.Infrastructure/ # Auth, caching, outbox, inbox, event bus
    Eternelle.Common.Presentation/   # IEndpoint, ResultExtensions
  Modules/
    Weddings/
      *.Domain/                  # Aggregates, value objects, domain errors
      *.Application/             # Commands, queries, validators, handlers
      *.Infrastructure/          # EF Core config, repositories, Quartz jobs
      *.Presentation/            # Minimal API endpoints
```

Module-level configuration files (`modules.weddings.json`) are loaded by `AddModuleConfiguration()` and keep per-module settings out of the root `appsettings.json`.

---

## Domain events and integration events

**Domain events** are raised inside aggregates via `Entity.Raise()` and persisted to an outbox table (`wedding.outbox_messages`) by `InsertOutboxMessagesInterceptor` as part of the same EF Core `SaveChanges` transaction. A Quartz job (`ProcessOutboxJob`) polls and dispatches them.

**Integration events** cross module boundaries via MassTransit and are consumed through an inbox (`ProcessInboxJob`) with idempotency tracking.

---

## Adding a new module

1. Create four projects: `*.Domain`, `*.Application`, `*.Infrastructure`, `*.Presentation`.
2. Wire the module in `Program.cs` via an `Add{Module}Module(builder.Configuration)` extension.
3. Register a `IDesignTimeDbContextFactory` in Infrastructure for EF Core tooling.
4. Add health checks for any new external dependencies.

See `CONTRIBUTING.md` for layer-by-layer rules.
