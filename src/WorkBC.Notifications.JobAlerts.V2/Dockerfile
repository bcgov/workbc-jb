# ── Build stage ──────────────────────────────────────────────────
# dom/simplexml/xml are compiled into the official image; only pdo_pgsql needs building.
FROM php:8.3.37-cli-bookworm AS build

RUN apt-get update \
    && apt-get install -y --no-install-recommends libpq-dev unzip \
    && docker-php-ext-install pdo_pgsql \
    && rm -rf /var/lib/apt/lists/*

COPY --from=composer:2 /usr/bin/composer /usr/bin/composer

WORKDIR /app
COPY composer.json ./
RUN composer install --no-dev --no-interaction --optimize-autoloader --prefer-dist
COPY src/ src/
COPY resources/ resources/

# ── Runtime stage ────────────────────────────────────────────────
FROM php:8.3.37-cli-bookworm

# libpq5 is named explicitly so it survives the --auto-remove of libpq-dev.
# tzdata is (re)installed so the OS zoneinfo is bookworm's 2026b-0+deb12u1 or newer;
# 2026b is the release that encodes B.C.'s permanent UTC-7 from 2026-11-01.
RUN apt-get update \
    && apt-get install -y --no-install-recommends libpq5 libpq-dev tzdata \
    && docker-php-ext-install pdo_pgsql \
    && apt-get purge -y --auto-remove libpq-dev \
    && rm -rf /var/lib/apt/lists/* \
    && ln -snf /usr/share/zoneinfo/America/Vancouver /etc/localtime \
    && echo "America/Vancouver" > /etc/timezone
# PHP ignores the OS tzdata and ships its own copy; every 8.3.x release (8.3.37 included)
# bundles 2026a, which predates B.C.'s move to permanent UTC-7 (tzdata 2026b, effective
# 2026-11-01 02:00). The PECL timezonedb extension replaces the bundled copy so
# America/Vancouver stays -07. 2026.6 = tzdata 2026f.
RUN pecl install timezonedb-2026.6 \
    && docker-php-ext-enable timezonedb \
    && rm -rf /tmp/pear
ENV TZ=America/Vancouver

WORKDIR /app
COPY --from=build /app/vendor vendor/
COPY --from=build /app/src src/
COPY --from=build /app/resources resources/
COPY php.ini /usr/local/etc/php/conf.d/php.ini

ENTRYPOINT ["php", "src/notify.php"]
