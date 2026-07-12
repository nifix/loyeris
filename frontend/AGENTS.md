# Loyeris Frontend — Agent instructions

## Scope

These instructions apply to everything under `frontend/`. The Angular application lives in `frontend/loyeris-app/`.

## Stack

- Angular 22.0.
- Standalone components only; no NgModules.
- TypeScript 6.0 in strict mode with `module: "preserve"` and decorators enabled.
- Tailwind 4.3 using CSS-first configuration.
- daisyUI 5.6 loaded from CSS.
- Storybook 10.5 for component stories and documentation.
- Vitest 4.1 through Angular's `@angular/build:unit-test` builder.
- Package manager: Yarn v1.22.22 only.

Angular 22 requires Node.js `^22.22.3`, `^24.15.0`, or `>=26.0.0`. The current frontend has been validated with Node.js 24.15.0.

Do not add Karma, `vitest.config.*`, `tailwind.config.js`, or npm lockfiles unless explicitly requested.

## Commands

Run all frontend commands from `frontend/loyeris-app/`.

```bash
yarn start   # ng serve, http://localhost:4200
yarn build   # ng build
yarn test    # ng test, Vitest/jsdom
yarn watch   # ng build --watch --configuration development
yarn storybook        # ng run loyeris-app:storybook, http://localhost:6006
yarn build-storybook  # ng run loyeris-app:build-storybook
```

There is no `ng lint` setup and no separate typecheck script.

## Containers

- `Dockerfile` is the production multi-stage build: Node.js 24 and Yarn 1.22.22 build Angular, then Caddy serves `dist/loyeris-app/browser`.
- `Caddyfile` proxies `/api/*` to `api:8080` without rewriting the path, applies the Angular `index.html` fallback, compresses responses, caches hashed assets, and prevents long-lived caching of `index.html`.
- Frontend API calls must remain relative to `/api`. Do not hardcode a backend host or introduce CORS for the normal Caddy-served application.
- `Dockerfile.dev` is used only by `compose.dev.yaml` and runs `ng serve` with hot reload on container port `4200`.
- Native `yarn start` uses `proxy.conf.json` and targets the host API at `http://localhost:5130`. Container development uses `proxy.docker.conf.json` and targets `http://api:8080`; do not merge these two proxy targets.
- Compose Watch synchronizes frontend source and rebuilds the development image when `package.json` or `yarn.lock` changes. Do not bind-mount or synchronize host `node_modules/`, `dist/`, or `storybook-static/` into the Linux container.
- Start the complete hot-reload environment from the repository root with `docker compose --file compose.dev.yaml up --build --watch`; the frontend is then available at `http://localhost:8080`.
- The root `compose.yaml` uses the final Caddy image and is the appropriate stack for production-like validation and CI smoke tests.
- Keep Yarn v1 as the only package manager in every Docker stage and container workflow.

## Tooling details

- `packageManager` is `yarn@1.22.22`.
- Lockfile is `yarn.lock`.
- Angular packages and Angular CLI are on 22.0.x.
- Keep TypeScript on `>=6.0 <6.1`, as required by Angular 22.0.x.
- Angular CLI package manager is set to Yarn in `angular.json`.
- Build builder is `@angular/build:application`.
- Test builder is `@angular/build:unit-test`.
- Prettier config is embedded in `package.json`.
- `.editorconfig` uses 2-space indent, final newline, and single quotes for TypeScript.

## Storybook

- Storybook is on 10.5.x.
- Storybook config lives in `frontend/loyeris-app/.storybook/`.
- Stories are discovered from `src/**/*.stories.ts`.
- Keep stories colocated with components and use Angular Storybook patterns (`Meta`/`StoryObj`).
- Storybook 10.5's published TypeScript peer range does not yet include TypeScript 6, but `yarn build-storybook` is validated with this Angular 22/TypeScript 6 setup. Do not downgrade TypeScript to silence that peer warning.

## Styling and UI

- Global styles are in `src/styles.css`.
- Tailwind is imported with `@import "tailwindcss";`.
- daisyUI is loaded with `@plugin "daisyui";`.
- `src/index.html` uses `lang="fr"` and `data-theme="corporate"`.
- Plus Jakarta Sans is loaded in `src/index.html` and applied in global CSS.

Use `../../docs/ui-kit/` as the design reference for Angular screens. Recreate the visual behavior with Angular templates, component CSS, Tailwind utilities, and daisyUI classes. Do not depend on the static UI kit files at runtime.

## Angular conventions

- Components are standalone and use `templateUrl` plus `styleUrl`.
- Prefer signals for component state and keep Angular 22's default OnPush change-detection behavior.
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
- Login, account registration, and email verification pages live under `src/app/features/login/pages/` and are exposed at `/login`, `/register`, and `/verify-email`.
- Authentication-only UI components, including the shared authentication layout and form helpers, live under `src/app/features/login/components/`.
- Every routed frontend page has a full-page Storybook story; authentication stories additionally cover submission-error and verification-status variants.
- Product screens are implemented under `src/app/features/` for login, dashboard, SCI, lots and lot detail, tenants and tenant detail, rents, and settings.
- Each feature keeps routed screens in `pages/` and page-private UI in `components/`; shared detail headers, cards, metrics, badges, and brand elements live under `src/app/shared/components/`.
- `core/` contains the application shell, while `shared/` contains reusable presentation components used across features.
- There is no HTTP layer, no services, and no state management beyond Angular primitives.

## Testing

- Tests use Vitest globals in `tsconfig.spec.json`.
- Test environment is jsdom via Angular's unit-test builder.
- Keep tests focused on user-visible behavior and component contracts.
- Add or update tests for meaningful component logic, routing changes, and service behavior.
