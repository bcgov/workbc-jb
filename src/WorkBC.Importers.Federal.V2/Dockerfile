# ── Build stage ──────────────────────────────────────────────────
FROM php:8.3.36-cli-alpine AS build

RUN apk add --no-cache postgresql-dev libxml2-dev autoconf gcc g++ make unzip \
    && docker-php-ext-install pdo_pgsql dom simplexml

COPY --from=composer:2 /usr/bin/composer /usr/bin/composer

WORKDIR /app
COPY composer.json ./
RUN composer install --no-dev --no-interaction --optimize-autoloader --prefer-dist
COPY src/ src/

# ── Runtime stage ────────────────────────────────────────────────
FROM php:8.3.36-cli-alpine

RUN apk add --no-cache postgresql-libs libxml2 tzdata \
    && apk add --no-cache --virtual .bd postgresql-dev libxml2-dev \
    && docker-php-ext-install pdo_pgsql dom simplexml \
    && apk del .bd \
    && cp /usr/share/zoneinfo/America/Vancouver /etc/localtime \
    && echo "America/Vancouver" > /etc/timezone
# PHP ignores the OS tzdata and ships its own copy; every 8.3.x release bundles 2026a,
# which predates B.C.'s move to permanent UTC-7 (tzdata 2026b, effective 2026-11-01 02:00).
# The PECL timezonedb extension replaces the bundled copy so America/Vancouver stays -07.
RUN apk add --no-cache --virtual .tzdb-build $PHPIZE_DEPS \
    && pecl install timezonedb-2026.5 \
    && docker-php-ext-enable timezonedb \
    && apk del .tzdb-build
ENV TZ=America/Vancouver

WORKDIR /app
COPY --from=build /app/vendor vendor/
COPY --from=build /app/src src/
COPY php.ini /usr/local/etc/php/conf.d/php.ini

ENTRYPOINT ["php", "src/import.php"]
