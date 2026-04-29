# WorkBC.Importers.Innovibe

PHP 8.3 cron job that fetches BC job postings from the **Innovibe API**
(`https://api-prod.jobs.innovibe.ca/api/v1`) and syncs them into the WorkBC
PostgreSQL database. Packaged as a Docker image and scheduled via a
Kubernetes CronJob.

By default each run fetches only jobs posted **yesterday and today**
(`postedFrom=yesterday`). Use the `--bulk` flag to fetch every job currently
advertised regardless of date — useful for backfills after schema changes or
prolonged outages.

The container also handles **expiry**: a separate Innovibe endpoint returns
the canonical list of expired job ids; matching jobs in our DB are archived
into `"ExpiredJobs"` and marked inactive in `"Jobs"`. This is authoritative —
we do **not** infer expiry from date heuristics.

## Where it runs in production

| Resource | Identifier |
|---|---|
| Image | `${ECR}/jb-importers-innovibe:<sha>` |
| Built by | `.github/workflows/build*.yml` (compose service `importers-innovibe`) |
| Tagged by | `.github/workflows/tag*.yml` (test → prod promotion) |
| Wired by | `.github/workflows/deploy-*.{yml,yaml}` (`IMAGE10`) |
| Cron | `cronjob/jb-importer-indexer`, container `wanted-importer` |

## Project Structure

```
WorkBC.Importers.Innovibe/
├── src/
│   ├── Api/InnovibeApiClient.php       # Cursor-paginated REST client (Guzzle).
│   │                                   #   Endpoints used:
│   │                                   #     /api/v1/jobs              — listing
│   │                                   #     /api/v1/jobs/expired/ids  — expiry feed
│   ├── Config/AppConfig.php            # Strongly-typed env-var configuration.
│   ├── Service/JobImportService.php    # Pipeline orchestrator:
│   │                                   #   import / upsert / expire / sync.
│   └── import.php                      # CLI entry point.
├── Dockerfile                          # Multi-stage Alpine build (PHP 8.3 + pdo_pgsql).
│                                       # Runs ENTRYPOINT ["php", "src/import.php"].
├── .dockerignore
├── .env.example                        # Template — copy to .env.
├── .gitignore
├── composer.json                       # monolog, guzzle, vlucas/phpdotenv.
├── php.ini                             # 512M memory_limit (handles --bulk).
└── README.md
```

The companion shell image is `src/php-cli.Dockerfile` (one level up): a
single image that bakes both `WorkBC.Importers.Innovibe` and
`WorkBC.Importers.Federal.V2` plus bash + psql, used as the operator shell
pod in production.

## Import Flow

The pipeline runs as a single process. Every step is idempotent — the
duplicate-hash check on `JobPostEnglish` makes re-runs safe.

1. **Fetch** — paginate through the Innovibe `/api/v1/jobs` API
   (cursor-based, `PAGE_SIZE` per page) with these filters:
   - `state=British Columbia`
   - `postedFrom=<yesterday>` (skipped when `--bulk`)
   - `includeExpired=false`
   - `includeNocUnmatched=<INCLUDE_NOC_UNMATCHED env, default false>`
2. **Filter** — skip jobs with no salary information at all (`salaryMin`,
   `salaryMax`, `salaryValue` all empty). Jobs without a salary cannot be
   shown on the public board because of provincial legislation.
3. **Upsert** — insert new jobs into `"ImportedJobsWanted"`; for existing
   rows, only update if the JSON payload hash differs (`H` marker = duplicate
   hash skip, `U` = updated, `I` = inserted, `S` = skipped).
4. **Mark seen** — refresh `"DateLastSeen"` on every returned job so we can
   tell which rows are still actively published.
5. **Expire** — call `/api/v1/jobs/expired/ids` (no date filter). Innovibe
   returns the canonical 3-month rolling window of expired ids, refreshed
   every 6 hours. Each id present in our DB is:
   - inserted/refreshed in `"ExpiredJobs"`
   - deleted from `"ImportedJobsWanted"`
   - marked `"IsActive" = FALSE` in `"Jobs"`
6. **Sync new → Jobs** — every staging row not yet in `"Jobs"` is converted
   to a `"Jobs"` row + a v1 `"JobVersions"` row.
7. **Sync updates → Jobs** — staging rows whose `DateLastImported` differs
   from the corresponding `"Jobs"` row are refreshed; if NOC, location,
   positions, posted date, or active flag changed, a new `"JobVersions"`
   row is created.

The console output uses single-character markers per job (`I`/`U`/`S`/`H`/`E`)
for compact log lines.

## Database (existing schema — no migrations owned here)

| Table | Purpose |
|---|---|
| `"ImportedJobsWanted"` | Staging — raw JSON in `"JobPostEnglish"`. |
| `"Jobs"` | Canonical, denormalised job records consumed by the API. |
| `"JobVersions"` | Version history (NOC / location / positions / active changes). |
| `"ExpiredJobs"` | Archive — drained by the indexer to remove ES docs. |
| `"DeletedJobs"` | Blacklist (read-only). |
| `"JobIds"` | Master ID list — FK target. |

This importer does **not** create or alter tables. Database migrations are
owned by `WorkBC.Admin` / `EFMigrationRunner`.

## Configuration

All settings come from environment variables. Copy `.env.example` → `.env` for
local development; in Kubernetes use a ConfigMap / Secret.

```bash
cp .env.example .env
# Edit .env — at a minimum set DB_USER, DB_PASSWORD, API_KEY.
```

### Required

| Var | Notes |
|---|---|
| `DB_HOST`, `DB_PORT`, `DB_NAME`, `DB_USER`, `DB_PASSWORD` | Postgres connection. The DB user needs write access to the tables listed above. |
| `API_BASE_URL` | Innovibe API root (e.g. `https://api-prod.jobs.innovibe.ca/api/v1`). |
| `API_KEY` | Innovibe-issued bearer key. The api returns `401 Unauthorized` if missing or wrong. |

### Optional

| Var | Default | Notes |
|---|---|---|
| `PAGE_SIZE` | `100` | Items per `/jobs` API page. |
| `INCLUDE_NOC_UNMATCHED` | `false` | Forward jobs whose NOC code didn't match a Statistics Canada code. Usually you want these excluded — they're often miscoded postings. |
| `LOG_LEVEL` | `INFO` | `DEBUG`, `INFO`, `WARNING`, `ERROR`, `CRITICAL`. |

## CLI Flags

| Flag | Purpose |
|---|---|
| `--bulk` | Fetch ALL jobs (drop the `postedFrom=yesterday` filter). Use after a long outage or when you suspect missed days. Heavier on the API and DB; takes minutes rather than seconds. |

## Build & Run

### Docker (standalone)

```bash
# Build the image
docker build -t workbc/importers-innovibe:latest .

# Daily import (yesterday + today)
docker run --rm --env-file .env workbc/importers-innovibe:latest

# Bulk import (every job, no date filter)
docker run --rm --env-file .env workbc/importers-innovibe:latest --bulk
```

### Docker Compose (integrated with the WorkBC dev stack)

From the `src/` directory:

```bash
docker compose build importers-innovibe
docker compose run --rm importers-innovibe
docker compose run --rm importers-innovibe --bulk
```

The compose service `importers-innovibe` has `scale: 0` — built but not
started by `docker compose up`. Use `docker compose run` for one-off
invocations, matching how the cron triggers it in production.

### Production shell — kubectl exec into the `php-cli` pod

The shared shell pod `php-cli` (built from `src/php-cli.Dockerfile`) bakes
this importer at `/app/workbc-importers-innovibe`. Operators land on it via:

```bash
kubectl exec -it <php-cli-pod-name> -- bash
# Sample-commands banner prints automatically. Then:
cd /app/workbc-importers-innovibe
php src/import.php          # daily
php src/import.php --bulk   # bulk
```

This is the recommended path for manual runs in test/prod — the cron pod
exits as soon as the daily import finishes, so it is not available for exec.

### Local PHP (development only)

```bash
composer install
cp .env.example .env
# Fill in DB_USER, DB_PASSWORD, API_KEY
php src/import.php          # daily
php src/import.php --bulk   # bulk
```

## Verifying a manual run via Postgres

After running an import manually you usually want to confirm data landed.
From the `php-cli` pod (`psql` is baked in) — or any host with psql:

```sql
-- High-level counts
SELECT (SELECT COUNT(*) FROM "ImportedJobsWanted") AS staging,
       (SELECT COUNT(*) FROM "Jobs" WHERE "JobSourceId" = 2
                                       AND "IsActive" = TRUE) AS active_innovibe,
       (SELECT COUNT(*) FROM "ExpiredJobs"
                                       WHERE "DateRemoved" > NOW() - INTERVAL '1 hour') AS expired_recent;

-- Rows just touched by the run
SELECT "JobId", LENGTH("JobPostEnglish") AS payload_bytes,
       "DateLastSeen", "DateLastImported"
FROM "ImportedJobsWanted"
WHERE "DateLastImported" > NOW() - INTERVAL '5 minutes'
LIMIT 10;
```

After a manual import you typically also want to **re-run the indexer** so
Elasticsearch reflects the new staging rows — that's a separate
`indexers-wanted` cron / pod.

## Troubleshooting

| Symptom | Likely cause / fix |
|---|---|
| `401 Unauthorized` on `/jobs` | `API_KEY` missing, expired, or wrong env. The key is per-environment — dev/test/prod use different keys. |
| `0 records returned by the API` despite normal API responses | `INCLUDE_NOC_UNMATCHED=false` (default) and Innovibe is currently only emitting unmatched-NOC jobs. Sanity-check by hitting the API directly with `curl`. |
| `Failed to connect to PostgreSQL` | `DB_HOST` unreachable from the pod. From the `php-cli` shell, try `psql "host=$DB_HOST port=$DB_PORT user=$DB_USER dbname=$DB_NAME"` to isolate. |
| `Expired-jobs API call failed: 401` | Same as the listing endpoint — check `API_KEY`. The importer continues through the SQL stages even if expiry fails, so partial recovery is automatic. |
| Bulk run runs out of memory | `--bulk` returns the entire current catalog. The image's `php.ini` sets `memory_limit = 512M` which has been enough so far; bump via `php -d memory_limit=1G src/import.php --bulk` for larger one-off runs. |
| Jobs reappear immediately after expiring | The Innovibe expiry feed has a 6-hour refresh window; if the publisher un-expires a job (rare), the next run will reinsert it. Expected behavior. |
