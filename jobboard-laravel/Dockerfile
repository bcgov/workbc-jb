# syntax=docker/dockerfile:1
#
# WorkBC Job Board — Laravel web container.
#
# Replaces the .NET WorkBC.Web container in the `web` slot of the jb2
# deployment on dev2. It must be drop-in compatible with that slot, because
# the pipeline only ever does `kubectl set image ... web=<image>` — the pod's
# port, probes and envFrom are unchanged. Hence port 8081, matching
# WorkBC.Web/Dockerfile's EXPOSE.
#
# Base image line matches the repo's other PHP images (php:8.3-*-alpine), and
# the container runs as root like every sibling container here — see the
# Dockerfile discussion in docs; deliberately not diverging on user for one
# image.

ARG PHP_VERSION=8.3

# ───────────────────────────── base: PHP + extensions ─────────────────────────
# Shared by the composer stage and the runtime so the extension set is defined
# exactly once and cannot drift between them.
FROM php:${PHP_VERSION}-fpm-alpine AS base

# The runtime libraries are installed PERMANENTLY and separately from the build
# deps. `apk del icu-dev` would otherwise take icu-libs with it as an orphaned
# dependency, leaving extensions whose .so files exist but cannot load — which
# surfaces confusingly as "ext-intl is missing" despite the .ini being present.
RUN set -eux; \
    apk add --no-cache \
        icu-libs \
        libzip \
        libpq; \
    apk add --no-cache --virtual .build-deps \
        $PHPIZE_DEPS postgresql-dev icu-dev libzip-dev linux-headers; \
    docker-php-ext-install -j"$(nproc)" \
        pdo_pgsql \
        intl \
        zip \
        opcache \
        pcntl; \
    pecl install redis; \
    docker-php-ext-enable redis; \
    apk del .build-deps; \
    rm -rf /tmp/pear; \
    php -m | grep -qE '^intl$' && php -m | grep -qE '^zip$' && php -m | grep -qE '^redis$' && php -m | grep -qE '^pdo_pgsql$'
# pdo_pgsql  → the existing Jobs/AspNetUsers database
# redis      → sessions, cache and queue (REDIS_CLIENT=phpredis)
# intl       → locale-aware formatting; Laravel warns without it
# pcntl      → required by queue workers (ACCT-4 later); harmless here
# opcache    → configured in docker/php.ini

# ───────────────────────────── vendor: composer install ───────────────────────
FROM base AS vendor
COPY --from=composer:2 /usr/bin/composer /usr/bin/composer
WORKDIR /app

# Copy manifests first so this layer caches unless dependencies actually change.
COPY composer.json composer.lock ./
RUN composer install \
        --no-dev --no-scripts --no-autoloader \
        --prefer-dist --no-interaction --no-progress

COPY . .
RUN composer dump-autoload --optimize --no-dev --classmap-authoritative

# ───────────────────────────── assets: Vite build ─────────────────────────────
# Node exists only here; it never reaches the runtime image.
FROM node:22-alpine AS assets
WORKDIR /app
COPY package.json package-lock.json ./
RUN npm ci --no-audit --no-fund
COPY resources ./resources
COPY vite.config.js ./
RUN npm run build

# ───────────────────────────── runtime ────────────────────────────────────────
FROM base AS runtime

ARG COMMIT_SHA=unknown
ARG RUN_NUMBER=unknown
ARG BUILD_DATE=unknown
# Surfaced so a running container can report which build it came from.
ENV APP_COMMIT_SHA=${COMMIT_SHA} \
    APP_RUN_NUMBER=${RUN_NUMBER} \
    APP_BUILD_DATE=${BUILD_DATE}

RUN apk add --no-cache nginx supervisor

WORKDIR /var/www/html

COPY --from=vendor /app ./
COPY --from=assets /app/public/build ./public/build

COPY docker/nginx.conf       /etc/nginx/nginx.conf
COPY docker/php-fpm.conf     /usr/local/etc/php-fpm.d/zz-app.conf
COPY docker/php.ini          /usr/local/etc/php/conf.d/zz-app.ini
COPY docker/supervisord.conf /etc/supervisord.conf
COPY docker/entrypoint.sh    /usr/local/bin/entrypoint
RUN chmod +x /usr/local/bin/entrypoint

# Laravel writes compiled Blade views and the framework cache here. Sessions,
# cache and queue all live in Redis, so this is the only writable state needed.
# php-fpm workers run as www-data (a root pool is refused), so the paths Laravel
# writes must be owned by it.
RUN mkdir -p storage/framework/{cache/data,sessions,views} storage/logs bootstrap/cache \
 && chown -R www-data:www-data storage bootstrap/cache \
 && chmod -R 775 storage bootstrap/cache \
 && mkdir -p /run/nginx

EXPOSE 8081

# Deliberately /up, not /healthz: /up exercises PHP, so a dead php-fpm reports
# unhealthy. /healthz answers from nginx alone — during testing that made the
# container report "healthy" while PHP was crash-looping and serving nothing.
#
# For the k8s probes, use the same split:
#   readinessProbe → /up       (don't route traffic to a broken app)
#   livenessProbe  → /healthz  (only restart if nginx itself is wedged)
HEALTHCHECK --interval=30s --timeout=5s --start-period=30s --retries=3 \
    CMD wget -qO- http://127.0.0.1:8081/up >/dev/null 2>&1 || exit 1

ENTRYPOINT ["entrypoint"]
CMD ["supervisord", "-c", "/etc/supervisord.conf"]
