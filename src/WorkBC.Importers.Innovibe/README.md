# WorkBC.Importers.Innovibe

PHP cron job that fetches jobs from the **Innovibe API** and syncs them into the existing **PostgreSQL** database used by the WorkBC Job Board. Packaged as a Docker image and scheduled via **Kubernetes CronJob**.

By default each run fetches only jobs posted **yesterday and today** (`postedFrom`). Use the `--bulk` flag to fetch all jobs regardless of date.

## Project Structure

```
WorkBC.Importers.Innovibe/
├── src/
│   ├── Api/InnovibeApiClient.php       # Cursor-paginated API client
│   ├── Config/AppConfig.php            # Env-var configuration
│   ├── Service/JobImportService.php    # Import / upsert / purge logic
│   └── import.php                      # CLI entry point
├── Dockerfile                          # Multi-stage Alpine build
├── .dockerignore
├── .env.example                        # Template — copy to .env
├── .gitignore
├── composer.json
└── README.md
```

## Import Flow

1. **Fetch** — paginate through the Innovibe `/api/v1/jobs` API (cursor-based) with date and province filters
2. **Filter** — skip jobs without salary data (`salaryMin`, `salaryMax`, `salaryValue` all empty)
3. **Upsert** — insert new / update changed jobs in `"ImportedJobsWanted"` (duplicate hash check)
4. **Mark seen** — update `"DateLastSeen"` on returned jobs
5. **Expire** — call `/api/v1/jobs/expired/ids` (no date parameters) to get all expired IDs from the last 3 months (updated by Innovibe every 6 hours). Jobs matching IDs in our DB are moved to `"ExpiredJobs"`, deleted from staging, and set `"IsActive" = FALSE` in `"Jobs"`
6. **Sync new** — insert new jobs into `"Jobs"` table
7. **Sync updates** — update changed jobs in `"Jobs"` table

## Database (existing — no schema changes)

| Table | Purpose |
|---|---|
| `"ImportedJobsWanted"` | Staging — raw JSON in `"JobPostEnglish"` |
| `"Jobs"` | Canonical job records |
| `"ExpiredJobs"` | Archived expired jobs |
| `"DeletedJobs"` | Blacklist (read-only) |
| `"JobIds"` | Master ID list |

## Configuration

All settings are provided via **environment variables**. Locally, copy `.env.example` to `.env` and fill in the required values. In Kubernetes, use ConfigMap / Secret.

```bash
cp .env.example .env
# Edit .env — set DB_USER, DB_PASSWORD, API_KEY at minimum
```

## Build & Run

### Docker (recommended)

```bash
# Build the image
docker build -t workbc/importers-innovibe:latest .

# Daily import (yesterday + today)
docker run --rm --env-file .env workbc/importers-innovibe:latest

# Bulk import (all jobs, no date filter)
docker run --rm --env-file .env workbc/importers-innovibe:latest --bulk
```

### Docker Compose (integrated with WorkBC stack)

From the `src/` directory:

```bash
# Build
docker compose build importers-innovibe

# Run once
docker compose run --rm importers-innovibe

# Bulk import
docker compose run --rm importers-innovibe --bulk
```

### Local PHP (development only)

```bash
composer install
cp .env.example .env
# Fill in .env
php src/import.php          # daily
php src/import.php --bulk   # bulk
```
