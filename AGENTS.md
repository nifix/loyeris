# Loyeris — Agent instructions

## Product

Loyeris is a SaaS de gestion locative for French SCI à l'IR. The product model is:

- SCI own lots.
- Tenants are assigned to lots.
- Monthly rent deadlines drive alerts, reminders, and rent tracking.

Keep product language and UI copy in French.

## Repository structure

This is a mono-repo with two independent projects, container orchestration, and a static design reference:

| Path | Role |
|---|---|
| `frontend/loyeris-app/` | Angular 22 app, standalone components, no NgModules, `core / features / shared` app structure |
| `backend/` | .NET 10 solution using `.slnx`, Minimal API, MediatR |
| `compose.dev.yaml` | Daily full-stack development with Compose Watch and hot reload |
| `compose.yaml` | Local production-like stack used for validation and CI smoke tests |
| `deploy/` | Production Compose manifest and deployment script, kept ready for a future VPS |
| `.github/workflows/ci-cd.yml` | Pull-request checks and GHCR image publication from `main` |
| `docs/ui-kit/` | Static HTML/CSS design reference using Tailwind 4 and daisyUI 5 |

More specific instructions exist in:

- `backend/AGENTS.md` for backend work.
- `frontend/AGENTS.md` for frontend work.

When files fall under one of those folders, follow the closest `AGENTS.md` first.

## Commands

Frontend commands are run from `frontend/loyeris-app/`:

```bash
yarn start
yarn build
yarn test
yarn watch
yarn storybook
yarn build-storybook
```

Backend commands are run from the project folders noted in `backend/AGENTS.md`:

```bash
dotnet build
dotnet test
dotnet run
```

## Containers

Run Compose commands from the repository root. Copy `.env.example` to `.env` when local overrides are needed; never commit `.env` or real credentials.

Use the development stack for daily work:

```bash
docker compose --file compose.dev.yaml up --build --watch
```

- Angular is available at `http://localhost:8080`, the API at `http://localhost:5130`, MailDev at `http://localhost:1080`, and development PostgreSQL at `localhost:${POSTGRES_HOST_PORT:-5432}`.
- The development PostgreSQL port is bound to `127.0.0.1` through a dedicated `db-host-access` network because the private `data` network is internal. Configure `POSTGRES_HOST_PORT` in `.env` if port `5432` is already occupied; production PostgreSQL must remain unexposed.
- Local stacks pin MailDev to the official `3.0.0-rc.1` image. Keep an exact tag while v3 remains a release candidate; its REST endpoint is used by the Docker CI smoke test. Preserve the IPv4 `/api/healthz` override because the image's `localhost` healthcheck resolves incorrectly under Docker Desktop.
- Compose Watch synchronizes source changes and provides Angular and .NET hot reload. Keep its sync rules instead of adding broad bind mounts: host `bin/`, `obj/`, and `node_modules/` artifacts must not be mixed with Linux container artifacts.
- The development stack uses isolated project names and volumes, so it does not share PostgreSQL data or dependency caches with the production-like stack.
- After creating an EF migration, rebuild the API image and run the migration task explicitly:

```bash
docker compose --file compose.dev.yaml build api
docker compose --file compose.dev.yaml run --rm migrations
```

Use the production-like local stack when validating final images and Caddy behavior:

```bash
docker compose up --build
```

It serves the application through Caddy at `http://localhost:8080`; the API remains available on port `5130` for local diagnostics, and MailDev on port `1080`. This stack builds the same production Dockerfiles used by CI.

Stop a stack with the matching Compose file. Add `--volumes` only when intentionally resetting its database and caches.

## CI and images

`.github/workflows/ci-cd.yml` validates the backend, frontend, Compose manifests, and complete Docker stack. On a successful push to `main`, it publishes immutable SHA-tagged API and web images to `ghcr.io/nifix/` and also updates the informational `latest` tags. There is currently no VPS deployment job; files under `deploy/` are inactive until a VPS is provisioned.

## Design reference

Use `docs/ui-kit/index.html` as the design map. It links mockups for login, dashboard, SCI list, lots, tenants, rent tracking, and settings.

The UI kit uses `data-theme="corporate"`, Plus Jakarta Sans, Tailwind 4, daisyUI 5, and custom reference classes in `docs/ui-kit/styles.css`. When implementing Angular screens, reproduce the visual language with app-local CSS, Tailwind utilities, and daisyUI classes rather than copying the static files wholesale.

## General conventions

- Keep changes scoped to the requested project unless cross-project behavior requires otherwise.
- Prefer existing project patterns over introducing new architecture.
- Do not change the MediatR version. Keep backend projects pinned to `MediatR` `12.5.0`; versions `13.0.0+` are under the commercial licensing model and must not be introduced.
- Frontend features are organized under `src/app/features/`, with app-wide concerns reserved for `src/app/core/` and cross-feature reusable code reserved for `src/app/shared/`.
- Within frontend feature folders, routed screens belong in `pages/`, feature-private UI components belong in `components/`, and the feature route file stays at the feature root.
- The current login feature lives under `frontend/loyeris-app/src/app/features/login/`.
- Do not mix package managers. The frontend uses Yarn v1 only.
- Keep generated or dependency folders out of manual edits.
- Use French for end-user HTML text and labels.
- Update or add focused tests when changing behavior.
