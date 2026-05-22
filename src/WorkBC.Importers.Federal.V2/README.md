# WorkBC.Importers.Federal.V2

PHP 8.3 importer that pulls BC job postings from the Government of Canada
**Job Bank XML feed** (`https://www.jobbank.gc.ca/xmlfeed`) and syncs them
into the WorkBC PostgreSQL database. Packaged as a Docker image and scheduled
via a Kubernetes CronJob — runs once per day.

V2 owns **stage 1** of the federal pipeline (the **importer**). Stage 2 (the
**indexer** that reads `ImportedJobsFederal` and writes to Elasticsearch) is a
separate service.

## Where it runs in production

| Resource | Identifier |
|---|---|
| Image | `${ECR}/jb-importers-federal-v2:<sha>` |
| Built by | `.github/workflows/build*.yml` (compose service `importers-federal-v2`) |
| Tagged by | `.github/workflows/tag*.yml` (test → prod promotion) |
| Wired by | `.github/workflows/deploy-*.{yml,yaml}` (`IMAGE12`) |
| Cron | `cronjob/jb-importer-indexer`, container `federal-importer` |

## Project Structure

```
WorkBC.Importers.Federal.V2/
├── src/
│   ├── Api/FederalApiClient.php         # Guzzle HTTP client for the jobbank.gc.ca XML feed.
│   │                                    #   Concurrent EN+FR fetches via Guzzle Pool.
│   │                                    #   Retries once after a 5s pause on transport failure.
│   ├── Config/AppConfig.php             # Strongly-typed env-var configuration.
│   ├── Service/JobImportService.php     # The 6-stage pipeline orchestrator.
│   ├── Xml/XmlJobMapper.php             # XML → Jobs/JobVersions field mapping (NOC validation,
│   │                                    #   location lookup, salary calc, work-period flags).
│   └── import.php                       # CLI entry point.
├── Dockerfile                           # Multi-stage Alpine build (PHP 8.3, postgres-libs,
│                                        # libxml2). Runs ENTRYPOINT ["php", "src/import.php"].
├── .dockerignore
├── .env.example                         # Template — copy to .env for local dev.
├── .gitignore
├── composer.json                        # monolog, guzzle, vlucas/phpdotenv.
├── php.ini                              # 1024M memory_limit, opcache, realpath cache.
└── README.md
```

The companion shell image is `src/php-cli.Dockerfile` (one level up): a
single image that bakes both `WorkBC.Importers.Federal.V2` and
`WorkBC.Importers.Innovibe` plus bash + psql, used as the operator shell pod
in production.

## Import Flow

The pipeline runs as a single process. Stages are sequential; if a stage hits
the per-run error budget (`MAX_ERRORS`, default 25) it logs the breach and
skips ahead.

1. **List** — fetch the BC job index from
   `${FEDERAL_XML_ROOT}/en/bc?includevirtual=true` to get every
   `{jobs_id, file_update_date}` pair currently advertised in BC.
2. **Diff vs. staging** — load `"ImportedJobsFederal"` and compute the work
   sets:
   - **insert**: ids in feed but not in staging
   - **update**: ids in both, but with a different `file_update_date`
3. **Per-job XML fetch** — for each insert/update target, GET
   `${FEDERAL_XML_ROOT}/en/{id}.xml` AND `${FEDERAL_XML_ROOT}/fr/{id}.xml`
   in parallel via Guzzle Pool. Failed responses (502 proxy errors are common)
   are retried once after a 5s sleep.
4. **Persist staging** — upsert into `"ImportedJobsFederal"`:
   - Full XML in `JobPostEnglish` / `JobPostFrench`
   - `ApiDate = file_update_date` from the feed
   - `DisplayUntil` parsed from the per-job XML's `<display_until>`
   - `ReIndexNeeded = TRUE` so the downstream indexer picks the row up
   - For brand-new ids, also insert into `"JobIds"` first (FK target,
     `JobSourceId = 1` = Federal)
5. **Purge** — drop expired (`DisplayUntil < now`) and feed-vanished rows
   from `"ImportedJobsFederal"`, archiving them to `"ExpiredJobs"`. Capped at
   1000 rows per run per pass to avoid a runaway purge if jobbank's feed
   blips.
6. **Sync new → Jobs** — for each new staging row whose `DisplayUntil > now`,
   convert to a `"Jobs"` row (Title, City, NOC2016, NOC2021, IndustryId,
   Salary, SalarySummary, PositionsAvailable, DatePosted, ExpireDate,
   LocationId, work-period flags FT/PT/Permanent/Temporary/Casual/Seasonal)
   and create version 1 in `"JobVersions"`.
7. **Sync updates → Jobs** — for each staging row whose `DateLastImported`
   differs from the corresponding `Jobs` row (or whose `IsActive = FALSE`,
   or every row when `--reimport`), refresh the `Jobs` row. If NOC, location,
   positions, posted date, or active flag changed, close the current
   `JobVersions` row and insert a new version.
8. **Deactivate orphans** — any active federal job in `"Jobs"` whose id is no
   longer present in `"ImportedJobsFederal"` is marked `IsActive = FALSE` and
   gets a closing `JobVersions` row.

After the run finishes, the importer prints a summary line:

```
==== IMPORTER FINISHED in 32.6s — stagingInserted=20 stagingUpdated=20
     alreadyExpired=0 errors=0 jobsInserted=20 jobsUpdated=20
     jobsDeactivated=2000 ====
```

## Database (existing schema — no migrations owned here)

| Table                  | Purpose                                                         |
|------------------------|-----------------------------------------------------------------|
| `"ImportedJobsFederal"` | Staging — raw EN/FR XML in `JobPostEnglish` / `JobPostFrench`. `ReIndexNeeded` flag tells the indexer to re-emit. |
| `"Jobs"`               | Canonical, denormalised job records consumed by the API.        |
| `"JobVersions"`        | Version history (NOC / location / positions / active changes). One row per snapshot; `IsCurrentVersion` points at the live record. |
| `"ExpiredJobs"`        | Archived expired jobs. The indexer drains this to remove ES docs. |
| `"DeletedJobs"`        | Blacklist (read-only).                                          |
| `"JobIds"`             | Master ID list — FK target for `ImportedJobsFederal`, `Jobs`, `ExpiredJobs`. |
| `"NocCodes"`           | Validation lookup for NOC 2016 codes.                           |
| `"NocCodes2021"`       | Validation lookup for NOC 2021 codes.                           |
| `"Locations"`          | City → LocationId resolution; `IsDuplicate` rows drive the federal disambiguation labels (e.g. "Mill Bay - Vancouver Island / Coast"). |
| `"GeocodedLocationCache"` | Postal-code → City/Province lookup used for virtual jobs.    |
| `"SystemSettings"`     | Read-only — `shared.settings.minimumWage` filters out below-minimum salaries. |

This importer does **not** create or alter tables. Database migrations are
owned by `WorkBC.Admin` / `EFMigrationRunner`.

## Configuration

All settings come from environment variables. Copy `.env.example` → `.env` for
local development; in Kubernetes use a ConfigMap / Secret.

```bash
cp .env.example .env
# Edit — at a minimum set DB_USER, DB_PASSWORD, FEDERAL_AUTH_COOKIE.
```

### Required

| Var | Notes |
|---|---|
| `DB_HOST`, `DB_PORT`, `DB_NAME`, `DB_USER`, `DB_PASSWORD` | Postgres connection. The DB user needs write access to the tables listed above. |
| `FEDERAL_AUTH_COOKIE` |

### Optional

| Var | Default | Notes |
|---|---|---|
| `FEDERAL_XML_ROOT` | `https://www.jobbank.gc.ca/xmlfeed` | Override only if jobbank moves the feed. |
| `FEDERAL_PROVINCE_PATH` | `/en/bc` | Province slug used to list jobs. |
| `FEDERAL_INCLUDE_VIRTUAL` | `true` | Append `?includevirtual=true` so virtual postings surface. |
| `PROXY_USE` | `false` | If `true`, route outbound traffic via the corporate forward proxy. |
| `PROXY_HOST`, `PROXY_PORT` | — | Forward-proxy host/port. |
| `PROXY_IGNORE_SSL_ERRORS` | `true` | Disable cert verification for the proxy chain. |
| `MAX_JOBS` | `20000` | Cap on inserts + updates per run. Overridden by the `--maxjobs` CLI flag. |
| `MAX_ERRORS` | `25` | If insert/update errors exceed this, the affected stage aborts. |
| `HTTP_TIMEOUT` | `30` | Per-request Guzzle timeout in seconds. |
| `RETRY_DELAY_MS` | `5000` | Pause before the single retry on transport failure. |
| `LOG_LEVEL` | `INFO` | `DEBUG`, `INFO`, `WARNING`, `ERROR`, `CRITICAL`. |

## CLI Flags

| Flag | Purpose |
|---|---|
| `--reimport` / `-r` | Skip Stages 1–5 (no calls to jobbank.gc.ca). Just re-sync `ImportedJobsFederal` → `Jobs`. In `--reimport` mode Stage 7 also runs against **every** matching row, not just changed ones — useful after a mapping bug fix. |
| `--maxjobs N` | Cap inserts AND updates per run (each capped independently). Defaults to `MAX_JOBS` env var. Use small values (e.g. `--maxjobs=50`) for smoke tests after a deploy. |

## Build & Run

### Docker (standalone)

```bash
# Build the image
docker build -t workbc/importers-federal:v2 .

# Daily run (matches what the cron does)
docker run --rm --env-file .env workbc/importers-federal:v2

# Bounded test run after a deploy
docker run --rm --env-file .env workbc/importers-federal:v2 --maxjobs=50

# Re-sync staging → Jobs only (no jobbank network calls)
docker run --rm --env-file .env workbc/importers-federal:v2 --reimport
```

### Docker Compose (integrated with the WorkBC dev stack)

From the `src/` directory:

```bash
docker compose build importers-federal-v2
docker compose run --rm importers-federal-v2
docker compose run --rm importers-federal-v2 --maxjobs=50
docker compose run --rm importers-federal-v2 --reimport
```

The compose service `importers-federal-v2` has `scale: 0` — it is built but
not started by `docker compose up`. Use `docker compose run` for one-off
invocations, exactly like the cron does in production.

### Production shell — kubectl exec into the `php-cli` pod

The shared shell pod `php-cli` (built from `src/php-cli.Dockerfile`) bakes
this importer at `/app/workbc-importers-federal-v2`. Operators land on it
via:

```bash
kubectl exec -it <php-cli-pod-name> -- bash
# Sample-commands banner prints automatically. Then:
cd /app/workbc-importers-federal-v2
php src/import.php --maxjobs=50
```

This is the recommended path for manual runs in test/prod — the cron pod is
short-lived and exits as soon as the daily import finishes.

### Local PHP (development only)

```bash
composer install
cp .env.example .env
# Fill in DB_USER, DB_PASSWORD, FEDERAL_AUTH_COOKIE
php src/import.php
php src/import.php --reimport
```

## Verifying a manual run via Postgres

After running an import manually you usually want to confirm data landed and
the indexer flag was set. From the `php-cli` pod (which has `psql` baked
in) — or any host with psql:

```sql
-- High-level counts
SELECT (SELECT COUNT(*) FROM "ImportedJobsFederal") AS staging,
       (SELECT COUNT(*) FROM "Jobs" WHERE "JobSourceId" = 1
                                       AND "IsActive" = TRUE) AS active_federal,
       (SELECT COUNT(*) FROM "ImportedJobsFederal"
                                       WHERE "ReIndexNeeded" = TRUE) AS pending_reindex;

-- Rows just touched by the run
SELECT "JobId", LENGTH("JobPostEnglish") AS en_len,
       LENGTH("JobPostFrench") AS fr_len, "DisplayUntil"
FROM "ImportedJobsFederal"
WHERE "DateLastImported" > NOW() - INTERVAL '5 minutes'
LIMIT 10;
```

After a manual import you typically also want to **re-run the indexer** so
Elasticsearch reflects the new staging rows — that's a separate
`indexers-federal` cron / pod.

## Troubleshooting

| Symptom | Likely cause / fix |
|---|---|
| `0 federal jobs found in BC or virtual` | `FEDERAL_AUTH_COOKIE` missing or wrong format. The feed returns HTTP 200 + a 403 HTML error page, which the parser sees as 0 jobs. Check the cookie has the `xmlfeedaccess=` prefix. |
| `Failed to connect to PostgreSQL` | `DB_HOST` unreachable from the pod / container. From the `php-cli` shell, try `psql "host=$DB_HOST port=$DB_PORT user=$DB_USER dbname=$DB_NAME"` to isolate. |
| Mass deactivations on every run (e.g. `jobsDeactivated=2000`) | Expected on the first few runs after a long outage — staging gets purged 1000/run while `Jobs` still has older entries. After ~3–4 daily runs the two converge. |
| `jobsInserted=0 jobsUpdated=0` even though `stagingInserted > 0` | Race with another concurrent run, or `Jobs.JobId` already covers the staging ids (run was idempotent). Confirm with the SQL above. |
| `MAX_ERRORS_25_EXCEEDED` | The per-job XML endpoint returned malformed responses or 502s for ≥25 ids in a row. Check jobbank.gc.ca status and the corporate proxy. The next run will pick up where this one stopped. |
| Indexer doesn't see new staging rows | This importer sets `ReIndexNeeded = TRUE` on every insert/update — confirm with the SQL above. If `ReIndexNeeded` rows pile up indefinitely, the indexer's clear-flag step is the culprit, not this importer. |
