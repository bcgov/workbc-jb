# WorkBC Job Importer

PHP cron job that fetches jobs from the **Jobs Innovibe API** and syncs them into an existing **PostgreSQL** database. Packaged as a Docker image, scheduled via **Kubernetes CronJob** every 6 hours.

By default, each run fetches only jobs posted **yesterday and today** (`postedFrom` / `postedTo`). Use `php src/import.php --bulk` to fetch all jobs.

## Structure

```
job-importer/
├── src/
│   ├── Api/InnovibeApiClient.php       # Cursor-paginated API client
│   ├── Config/AppConfig.php            # Env-var config
│   ├── Service/JobImportService.php    # Import logic
│   └── import.php                      # Entry point
├── Dockerfile                          # Multi-stage Alpine build
├── .env                                # Local config
├── composer.json
```

## Import Flow

1. **Fetch** — GET jobs from Innovibe API with date filter (`postedFrom`/`postedTo`), cursor pagination
2. **Filter** — Skip jobs without salary data (`salaryMin`, `salaryMax`, `salaryValue` all empty)
3. **Upsert** — Insert new / update changed jobs in `"ImportedJobsWanted"` (duplicate hash check)
4. **Mark seen** — Update `"DateLastSeen"` on returned jobs
5. **Purge** — Move expired jobs to `"ExpiredJobs"`, delete from staging
6. **Sync new** — Insert new jobs into `"Jobs"` table
7. **Sync updates** — Update changed jobs in `"Jobs"` table
8. **Deactivate** — Set `"IsActive" = FALSE` on expired entries

## Database (existing — no migrations)

| Table | Purpose |
|-------|---------|
| `"ImportedJobsWanted"` | Staging — raw JSON in `"JobPostEnglish"` |
| `"Jobs"` | Canonical job records |
| `"ExpiredJobs"` | Archived expired jobs |
| `"DeletedJobs"` | Blacklist (read-only) |
| `"JobIds"` | Master ID list |

## Configuration

All from environment variables (`.env` locally, ConfigMap/Secret in K8s):

| Variable | Default | Required |
|----------|---------|----------|
| `DB_HOST` | `postgres` | |
| `DB_PORT` | `5432` | |
| `DB_NAME` | `jobboard` | |
| `DB_USER` | `workbc` | |
| `DB_PASSWORD` | `workbc` | |
| `API_BASE_URL` | `https://api-prod.jobs.innovibe.ca/api/v1` | |
| `API_KEY` | | ✅ |
| `PAGE_SIZE` | `100` | |
| `JOB_EXPIRY_DAYS` | `30` | |
| `DAYS_TO_KEEP_SINCE_LAST_SEEN` | `2` | |
| `MAX_JOBS_TO_EXPIRE_AT_ONCE` | `1250` | |
| `LOG_LEVEL` | `INFO` | |
| `INCLUDE_NOC_UNMATCHED` | `false` | |

## Build & Run

```bash
# Build
docker build -t workbc/job-importer:latest .

# Run daily import (yesterday + today)
php src/import.php

# Run bulk import (ALL jobs, no date filter)
php src/import.php --bulk

# Run via Docker
docker run --rm --env-file .env workbc/job-importer:latest
```

## API Reference

```bash
# Daily import (default: postedFrom=yesterday, postedTo=today)
curl -H "x-api-key: KEY" "https://api-prod.jobs.innovibe.ca/api/v1/jobs?limit=100&state=British+Columbia&includeExpired=false&includeNocUnmatched=false&postedFrom=2026-02-18&postedTo=2026-02-19"

# Paginate with cursor
curl -H "x-api-key: KEY" "https://api-prod.jobs.innovibe.ca/api/v1/jobs?limit=100&cursor={nextCursor}"

# List companies (get IDs for excludeCompanyIds filter)
curl -H "x-api-key: KEY" "https://api-prod.jobs.innovibe.ca/api/v1/companies?q=CompanyName"

# Available filters (in InnovibeApiClient.php, uncomment as needed):
#   includeNoSalary, company, excludeCompany[], includeCompanyIds[], excludeCompanyIds[]
```
