# Loyeris Frontend — Agent instructions

## Scope

These instructions apply to everything under `frontend/`. The Angular application lives in `frontend/loyeris-app/`.

## Stack

- Angular 21.
- Standalone components only; no NgModules.
- TypeScript strict mode with `module: "preserve"` and decorators enabled.
- Tailwind 4 using CSS-first configuration.
- daisyUI 5 loaded from CSS.
- Vitest through Angular's `@angular/build:unit-test` builder.
- Package manager: Yarn v1.22.22 only.

Do not add Karma, `vitest.config.*`, `tailwind.config.js`, or npm lockfiles unless explicitly requested.

## Commands

Run all frontend commands from `frontend/loyeris-app/`.

```bash
yarn start   # ng serve, http://localhost:4200
yarn build   # ng build
yarn test    # ng test, Vitest/jsdom
yarn watch   # ng build --watch --configuration development
```

There is no `ng lint` setup and no separate typecheck script.

## Tooling details

- `packageManager` is `yarn@1.22.22`.
- Lockfile is `yarn.lock`.
- Angular CLI package manager is set to Yarn in `angular.json`.
- Build builder is `@angular/build:application`.
- Test builder is `@angular/build:unit-test`.
- Prettier config is embedded in `package.json`.
- `.editorconfig` uses 2-space indent, final newline, and single quotes for TypeScript.

## Styling and UI

- Global styles are in `src/styles.css`.
- Tailwind is imported with `@import "tailwindcss";`.
- daisyUI is loaded with `@plugin "daisyui";`.
- `src/index.html` uses `lang="fr"` and `data-theme="corporate"`.
- Plus Jakarta Sans is loaded in `src/index.html` and applied in global CSS.

Use `../../docs/ui-kit/` as the design reference for Angular screens. Recreate the visual behavior with Angular templates, component CSS, Tailwind utilities, and daisyUI classes. Do not depend on the static UI kit files at runtime.

## Angular conventions

- Components are standalone and use `templateUrl` plus `styleUrl`.
- Component filenames omit the `.component` infix, for example `login.ts`, `login.html`, `login.css`.
- Angular CLI prefix is `app`.
- Prefer `protected readonly` for signals and template-facing immutable fields.
- Keep templates and styles out of inline component metadata unless the nearby component already uses that pattern.
- Keep end-user copy in French.
- Keep routes in `src/app/app.routes.ts`; feature screens should be route-aligned and lazy-loaded where practical.

## Application architecture

The Angular app follows the `core / features / shared` folder structure:

- `src/app/core/` is reserved for app-wide concerns such as layouts, guards, interceptors, global services, and provider helpers.
- `src/app/features/` contains domain or route-aligned features. Each feature owns its page components and feature-private UI components.
- Inside a feature, put routed/container screens under `pages/` and feature-private presentation components under `components/`.
- Keep feature route files at the feature root, for example `src/app/features/login/login.routes.ts`.
- `src/app/shared/` is reserved for reusable presentation components, directives, pipes, and utilities used by more than one feature.
- Keep feature-private components colocated inside their feature folder until they are reused across features.
- Do not introduce barrel files by default; keep imports explicit unless a local convention emerges.

## Current application state

- The app is still early.
- `App` renders routed content through `RouterOutlet`.
- Routes are wired in `src/app/app.routes.ts`.
- The login page lives under `src/app/features/login/pages/login-page/`.
- Login-only UI components live under `src/app/features/login/components/`.
- `core/` and `shared/` exist as placeholders until app-wide or cross-feature concerns appear.
- There is no HTTP layer, no services, and no state management beyond Angular primitives.

## Testing

- Tests use Vitest globals in `tsconfig.spec.json`.
- Test environment is jsdom via Angular's unit-test builder.
- Keep tests focused on user-visible behavior and component contracts.
- Add or update tests for meaningful component logic, routing changes, and service behavior.
