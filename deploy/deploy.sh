#!/usr/bin/env bash
set -Eeuo pipefail

if [[ $# -ne 1 || ! "$1" =~ ^[0-9a-f]{40}$ ]]; then
  echo "Usage: $0 <full-git-sha>" >&2
  exit 64
fi

readonly NEW_IMAGE_TAG="$1"
readonly DEPLOY_DIR="/opt/loyeris"
readonly COMPOSE_FILE="$DEPLOY_DIR/compose.production.yaml"
readonly ENV_FILE="$DEPLOY_DIR/.env"
readonly RELEASE_FILE="$DEPLOY_DIR/.release.env"
readonly BACKUP_DIR="$DEPLOY_DIR/backups"

cd "$DEPLOY_DIR"

if [[ ! -f "$ENV_FILE" ]]; then
  echo "Missing production environment file: $ENV_FILE" >&2
  exit 66
fi

exec 9>"$DEPLOY_DIR/.deploy.lock"
if ! flock -n 9; then
  echo "Another Loyeris deployment is already running." >&2
  exit 75
fi

APP_DOMAIN="$(grep -m1 '^APP_DOMAIN=' "$ENV_FILE" | cut -d= -f2- | tr -d '\r')"
if [[ -z "$APP_DOMAIN" ]]; then
  echo "APP_DOMAIN must be set in $ENV_FILE" >&2
  exit 65
fi

PREVIOUS_IMAGE_TAG=""
if [[ -f "$RELEASE_FILE" ]]; then
  PREVIOUS_IMAGE_TAG="$(grep -m1 '^IMAGE_TAG=' "$RELEASE_FILE" | cut -d= -f2- | tr -d '\r' || true)"
fi

compose() {
  docker compose \
    --env-file "$ENV_FILE" \
    --env-file "$RELEASE_FILE" \
    --file "$COMPOSE_FILE" \
    "$@"
}

write_release() {
  printf 'IMAGE_TAG=%s\n' "$1" > "$RELEASE_FILE"
  chmod 600 "$RELEASE_FILE"
}

rollback_images() {
  if [[ ! "$PREVIOUS_IMAGE_TAG" =~ ^[0-9a-f]{40}$ ]]; then
    echo "No previous immutable image tag is available for rollback." >&2
    return 1
  fi

  echo "Rolling application images back to $PREVIOUS_IMAGE_TAG." >&2
  write_release "$PREVIOUS_IMAGE_TAG"
  compose pull api web
  compose up --detach --no-deps api web

  for _ in {1..30}; do
    if curl --fail --silent --show-error "https://$APP_DOMAIN/api/health/ready" >/dev/null; then
      echo "Rollback completed successfully." >&2
      return 0
    fi
    sleep 5
  done

  echo "Rollback images started, but the public readiness check still fails." >&2
  return 1
}

write_release "$NEW_IMAGE_TAG"
mkdir -p "$BACKUP_DIR"
chmod 700 "$BACKUP_DIR"

echo "Pulling immutable application images for $NEW_IMAGE_TAG."
compose pull api web migrations

echo "Starting PostgreSQL and waiting for readiness."
compose up --detach --wait --wait-timeout 120 db

readonly BACKUP_FILE="$BACKUP_DIR/loyeris-$(date -u +%Y%m%dT%H%M%SZ).dump"
echo "Creating database backup $BACKUP_FILE."
compose exec --no-TTY db sh -c \
  'pg_dump --username "$POSTGRES_USER" --dbname "$POSTGRES_DB" --format=custom' \
  > "$BACKUP_FILE"
chmod 600 "$BACKUP_FILE"
find "$BACKUP_DIR" -type f -name 'loyeris-*.dump' -mtime +14 -delete

echo "Applying database migrations."
if ! compose up --no-deps --force-recreate --abort-on-container-exit --exit-code-from migrations migrations; then
  echo "Database migration failed." >&2
  rollback_images || true
  exit 1
fi

echo "Starting the API and waiting for its readiness check."
if ! compose up --detach --no-deps --wait --wait-timeout 180 api; then
  echo "The API failed to become ready." >&2
  rollback_images || true
  exit 1
fi

echo "Starting the Caddy web gateway."
if ! compose up --detach --no-deps web; then
  echo "The web gateway failed to start." >&2
  rollback_images || true
  exit 1
fi

for _ in {1..30}; do
  if curl --fail --silent --show-error "https://$APP_DOMAIN/api/health/ready" >/dev/null; then
    echo "Loyeris $NEW_IMAGE_TAG is healthy at https://$APP_DOMAIN."
    exit 0
  fi
  sleep 5
done

echo "The public readiness check failed after deployment." >&2
rollback_images || true
exit 1
