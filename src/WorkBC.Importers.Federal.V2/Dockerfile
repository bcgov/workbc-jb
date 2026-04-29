# ── Build stage ──────────────────────────────────────────────────
FROM php:8.3-cli-alpine AS build

RUN apk add --no-cache postgresql-dev libxml2-dev autoconf gcc g++ make unzip \
    && docker-php-ext-install pdo_pgsql dom simplexml

COPY --from=composer:2 /usr/bin/composer /usr/bin/composer

WORKDIR /app
COPY composer.json ./
RUN composer install --no-dev --no-interaction --optimize-autoloader --prefer-dist
COPY src/ src/

# ── Runtime stage ────────────────────────────────────────────────
FROM php:8.3-cli-alpine

RUN apk add --no-cache postgresql-libs libxml2 tzdata \
    && apk add --no-cache --virtual .bd postgresql-dev libxml2-dev \
    && docker-php-ext-install pdo_pgsql dom simplexml \
    && apk del .bd \
    && cp /usr/share/zoneinfo/America/Vancouver /etc/localtime \
    && echo "America/Vancouver" > /etc/timezone

WORKDIR /app
COPY --from=build /app/vendor vendor/
COPY --from=build /app/src src/

ENTRYPOINT ["php", "src/import.php"]
