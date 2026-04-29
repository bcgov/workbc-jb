FROM php:8.3-cli-alpine AS federal-app
RUN apk add --no-cache postgresql-dev libxml2-dev autoconf gcc g++ make unzip \
    && docker-php-ext-install pdo_pgsql dom simplexml
COPY --from=composer:2 /usr/bin/composer /usr/bin/composer
WORKDIR /app
COPY WorkBC.Importers.Federal.V2/composer.json ./
RUN composer install --no-dev --no-interaction --optimize-autoloader --prefer-dist
COPY WorkBC.Importers.Federal.V2/src/ src/

FROM php:8.3-cli-alpine AS innovibe-app
RUN apk add --no-cache postgresql-dev autoconf gcc g++ make unzip \
    && docker-php-ext-install pdo_pgsql
COPY --from=composer:2 /usr/bin/composer /usr/bin/composer
WORKDIR /app
COPY WorkBC.Importers.Innovibe/composer.json ./
RUN composer install --no-dev --no-interaction --optimize-autoloader --prefer-dist
COPY WorkBC.Importers.Innovibe/src/ src/

FROM php:8.3-cli-alpine
RUN apk add --no-cache bash postgresql-client postgresql-libs libxml2 tzdata \
    && apk add --no-cache --virtual .bd postgresql-dev libxml2-dev autoconf gcc g++ make \
    && docker-php-ext-install pdo_pgsql dom simplexml \
    && apk del .bd \
    && cp /usr/share/zoneinfo/America/Vancouver /etc/localtime \
    && echo "America/Vancouver" > /etc/timezone \
    && rm -rf /var/cache/apk/* /tmp/*

COPY --from=federal-app  /app /app/workbc-importers-federal-v2
COPY --from=innovibe-app /app /app/workbc-importers-innovibe

COPY WorkBC.Importers.Federal.V2/php.ini /usr/local/etc/php/conf.d/zz-workbc.ini

RUN cat > /root/.bashrc <<'EOF'
cat <<'B'
─────────────────────────────────────────────────────────────────────
  WorkBC PHP CLI  ·  PHP 8.3
  Federal V2 (jobbank.gc.ca) + Innovibe importers in one shell.
─────────────────────────────────────────────────────────────────────

  Federal V2:
    cd /app/workbc-importers-federal-v2
    php src/import.php                  # daily diff import
    php src/import.php --reimport       # re-sync staging → Jobs only
    php src/import.php --maxjobs=50     # bounded test run

  Innovibe:
    cd /app/workbc-importers-innovibe
    php src/import.php                  # yesterday + today
    php src/import.php --bulk           # full re-import

  After manually running an import you usually need to re-index.

  Postgres:
    psql "host=$DB_HOST port=$DB_PORT user=$DB_USER dbname=$DB_NAME"

─────────────────────────────────────────────────────────────────────
B
EOF

WORKDIR /app
ENV TERM=xterm-256color

ENTRYPOINT ["sleep", "infinity"]
