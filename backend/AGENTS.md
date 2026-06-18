# Loyeris Backend — Agent instructions

## Scope

These instructions apply to everything under `backend/`.

The backend is a .NET 10 solution using the `.slnx` format, ASP.NET Core Minimal API, and MediatR for CQRS-style request handling.

## Projects

| Project | Layer |
|---|---|
| `Loyeris.Api/` | Minimal API host, endpoint groups, OpenAPI setup |
| `Loyeris.IdentityAccess.App/` | Application layer for identity, workspace access, and authentication use cases |
| `Loyeris.IdentityAccess.Core/` | Domain layer for identity, workspace access, and authentication entities |
| `Loyeris.IdentityAccess.Infrastructure/` | Infrastructure layer for identity, workspace access, and authentication persistence |
| `Loyeris.Shared/` | Shared kernel, including `Result<T>` and `Error` types |
| `Loyeris.Tests/` | NUnit tests with Moq and FluentAssertions |

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

The API runs on `http://localhost:5000` when launched from `Loyeris.Api/`.

## Architecture

- Keep HTTP concerns in `Loyeris.Api`.
- Group Minimal API routes under endpoint classes in `Loyeris.Api/Endpoints/`.
- Use MediatR requests and handlers for application behavior.
- Keep reusable result mapping in `Loyeris.Api/Extensions/`.
- Keep shared primitives in `Loyeris.Shared`; avoid app-specific logic there.
- Do not put infrastructure or persistence concerns into domain `App` projects.

Current API shape:

- `Program.cs` registers OpenAPI and MediatR.
- Endpoint groups are registered under `Loyeris.Api/Endpoints/` when the corresponding use cases exist.
- Result objects are converted to HTTP responses through `ToHttpResult()`.

## Testing conventions

- Framework: NUnit.
- Mocking: Moq.
- Assertions: FluentAssertions.
- Test namespaces mirror production namespaces, for example `Loyeris.IdentityAccess.App.Handlers` maps to `Loyeris.Tests.IdentityAccess.Handlers`.
- Use the AAA pattern in every test method with explicit `// Arrange`, `// Act`, and `// Assert` comments.
- Test classes have XML `<summary>` documentation.
- Test methods have XML `<summary>` documentation and use `<see cref/>` for referenced production types when relevant.

## C# conventions

- Follow the existing namespace style and folder boundaries.
- Keep one class per `.cs` file. Do not group multiple classes in a single file, including EF Core configuration classes; name the file after the class it contains.
- Keep public APIs documented when the surrounding code is documented.
- Prefer small request/handler classes over adding behavior directly in endpoint lambdas.
- Do not change the `MediatR` version. Keep all backend projects pinned to `12.5.0`; do not upgrade to `13.0.0+` because those releases are under the commercial licensing model.
- Preserve nullable settings per project; do not flip nullable globally as part of unrelated work.
- Keep error handling expressed through `Result<T>` and `Error` unless there is an established exception-based pattern nearby.
