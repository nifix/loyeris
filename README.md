# Loyeris

SaaS de gestion locative pour les SCI françaises à l'IR.

## Développement quotidien avec hot reload

Les valeurs par défaut sont réservées au développement. Pour les personnaliser,
copier `.env.example` vers `.env`, puis lancer :

```bash
docker compose --file compose.dev.yaml up --build --watch
```

- application : http://localhost:8080
- API directe : http://localhost:5130
- MailDev 3 (`3.0.0-rc.1`) : http://localhost:1080
- PostgreSQL : `localhost:5432` par défaut, avec les identifiants du fichier `.env`

PostgreSQL est publié uniquement sur l'interface locale Windows. Le port peut
être modifié avec `POSTGRES_HOST_PORT` dans `.env`, notamment si une autre
instance PostgreSQL utilise déjà le port `5432`. La stack de production ne
publie jamais PostgreSQL.

La première exécution crée PostgreSQL, applique les migrations de chaque domaine,
puis lance `dotnet watch` et `ng serve`. Une modification C# redémarre l'API et
une modification Angular recharge automatiquement le frontend.

Pour suivre uniquement les recompilations applicatives :

```bash
docker compose --file compose.dev.yaml logs --follow api web
```

Après l'ajout d'une migration EF Core, reconstruire l'image API puis appliquer
explicitement les migrations sans arrêter la stack :

```bash
docker compose --file compose.dev.yaml build api
docker compose --file compose.dev.yaml run --rm migrations
```

Après une modification de `package.json`, `yarn.lock` ou des dépendances NuGet,
reconstruire les images de développement :

```bash
docker compose --file compose.dev.yaml up --build --watch
```

Pour arrêter la stack de développement :

```bash
docker compose --file compose.dev.yaml down
```

Les données PostgreSQL sont conservées dans un volume dédié. Pour repartir avec
une base et des caches de dépendances vides :

```bash
docker compose --file compose.dev.yaml down --volumes
```

## Validation locale proche de la production

Avant un push important, la stack sans hot reload construit les images finales,
sert Angular avec Caddy et reproduit le comportement vérifié par la CI :

```bash
docker compose up --build
```

Pour l'arrêter :

```bash
docker compose down
```

Les stacks `loyeris-dev` et `loyeris` utilisent des volumes PostgreSQL séparés.

## Production

Le manifeste et la préparation future du VPS sont documentés dans
[`deploy/README.md`](deploy/README.md). Les pull requests exécutent seules la CI
complète jusqu'au smoke test. Un workflow distinct, déclenché uniquement par un
push sur `main`, regroupe la publication des images GHCR et le déploiement vers
le VPS, mais reste explicitement désactivé.
