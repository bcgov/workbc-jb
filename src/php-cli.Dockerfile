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

FROM php:8.3-cli-alpine AS federal-indexer-app
RUN apk add --no-cache postgresql-dev libxml2-dev autoconf gcc g++ make unzip \
    && docker-php-ext-install pdo_pgsql dom simplexml
COPY --from=composer:2 /usr/bin/composer /usr/bin/composer
WORKDIR /app
COPY WorkBC.Indexers.Federal.V2/composer.json ./
RUN composer install --no-dev --no-interaction --optimize-autoloader --prefer-dist
COPY WorkBC.Indexers.Federal.V2/src/ src/
COPY WorkBC.Indexers.Federal.V2/resources/ resources/

FROM php:8.3-cli-alpine
RUN apk add --no-cache bash postgresql-client postgresql-libs libxml2 tzdata \
    && apk add --no-cache --virtual .bd postgresql-dev libxml2-dev autoconf gcc g++ make \
    && docker-php-ext-install pdo_pgsql dom simplexml \
    && apk del .bd \
    && cp /usr/share/zoneinfo/America/Vancouver /etc/localtime \
    && echo "America/Vancouver" > /etc/timezone \
    && rm -rf /var/cache/apk/* /tmp/*

COPY --from=federal-app          /app /app/workbc-importers-federal-v2
COPY --from=innovibe-app         /app /app/workbc-importers-innovibe
COPY --from=federal-indexer-app  /app /app/workbc-indexers-federal-v2

COPY WorkBC.Importers.Federal.V2/php.ini /usr/local/etc/php/conf.d/php.ini

RUN cat > /root/.bashrc <<'EOF'
cat <<'B'
─────────────────────────────────────────────────────────────────────
  WorkBC PHP CLI  ·  PHP 8.3
  Federal V2 + Innovibe importers + Federal indexer in one shell.
─────────────────────────────────────────────────────────────────────

  Federal V2 importer:
    cd /app/workbc-importers-federal-v2
    php src/import.php                  # daily diff import
    php src/import.php --reimport       # re-sync staging → Jobs only
    php src/import.php --maxjobs=50     # bounded test run

  Innovibe importer:
    cd /app/workbc-importers-innovibe
    php src/import.php                  # yesterday + today
    php src/import.php --bulk           # full re-import

  Federal indexer (staging → Elasticsearch):
    cd /app/workbc-indexers-federal-v2
    php src/index.php                   # index pending jobs + purge
    php src/index.php -r                # recreate indexes + reindex all
    php src/index.php -o                # close/reopen to reload synonyms
    php src/index.php -d                # diff ES vs Jobs (debug)

  After manually running an import you usually need to re-index.

  Postgres:
    psql "host=$DB_HOST port=$DB_PORT user=$DB_USER dbname=$DB_NAME"

─────────────────────────────────────────────────────────────────────
B
EOF

WORKDIR /app
ENV TERM=xterm-256color

ENTRYPOINT ["sleep", "infinity"]
