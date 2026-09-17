#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

# Debian 11 (bullseye) left LTS on 2026-08-31 and its security pool is being purged from
# deb.debian.org (apt-get install 404s), so this image must track the bookworm variant.
FROM mcr.microsoft.com/dotnet/runtime:6.0-bookworm-slim

# Install prerequisites
RUN apt-get update && apt-get install -y \
    wget \
    curl \
    unzip \
    ca-certificates \
    gnupg \
    lsb-release \
    tzdata \
    && rm -rf /var/lib/apt/lists/*

# Add PostgreSQL apt repo and install psql client v16
RUN wget -qO - https://www.postgresql.org/media/keys/ACCC4CF8.asc | tee /etc/apt/trusted.gpg.d/postgresql.asc \
    && echo "deb http://apt.postgresql.org/pub/repos/apt $(lsb_release -cs)-pgdg main" \
       > /etc/apt/sources.list.d/pgdg.list \
    && apt-get update \
    && apt-get install -y postgresql-client-16 \
    && rm -rf /var/lib/apt/lists/*

# Install AWS CLI v2
RUN curl "https://awscli.amazonaws.com/awscli-exe-linux-x86_64.zip" -o "awscliv2.zip" \
    && unzip awscliv2.zip \
    && ./aws/install \
    && rm -rf awscliv2.zip aws

# Verify installations
RUN psql --version && aws --version

RUN cp /usr/share/zoneinfo/America/Vancouver /etc/localtime && \
    echo "America/Vancouver" > /etc/timezone

# Federal & Innovibe indexers and the job-alert notifier are now PHP — they
# live in the php-cli (cli2) image, not here. This .NET operator shell keeps
# only the remaining .NET tools.
COPY --from=src_migration-runner /app /app/efmigrationrunner

# keep the container running indefinitely by using this entrypoint
ENTRYPOINT ["sleep", "infinity"]
