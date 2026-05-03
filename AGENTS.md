# Loyeris — Agent instructions

## Product

SaaS de gestion locative pour les SCI à l'IR françaises. SCI → lots → locataires assignés aux lots. Alertes/relances sur échéances mensuelles.

## Structure

Mono-repo, two independent projects:

| Path | Role |
|---|---|
| `frontend/loyeris-app/` | Angular 21 app (standalone, no NgModules) |
| `backend/` | .NET 10 solution (`.slnx` format), MediatR minimal API |
| `docs/ui-kit/` | Static HTML/CSS design reference (daisyUI 5 + Tailwind 4) |

### Backend projects (`backend/Loyeris.slnx`)

| Project | Layer |
|---|---|
| `Loyeris.Api/` | Minimal API host (MediatR, OpenAPI) |
| `Loyeris.Auth.App/` | Application — CQRS queries/handlers |
| `Loyeris.Auth.Core/` | Domain — empty stub |
| `Loyeris.Auth.Infrastructure/` | Infrastructure — empty stub |
| `Loyeris.Shared/` | Shared kernel — `Result<T>`, `Error` types |

All target `net10.0`, `Nullable: disable`, `ImplicitUsings: enable`.

## Commands

Run everything from `frontend/loyeris-app/`:

```bash
yarn start   # ng serve — http://localhost:4200
yarn build   # ng build
yarn test    # ng test — Vitest (not Karma)
yarn watch   # ng build --watch --configuration development
```

No lint or typecheck scripts (`ng lint` not configured). No CI (no `.github/workflows/`). Prettier config embedded in `package.json`.

## Tooling quirks

- **Package manager:** `yarn` v1.22.22 — lockfile `yarn.lock`, `packageManager` field set. Never `npm`.
- **Tailwind v4:** CSS-first — no `tailwind.config.js`. Imports in `src/styles.css` via `@import "tailwindcss"`; daisyUI via `@plugin "daisyui"`. PostCSS at `.postcssrc.json`.
- **TypeScript:** strict mode, `module: "preserve"`, decorators enabled. Vitest globals in `tsconfig.spec.json`.
- **EditorConfig:** 2-space indent, single quotes for `.ts`.
- **Angular build:** `@angular/build:application` (Application builder). Vitest via `@angular/build:unit-test` builder — no `vitest.config.*` file.
- **Test env:** jsdom (no Karma anywhere).
- **Prettier:** embedded in `package.json` — `printWidth: 100`, `singleQuote: true`, Angular HTML parser.

## Current state

- **Very early:** single `App` component, empty routes, no pages wired
- **`index.html`** has `lang="en"` — should be `"fr"`
- Plus Jakarta Sans font and `data-theme="corporate"` not yet imported in Angular app
- No services, no HTTP layer, no state management beyond `signal()`

## UI Kit (design reference)

Open `docs/ui-kit/index.html` in a browser. Mockups for: login, dashboard, SCI list, lots, tenants, rent tracking, settings. Uses `data-theme="corporate"`, Plus Jakarta Sans, custom CSS classes (`auth-shell`, `glass-panel`, `brand-mark`, `page-header`, etc.) in `docs/ui-kit/styles.css` — replicate with Tailwind/daisyUI utilities in Angular.

## Conventions

- Standalone components, `templateUrl` + `styleUrl` (not inline), `protected readonly` for signals
- Angular CLI prefix: `app`
- HTML is French (`lang="fr"`)
- **File naming:** Component files omit `.component` infix — `app.ts`, `app.html`, `app.css` (not `app.component.*`)
