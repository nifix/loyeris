# syntax=docker/dockerfile:1.7

FROM node:24-alpine AS build
WORKDIR /app

RUN corepack enable && corepack prepare yarn@1.22.22 --activate
COPY package.json yarn.lock ./
RUN yarn install --frozen-lockfile --non-interactive

COPY . .
RUN yarn build

FROM caddy:2.10-alpine AS runtime
COPY Caddyfile /etc/caddy/Caddyfile
COPY --from=build /app/dist/loyeris-app/browser /srv

EXPOSE 80 443
