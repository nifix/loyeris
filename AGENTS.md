# Loyeris — Agent instructions

## Product

SaaS de gestion locative pour les SCI à l'IR françaises. Alternative aux tableaux Excel pour les petits propriétaires.

- **SCI** → lots → **locataires** assignés aux lots
- Système d'alerte et de relance sur les échéances mensuelles
- Pas de backend pour l'instant — tout le travail est frontend-only
- Backend prévu plus tard : API .NET 10 dans `backend/` (actuellement vide)

## Structure

Mono-repo with a single Angular app.

| Path | Role |
|---|---|
| `frontend/loyeris-app/` | Angular 21 app (standalone components, no NgModules) |
| `backend/` | Placeholder — empty |
| `docs/ui-kit/` | Static HTML/CSS design reference (daisyUI 5 + Tailwind 4), open in browser to preview |

## Commands

Run everything from `frontend/loyeris-app/`:

```bash
yarn start          # ng serve — http://localhost:4200
yarn build          # ng build
yarn test           # ng test — Vitest (not Karma)
yarn watch          # ng build --watch --configuration development
```

No lint or typecheck scripts exist (`ng lint` not configured). CI not set up (no `.github/workflows/`). Prettier config is embedded in `package.json`.

## Tooling quirks

- **Package manager:** `yarn` (v1.22.22) — `angular.json` has `cli.packageManager: "npm"`, so `ng` subcommands may default to npm. Always use `yarn`.
- **Tailwind v4:** CSS-first config — no `tailwind.config.js`. Imported in `src/styles.css` via `@import "tailwindcss"`; daisyUI via `@plugin "daisyui"`. PostCSS at `.postcssrc.json`.
- **TypeScript:** strict mode, `module: "preserve"`, decorators enabled, Vitest globals in `tsconfig.spec.json`.
- **EditorConfig:** 2-space indent, single quotes for `.ts`.
- **Angular build:** uses `@angular/build:application` (Application builder, not deprecated).

## Current state

- **Very early stage:** single `App` component, empty routes, no pages wired
- **`app.config.ts`** uses `provideBrowserGlobalErrorListeners()` + `provideRouter(routes)`
- **`src/index.html`** has `lang="en"` (should be `"fr"` per convention)
- **Tests are broken:** `app.spec.ts` asserts `<h1>` with "Hello, loyeris-app" but the template only has a `<button>` with "Testouille"
- No services, no HTTP layer, no state management beyond `signal()`

## UI Kit (design reference)

Open `docs/ui-kit/index.html` in a browser. Contains mockups for: login, dashboard, SCI list, lots, tenants, rent tracking, settings. Built with daisyUI 5 + Tailwind 4 CDN. Uses `data-theme="corporate"` and font **Plus Jakarta Sans** (not yet imported in the Angular app). Custom CSS classes (`auth-shell`, `glass-panel`, `brand-mark`, `page-header`, etc.) in `docs/ui-kit/styles.css` — replicate with Tailwind/daisyUI utilities in Angular.

## Conventions

- Standalone components, `templateUrl` + `styleUrl` (not inline), `protected readonly` for signals
- Angular CLI prefix: `app`
- HTML is French (`lang="fr"`)
