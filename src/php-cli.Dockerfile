# dom/simplexml/xml are compiled into the official image; only pdo_pgsql needs building.
FROM php:8.3.37-cli-bookworm AS federal-app
RUN apt-get update \
    && apt-get install -y --no-install-recommends libpq-dev unzip \
    && docker-php-ext-install pdo_pgsql \
    && rm -rf /var/lib/apt/lists/*
COPY --from=composer:2 /usr/bin/composer /usr/bin/composer
WORKDIR /app
COPY WorkBC.Importers.Federal.V2/composer.json ./
RUN composer install --no-dev --no-interaction --optimize-autoloader --prefer-dist
COPY WorkBC.Importers.Federal.V2/src/ src/

FROM php:8.3.37-cli-bookworm AS innovibe-app
RUN apt-get update \
    && apt-get install -y --no-install-recommends libpq-dev unzip \
    && docker-php-ext-install pdo_pgsql \
    && rm -rf /var/lib/apt/lists/*
COPY --from=composer:2 /usr/bin/composer /usr/bin/composer
WORKDIR /app
COPY WorkBC.Importers.Innovibe/composer.json ./
RUN composer install --no-dev --no-interaction --optimize-autoloader --prefer-dist
COPY WorkBC.Importers.Innovibe/src/ src/

FROM php:8.3.37-cli-bookworm AS federal-indexer-app
RUN apt-get update \
    && apt-get install -y --no-install-recommends libpq-dev unzip \
    && docker-php-ext-install pdo_pgsql \
    && rm -rf /var/lib/apt/lists/*
COPY --from=composer:2 /usr/bin/composer /usr/bin/composer
WORKDIR /app
COPY WorkBC.Indexers.Federal.V2/composer.json ./
RUN composer install --no-dev --no-interaction --optimize-autoloader --prefer-dist
COPY WorkBC.Indexers.Federal.V2/src/ src/
COPY WorkBC.Indexers.Federal.V2/resources/ resources/

FROM php:8.3.37-cli-bookworm AS innovibe-indexer-app
RUN apt-get update \
    && apt-get install -y --no-install-recommends libpq-dev unzip \
    && docker-php-ext-install pdo_pgsql \
    && rm -rf /var/lib/apt/lists/*
COPY --from=composer:2 /usr/bin/composer /usr/bin/composer
WORKDIR /app
COPY WorkBC.Indexers.Innovibe.V2/composer.json ./
RUN composer install --no-dev --no-interaction --optimize-autoloader --prefer-dist
COPY WorkBC.Indexers.Innovibe.V2/src/ src/
COPY WorkBC.Indexers.Innovibe.V2/resources/ resources/

FROM php:8.3.37-cli-bookworm AS jobalert-notifier-app
RUN apt-get update \
    && apt-get install -y --no-install-recommends libpq-dev unzip \
    && docker-php-ext-install pdo_pgsql \
    && rm -rf /var/lib/apt/lists/*
COPY --from=composer:2 /usr/bin/composer /usr/bin/composer
WORKDIR /app
COPY WorkBC.Notifications.JobAlerts.V2/composer.json ./
RUN composer install --no-dev --no-interaction --optimize-autoloader --prefer-dist
COPY WorkBC.Notifications.JobAlerts.V2/src/ src/
COPY WorkBC.Notifications.JobAlerts.V2/resources/ resources/

FROM php:8.3.37-cli-bookworm
# psql comes from apt.postgresql.org (client 16, same repo and major as dotnet-cli.Dockerfile)
# because bookworm itself only packages PostgreSQL 15. The pgdg repo is added before
# libpq-dev so the headers pdo_pgsql compiles against and the runtime libpq5 are the same
# build. libpq5 is named explicitly so it survives the --auto-remove of libpq-dev.
# tzdata is (re)installed so the OS zoneinfo is bookworm's 2026b-0+deb12u1 or newer;
# 2026b is the release that encodes B.C.'s permanent UTC-7 from 2026-11-01.
RUN apt-get update \
    && apt-get install -y --no-install-recommends tzdata \
    && curl -fsSL https://www.postgresql.org/media/keys/ACCC4CF8.asc -o /etc/apt/trusted.gpg.d/postgresql.asc \
    && echo "deb http://apt.postgresql.org/pub/repos/apt $(. /etc/os-release && echo "$VERSION_CODENAME")-pgdg main" \
       > /etc/apt/sources.list.d/pgdg.list \
    && apt-get update \
    && apt-get install -y --no-install-recommends libpq5 libpq-dev postgresql-client-16 \
    && docker-php-ext-install pdo_pgsql \
    && apt-get purge -y --auto-remove libpq-dev \
    && rm -rf /var/lib/apt/lists/* /tmp/* \
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

COPY --from=federal-app          /app /app/workbc-importers-federal-v2
COPY --from=innovibe-app         /app /app/workbc-importers-innovibe
COPY --from=federal-indexer-app  /app /app/workbc-indexers-federal-v2
COPY --from=innovibe-indexer-app /app /app/workbc-indexers-innovibe-v2
COPY --from=jobalert-notifier-app /app /app/workbc-notifications-jobalerts-v2

COPY WorkBC.Importers.Federal.V2/php.ini /usr/local/etc/php/conf.d/php.ini

RUN cat > /root/.bashrc <<'EOF'
cat <<'B'
─────────────────────────────────────────────────────────────────────
  WorkBC PHP CLI  ·  PHP 8.3
  Federal V2 + Innovibe importers, Federal & Innovibe indexers and the
  Job Alert notifier in one shell.
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

  Innovibe indexer (staging → Elasticsearch):
    cd /app/workbc-indexers-innovibe-v2
    php src/index.php                   # index pending jobs + purge
    php src/index.php -r                # recreate indexes + reindex all
    php src/index.php -d                # diff ES vs Jobs (debug)

  Job alert notifier (matches saved alerts in ES, emails job seekers):
    cd /app/workbc-notifications-jobalerts-v2
    php src/notify.php                  # send the alerts that are due today

  After manually running an import you usually need to re-index.

  Postgres:
    psql "host=$DB_HOST port=$DB_PORT user=$DB_USER dbname=$DB_NAME"

─────────────────────────────────────────────────────────────────────
B
EOF

WORKDIR /app
ENV TERM=xterm-256color

ENTRYPOINT ["sleep", "infinity"]
