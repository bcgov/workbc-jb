# WorkBC.Importers.Federal.V2

PHP rewrite of the C# `WorkBC.Importers.Federal` console app. Pulls job postings from
the Government of Canada **Job Bank XML feed** (`jobbank.gc.ca/xmlfeed`) and syncs
them into the existing PostgreSQL database used by the WorkBC Job Board. Packaged as a
Docker image, scheduled the same way as `WorkBC.Importers.Innovibe` (Kubernetes CronJob).

The legacy C# importer at `src/WorkBC.Importers.Federal/` is unchanged and continues to
be built and deployed in parallel; this V2 service is a drop-in candidate for the same
slot in the cron pipeline. Cluster operators can flip the cron-job image from
`jb-importers-federal:<sha>` to `jb-importers-federal-v2:<sha>` once the rewrite has
been verified.

## Project Structure

```
WorkBC.Importers.Federal.V2/
├── src/
│   ├── Api/FederalApiClient.php         # HTTP client for the jobbank.gc.ca XML feed
│   ├── Config/AppConfig.php             # Env-var configuration
│   ├── Service/JobImportService.php     # Import / purge / sync / version logic
│   ├── Xml/XmlJobMapper.php             # XML → Jobs-table field mapping (mirrors XmlParsingServiceFederal subset)
│   └── import.php                       # CLI entry point
├── Dockerfile                           # Multi-stage Alpine build (PHP 8.3)
├── Dockerfile.cli                       # `sleep infinity` variant for kubectl exec
├── .dockerignore
├── .env.example                         # Template — copy to .env
├── .gitignore
├── composer.json
├── php.ini
└── README.md
```

## Import Flow (mirrors the legacy C# implementation)

1. **List** — fetch the BC job index from `${FEDERAL_XML_ROOT}/en/bc?includevirtual=true`
   to get every `{jobs_id, file_update_date}` pair currently advertised in BC.
2. **Diff vs. staging** — load `"ImportedJobsFederal"` and compute insert/update sets:
   - new jobs (id not in staging)
   - changed jobs (`ApiDate` differs from `file_update_date`)
3. **Per-job fetch** — for each insert/update, GET both
   `${FEDERAL_XML_ROOT}/en/{id}.xml` and `${FEDERAL_XML_ROOT}/fr/{id}.xml` in
   parallel (Guzzle pool). Retries once after a 5s pause on transport failure.
4. **Persist staging** — upsert into `"ImportedJobsFederal"` with full XML text in
   `JobPostEnglish` / `JobPostFrench`, set `DisplayUntil` from `display_until`, and
   add the id to `"JobIds"` for new jobs (`JobSourceId = 1`).
5. **Purge** — drop expired (`DisplayUntil < now`) or vanished rows from
   `"ImportedJobsFederal"` and insert/refresh them in `"ExpiredJobs"`.
6. **Sync new** — convert each new staging row to a `"Jobs"` row (Title, City,
   NOC codes, salary, location, work-period flags, etc.) and create version 1 in
   `"JobVersions"`.
7. **Sync updates** — refresh `"Jobs"` rows whose `DateLastImported` changed; if the
   NOC, location, positions, posting date, or active flag moved, increment
   `JobVersions`.
8. **Deactivate** — any active federal job in `"Jobs"` whose id is no longer present
   in `"ImportedJobsFederal"` is marked `IsActive = FALSE` and gets a closing version.

The downstream `WorkBC.Indexers.Federal` (still C#) continues to read
`"ImportedJobsFederal"` and produce Elasticsearch documents — V2 is a drop-in
replacement for stage 1 only.

## Database (existing — no schema changes)

| Table                  | Purpose                                                         |
|------------------------|-----------------------------------------------------------------|
| `"ImportedJobsFederal"` | Staging — raw XML in `JobPostEnglish` / `JobPostFrench`        |
| `"Jobs"`               | Canonical job records                                           |
| `"JobVersions"`        | Version history (NOC / location / positions / active changes)   |
| `"ExpiredJobs"`        | Archived expired jobs                                           |
| `"DeletedJobs"`        | Blacklist (read-only)                                           |
| `"JobIds"`             | Master ID list                                                  |
| `"NocCodes2021"`       | Validation lookup for NOC 2021 codes                            |
| `"Locations"`          | City → LocationId resolution                                    |

## Configuration

All settings come from environment variables. Copy `.env.example` → `.env` for local
development; in Kubernetes use a ConfigMap / Secret.

```bash
cp .env.example .env
# Edit — at a minimum set DB_USER, DB_PASSWORD, FEDERAL_AUTH_COOKIE
```

`FEDERAL_AUTH_COOKIE` corresponds to `FederalSettings:AuthCookie` from the legacy
`appsettings.json`. Without it, jobbank.gc.ca rejects requests from non-whitelisted
hosts.

## CLI Flags

| Flag           | Purpose                                                                     |
|----------------|-----------------------------------------------------------------------------|
| `--reimport` / `-r` | Skip the per-job XML fetch and only re-run the staging→Jobs sync.           |
| `--maxjobs N`  | Cap the number of inserts/updates per run. Defaults to `MAX_JOBS` env var.  |

## Build & Run

### Docker (recommended)

```bash
# Build the image
docker build -t workbc/importers-federal:v2 .

# Daily run
docker run --rm --env-file .env workbc/importers-federal:v2

# Re-sync staging → Jobs (no network calls to jobbank)
docker run --rm --env-file .env workbc/importers-federal:v2 --reimport
```

### Docker Compose

From the `src/` directory:

```bash
docker compose build importers-federal-v2
docker compose run --rm importers-federal-v2
docker compose run --rm importers-federal-v2 --reimport
```

### Local PHP (development only)

```bash
composer install
cp .env.example .env
# Fill in .env
php src/import.php
php src/import.php --reimport
```

## Migration from the C# importer

| Legacy (`WorkBC.Importers.Federal`)            | V2 (`WorkBC.Importers.Federal.V2`)         |
|------------------------------------------------|---------------------------------------------|
| `appsettings.json` `FederalSettings:FederalJobXmlRoot` | `FEDERAL_XML_ROOT` env var                  |
| `appsettings.json` `FederalSettings:AuthCookie`        | `FEDERAL_AUTH_COOKIE` env var               |
| `appsettings.json` `ProxySettings:*`                   | `PROXY_*` env vars                          |
| `--maxjobs` CLI option                                  | `--maxjobs` (same) or `MAX_JOBS` env var    |
| `--reimport` / `-r`                                     | `--reimport` / `-r` (same)                  |
| `--migrate` / `-m`                                      | **Removed.** Migrations are owned by `WorkBC.Admin` / `EFMigrationRunner`. |
