# Déploiement du VPS

> Le déploiement VPS n'est pas encore activé dans GitHub Actions. Le workflow
> actuel construit et publie uniquement les images GHCR. Cette documentation et
> les scripts sont conservés pour l'activation future du déploiement.

## Préparation unique

Le VPS doit disposer d'un Linux x86-64, de Docker Engine, du plugin Docker Compose,
de `curl` et de `flock`. Les ports entrants 80 et 443 doivent être ouverts, et le
DNS du domaine doit pointer vers le VPS.

Créer un utilisateur SSH dédié au déploiement, autorisé à utiliser Docker, puis
préparer le répertoire :

```bash
sudo install -d -m 700 -o loyeris-deploy -g loyeris-deploy /opt/loyeris
sudo -u loyeris-deploy cp .env.production.example /opt/loyeris/.env
sudo -u loyeris-deploy chmod 600 /opt/loyeris/.env
```

Compléter `/opt/loyeris/.env` avec le domaine, un mot de passe PostgreSQL aléatoire,
une clé JWT aléatoire d'au moins 32 octets et les identifiants du fournisseur SMTP.
Le fichier ne doit jamais être ajouté au dépôt.

> L'appartenance au groupe `docker` donne des privilèges équivalents à root. Le
> compte doit utiliser une clé SSH dédiée, sans mot de passe interactif et sans
> autre usage applicatif.

## Configuration GitHub

Créer l'environnement GitHub `production` et y ajouter :

- `DEPLOY_HOST` : nom DNS ou IP SSH du VPS ;
- `DEPLOY_USER` : utilisateur SSH dédié ;
- `DEPLOY_SSH_KEY` : clé privée dédiée ;
- `DEPLOY_KNOWN_HOSTS` : ligne `known_hosts` vérifiée du VPS.

Protéger ensuite `main` en exigeant les contrôles `Backend`, `Frontend`,
`Compose` et `Docker smoke test`.

## Sauvegardes et migrations

Chaque déploiement conserve un dump PostgreSQL pendant 14 jours dans
`/opt/loyeris/backups`. Une sauvegarde chiffrée hors VPS reste obligatoire avant
d'héberger des données réelles. Les migrations doivent suivre une stratégie
extension/réduction : une version ne supprime pas les colonnes encore utilisées
par la version applicative précédente.
