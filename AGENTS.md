# Loyeris — Agent instructions

## Product

Loyeris is a SaaS de gestion locative for French SCI à l'IR. The product model is:

- SCI own lots.
- Tenants are assigned to lots.
- Monthly rent deadlines drive alerts, reminders, and rent tracking.

Keep product language and UI copy in French.

## Repository structure

This is a mono-repo with two independent projects plus a static design reference:

| Path | Role |
|---|---|
| `frontend/loyeris-app/` | Angular 21 app, standalone components, no NgModules, `core / features / shared` app structure |
| `backend/` | .NET 10 solution using `.slnx`, Minimal API, MediatR |
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
```

Backend commands are run from the project folders noted in `backend/AGENTS.md`:

```bash
dotnet build
dotnet test
dotnet run
```

There is no CI configuration in `.github/workflows/`.

## Design reference

Use `docs/ui-kit/index.html` as the design map. It links mockups for login, dashboard, SCI list, lots, tenants, rent tracking, and settings.

The UI kit uses `data-theme="corporate"`, Plus Jakarta Sans, Tailwind 4, daisyUI 5, and custom reference classes in `docs/ui-kit/styles.css`. When implementing Angular screens, reproduce the visual language with app-local CSS, Tailwind utilities, and daisyUI classes rather than copying the static files wholesale.

## General conventions

- Keep changes scoped to the requested project unless cross-project behavior requires otherwise.
- Prefer existing project patterns over introducing new architecture.
- Frontend features are organized under `src/app/features/`, with app-wide concerns reserved for `src/app/core/` and cross-feature reusable code reserved for `src/app/shared/`.
- Within frontend feature folders, routed screens belong in `pages/`, feature-private UI components belong in `components/`, and the feature route file stays at the feature root.
- The current login feature lives under `frontend/loyeris-app/src/app/features/login/`.
- Do not mix package managers. The frontend uses Yarn v1 only.
- Keep generated or dependency folders out of manual edits.
- Use French for end-user HTML text and labels.
- Update or add focused tests when changing behavior.
