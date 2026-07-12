# Loyeris Backend — Agent instructions

## Scope

These instructions apply to everything under `backend/`.

The backend is a .NET 10 solution using the `.slnx` format, ASP.NET Core Minimal API, MediatR for CQRS-style request handling, EF Core, and PostgreSQL.

## Projects

The solution is organized by domains. Each domain follows a `Core / App / Infrastructure` trio.

| Project | Layer |
|---|---|
| `Loyeris.Api/` | Minimal API host, endpoint groups, OpenAPI setup |
| `Loyeris.IdentityAccess.Core/` | Domain entities/enums for users, workspaces, memberships, preferences, sessions, tokens, and auth events |
| `Loyeris.IdentityAccess.App/` | CQRS application layer for identity, workspace access, and authentication read/write use cases |
| `Loyeris.IdentityAccess.Infrastructure/` | EF Core persistence and DAL implementations for the `identity` schema |
| `Loyeris.Portfolio.Core/` | Domain entities/enums for SCI, associates, and lots |
| `Loyeris.Portfolio.App/` | CQRS application layer for portfolio use cases |
| `Loyeris.Portfolio.Infrastructure/` | EF Core persistence and DAL implementations for the `portfolio` schema |
| `Loyeris.Leasing.Core/` | Domain entities/enums for tenants, leases, and lease occupants |
| `Loyeris.Leasing.App/` | CQRS application layer for leasing use cases |
| `Loyeris.Leasing.Infrastructure/` | EF Core persistence and DAL implementations for the `leasing` schema |
| `Loyeris.RentCollection.Core/` | Domain entities/enums for rent deadlines, payments, and reminders |
| `Loyeris.RentCollection.App/` | CQRS application layer for rent collection use cases |
| `Loyeris.RentCollection.Infrastructure/` | EF Core persistence and DAL implementations for the `collection` schema |
| `Loyeris.TaxPreparation.Core/` | Domain entities/enums for fiscal periods and rental expenses |
| `Loyeris.TaxPreparation.App/` | CQRS application layer for tax preparation use cases |
| `Loyeris.TaxPreparation.Infrastructure/` | EF Core persistence and DAL implementations for the `tax` schema |
| `Loyeris.Messaging.Core/` | Domain entities/enums for notification preferences and outbox messages |
| `Loyeris.Messaging.App/` | CQRS application layer for messaging use cases |
| `Loyeris.Messaging.Infrastructure/` | EF Core persistence and DAL implementations for the `messaging` schema |
| `Loyeris.Shared/` | Shared kernel, including `Result<T>` and `Error` types |
| `Loyeris.Tests/` | NUnit tests with Moq and FluentAssertions |

Do not reintroduce the old `Loyeris.Auth.*` projects. Authentication belongs to the `IdentityAccess` domain.

Production projects target `net10.0` with `Nullable` disabled and `ImplicitUsings` enabled. `Loyeris.Tests` targets `net10.0`, has `Nullable` enabled, and uses `LangVersion latest`.

## Commands

Run backend commands from the specific project folder unless a task requires the solution root.

```bash
# from backend/Loyeris.Api/
dotnet build
dotnet run

# from backend/Loyeris.Tests/
dotnet test
```

The API runs on `http://localhost:5130` when launched from `Loyeris.Api/` with the development launch profile.

Connection strings and secrets are not stored in `appsettings.Development.json`. Native development must provide the required environment variables; the Compose stacks inject them from their environment and `.env` values.

## Containers and operational behavior

- `Dockerfile` is the multi-stage production build. It publishes `Loyeris.Api`, listens on internal port `8080`, includes `curl` for health checks, and runs as a non-root user.
- `Dockerfile.dev` is used only by `compose.dev.yaml`. It retains the .NET SDK and runs the API through `dotnet watch`.
- Backend source is synchronized through Compose Watch. Do not add broad host bind mounts or sync `bin/` and `obj/`; Windows build artifacts are incompatible with the Linux container runtime.
- `compose.dev.yaml` publishes PostgreSQL on `127.0.0.1:${POSTGRES_HOST_PORT:-5432}` for local database tools. It uses a dedicated `db-host-access` network because the API/PostgreSQL `data` network is internal. Production PostgreSQL must remain unexposed.
- `StaticWebAssetsEnabled=false` in `Loyeris.Api.csproj` is intentional. The API does not serve static assets, and disabling this avoids static-web-assets failures during containerized `dotnet watch`.
- Public health endpoints are `GET /api/health/live` for process liveness and `GET /api/health/ready` for API plus PostgreSQL readiness.
- `Loyeris.Api --migrate` applies migrations for all six domain `DbContext` instances and exits non-zero on failure. Compose runs it as a one-shot `migrations` service before starting the API. Normal API startup must never apply migrations automatically.
- Required runtime configuration uses .NET environment variable names such as `ConnectionStrings__LoyerisDatabase`, `Jwt__SigningKey`, `ApplicationUrls__FrontendBaseUrl`, and `Smtp__*`. SMTP username and password are optional, but must be supplied together.
- Caddy terminates HTTPS in production. Keep forwarded-header processing before HTTPS redirection, rate limiting, and authentication so client IPs and secure-cookie behavior remain correct behind the proxy.
- When changing containers or operational behavior, validate both `compose.dev.yaml` and the production-like root `compose.yaml`.

## Architecture

- Keep HTTP concerns in `Loyeris.Api`.
- Group Minimal API routes under endpoint classes in `Loyeris.Api/Endpoints/`.
- Use MediatR requests and handlers for application behavior.
- Keep reusable result mapping in `Loyeris.Api/Extensions/`.
- Keep shared primitives in `Loyeris.Shared`; avoid app-specific logic there.
- Keep infrastructure and persistence concerns out of domain `App` projects.
- Register MediatR from each domain application assembly through `*ApplicationAssemblyReference` marker types.
- Register infrastructure services through one `Add<Domain>Infrastructure(configuration)` method per infrastructure project.
- Do not apply EF migrations automatically at application startup.

Current API shape:

- `Program.cs` registers OpenAPI, MediatR, all domain infrastructure services, and endpoint groups.
- `Program.cs` also configures validated runtime options, forwarded headers, health checks, and the explicit migration mode.
- Endpoint groups currently expose starter GET routes for `IdentityAccess`, `Portfolio`, `Leasing`, `RentCollection`, `TaxPreparation`, and `Messaging`.
- Every endpoint sends a MediatR query and converts the `Result<T>` through `ToHttpResult()`.
- Endpoints are read-only starters for now; do not add write endpoints unless the task explicitly asks for commands to be exposed.

## CQRS conventions

Each `*.App` project follows this structure:

| Folder | Role |
|---|---|
| `Commands/` | Write requests and command handlers when write use cases exist |
| `Queries/` | Read requests used by GET endpoints and read flows |
| `Handlers/` | MediatR handlers for commands and queries |
| `Dtos/` | Flat response/read models returned by application queries |
| `Persistence/` | Application-owned repository interfaces and unit-of-work contracts |

Rules:

- Queries return `Result<T>` and should use read repositories.
- Commands return `Result<T>` or `Result<Unit>` style outcomes and should enforce use-case rules through the domain owner.
- Handlers stay small and orchestrate use cases; do not put HTTP details in handlers.
- Do not inject EF Core `DbContext` directly into `App` projects.
- Do not return EF entities from API endpoints; return DTOs/read models.
- Do not expose sensitive fields in DTOs, including `PasswordHash`, `SecurityStamp`, `TokenHash`, or outbox `Payload`.

## DAL and persistence

There is one PostgreSQL database and separate schemas by domain:

| Domain | DbContext | Schema |
|---|---|---|
| `IdentityAccess` | `IdentityAccessDbContext` | `identity` |
| `Portfolio` | `PortfolioDbContext` | `portfolio` |
| `Leasing` | `LeasingDbContext` | `leasing` |
| `RentCollection` | `RentCollectionDbContext` | `collection` |
| `TaxPreparation` | `TaxPreparationDbContext` | `tax` |
| `Messaging` | `MessagingDbContext` | `messaging` |

Rules:

- Keep EF Core configurations under `Persistence/Configurations/`.
- Keep repositories under `Persistence/Repositories/`.
- Use `AsNoTracking()` for read repositories unless change tracking is explicitly required.
- Project directly to DTOs for read endpoints.
- Keep enums stored as text.
- Keep identifiers as `Guid`.
- Keep monetary values as cents in `long`.
- Use `DateOnly` for business dates and `DateTimeOffset` for technical timestamps.
- Do not create EF navigation properties across domains; use IDs for cross-domain references.
- Keep domain invariants in the owning domain.
- Keep migrations inside each infrastructure project under `Migrations/`.

Authentication persistence lives in `IdentityAccess`:

- `app_users` owns account security state, including password hash and security stamp.
- `auth_sessions` tracks browser/device sessions.
- `refresh_tokens` stores hashed refresh tokens only.
- `auth_one_time_tokens` stores hashed one-time tokens only.
- `auth_events` stores lightweight security audit events.

## Testing conventions

- Framework: NUnit.
- Mocking: Moq.
- Assertions: FluentAssertions.
- Test namespaces mirror production namespaces, for example `Loyeris.IdentityAccess.App.Handlers` maps to `Loyeris.Tests.IdentityAccess.Handlers`.
- Use the AAA pattern in every test method with explicit `// Arrange`, `// Act`, and `// Assert` comments.
- Test classes have XML `<summary>` documentation.
- Test methods have XML `<summary>` documentation and use `<see cref/>` for referenced production types when relevant.
- Add focused tests when changing behavior.
- For CQRS read handlers, prefer repository mocks and assert successful `Result<T>` behavior.
- For persistence changes, verify EF model shape, indexes, filters, schemas, enum mappings, and constraints where relevant.
- For API endpoint groups, verify routes are registered when adding or changing endpoint mappings.

## C# conventions

- Follow the existing namespace style and folder boundaries.
- Keep one public type per `.cs` file. Do not group multiple classes, records, interfaces, or enums in a single file.
- Keep one EF Core configuration class per file; name the file after the class it contains.
- Keep public APIs documented when the surrounding code is documented.
- Prefer small request/handler classes over adding behavior directly in endpoint lambdas.
- Do not change the `MediatR` version. Keep all backend projects pinned to `12.5.0`; do not upgrade to `13.0.0+` because those releases are under the commercial licensing model.
- Preserve nullable settings per project; do not flip nullable globally as part of unrelated work.
- Keep error handling expressed through `Result<T>` and `Error` unless there is an established exception-based pattern nearby.
- Use English for code comments and XML documentation.
